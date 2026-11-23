using GoldCrew.Data;
using GoldCrew.Data.Enums;
using GoldCrew.Data.Models;
using Microsoft.EntityFrameworkCore;
using static GoldCrew.Data.Models.Usuario;
namespace GoldCrew.Services.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(GoldCrewDbContext db)
    {
        if (await db.Usuarios.AnyAsync()) return;
        var hoy = DateTime.Today;
        static string Hash(string s) => BCrypt.Net.BCrypt.HashPassword(s);

        // para los Usuarios
        var us = new[]
        {
            new Usuario { NombreUsuario = "admin", NombreCompleto = "Administrador del Sistema", Rol = RolUsuario.Administrador, ClaveHash = Hash("Admin123*") },
            new Usuario { NombreUsuario = "almacen", NombreCompleto = "Juan Pérez", Rol = RolUsuario.Almacenista, ClaveHash = Hash("Almacen123*") },
            new Usuario { NombreUsuario = "rmartinez", NombreCompleto = "Rosa Martínez", Rol = RolUsuario.Almacenista, ClaveHash = Hash("Almacen123*") },
            new Usuario { NombreUsuario = "contabilidad", NombreCompleto = "Carmen Rodríguez", Rol = RolUsuario.Contabilidad, ClaveHash = Hash("Conta123*") },
            new Usuario { NombreUsuario = "lalmonte", NombreCompleto = "Luis Almonte", Rol = RolUsuario.Contabilidad, ClaveHash = Hash("Conta123*") },
        };
        var alm = us[1]; var alm2 = us[2];

        //  para los Catalogos 
        var cats = new (string n, string d)[]
        {
            ("Bebidas", "Aguas, refrescos y jugos"),
            ("Licores y Cervezas", "Ron, whisky, cervezas y vinos"),
            ("Alimentos y Cocina", "Insumos secos y aceites para cocina"),
            ("Limpieza e Higiene", "Desinfectantes y productos de limpieza"),
            ("Desechables y Empaques", "Vasos, platos y empaques"),
        }.Select(x => new Usuario.Categoria { Nombre = x.n, Descripcion = x.d }).ToArray();

        var provs = new (string n, string rnc, string tel, string mail, string dir)[]
        {
            ("Distribuidora del Cibao SRL", "131245678", "809-555-0101", "ventas@distcibao.example.com", "Av. Circunvalación #45, Santiago"),
            ("Licores Nacionales Tenares SRL", "130987654", "809-555-0102", "pedidos@licoresnac.example.com", "Calle Duarte #12, Tenares"),
            ("Alimentos y Suministros Duarte SRL", "131556677", "809-555-0103", "contacto@alimduarte.example.com", "Carretera Duarte Km 5, San Francisco de Macorís"),
            ("Limpieza Total Macorís SRL", "131334455", "809-555-0104", "ventas@limpiezatotal.example.com", "Calle Castillo #78, San Francisco de Macorís"),
            ("Empaques del Nordeste SRL", "130223344", "809-555-0105", "info@empaquesnordeste.example.com", "Zona Industrial, San Francisco de Macorís"),
        }.Select(x => new Proveedor { Nombre = x.n, Rnc = x.rnc, Telefono = x.tel, Correo = x.mail, Direccion = x.dir }).ToArray();

        var ubis = new (string c, string d)[]
        {
            ("A-01", "Pasillo A, estante 1: bebidas no alcohólicas"),
            ("B-01", "Pasillo B, estante 1: licores y cervezas"),
            ("C-01", "Pasillo C, estante 1: alimentos secos y aceites"),
            ("D-01", "Pasillo D, estante 1: limpieza e higiene"),
            ("E-01", "Pasillo E, estante 1: desechables y empaques"),
        }.Select(x => new Usuario.Ubicacion { Codigo = x.c, Descripcion = x.d }).ToArray();

        //  para los Productos
        Producto P(string cod, string nom, int c, int pr, int ub, UnidadMedida um, decimal pc, decimal pv, int min, DateTime? venc) =>
            new() { Codigo = cod, Nombre = nom, Categoria = cats[c], Proveedor = provs[pr], Ubicacion = ubis[ub],
                    UnidadMedida = um, PrecioCompra = pc, PrecioVenta = pv, StockMinimo = min,
                    FechaVencimiento = venc, FechaRegistro = hoy.AddDays(-30) };

        var ps = new[]
        {
            P("BEB-001", "Agua mineral 500 ml (caja x24)", 0, 0, 0, UnidadMedida.Caja, 280, 450, 40, hoy.AddDays(300)),
            P("BEB-002", "Refresco cola 355 ml (caja x24)", 0, 0, 0, UnidadMedida.Caja, 520, 780, 25, hoy.AddDays(150)),
            P("LIC-001", "Ron añejo 750 ml", 1, 1, 1, UnidadMedida.Unidad, 650, 950, 20, null),
            P("LIC-002", "Cerveza nacional 12 oz (caja x24)", 1, 1, 1, UnidadMedida.Caja, 900, 1250, 30, hoy.AddDays(90)),
            P("COC-001", "Arroz selecto (saco 25 lb)", 2, 2, 2, UnidadMedida.Unidad, 1100, 1400, 20, hoy.AddDays(200)),
            P("COC-002", "Aceite vegetal 1 galón", 2, 2, 2, UnidadMedida.Galon, 480, 620, 12, hoy.AddDays(20)),
            P("LIM-001", "Desinfectante multiuso 1 galón", 3, 3, 3, UnidadMedida.Galon, 210, 320, 15, hoy.AddDays(400)),
            P("EMP-001", "Vasos desechables 12 oz (paquete x50)", 4, 4, 4, UnidadMedida.Paquete, 95, 150, 80, null),
        };
        var carga = new[] { 120, 60, 48, 35, 18, 30, 40, 200 };

        db.AddRange(us); db.AddRange(cats); db.AddRange(provs); db.AddRange(ubis); db.AddRange(ps);

        void Mover(Producto p, TipoMovimiento t, int cant, string refe, DateTime f, Usuario u)
        {
            p.Existencia += t is TipoMovimiento.Entrada or TipoMovimiento.Ajuste ? cant : -cant;
            db.Movimientos.Add(new MovimientoInventario
            {
                Producto = p, Tipo = t, Cantidad = cant, ExistenciaResultante = p.Existencia,
                Referencia = refe, Fecha = f, Usuario = u
            });
        }

        for (int i = 0; i < ps.Length; i++)
            Mover(ps[i], TipoMovimiento.Ajuste, carga[i], "CARGA-INICIAL", hoy.AddDays(-30), us[0]);

        //  Notas de despacho  
        NotaDespacho ND(int n, int dias, string dest, string dir, Usuario resp, params (int prod, int cant)[] items)
        {
            var nd = new NotaDespacho { Numero = $"ND-{n:0000}", Fecha = hoy.AddDays(-dias), Destinatario = dest,
                                        DireccionEntrega = dir, Responsable = resp, Observaciones = "Despacho regular" };
            foreach (var (pi, c) in items)
            {
                nd.Detalles.Add(new NotaDespachoDetalle { Producto = ps[pi], Cantidad = c });
                Mover(ps[pi], TipoMovimiento.Salida, c, nd.Numero, nd.Fecha, resp);
            }
            return nd;
        }
        db.NotasDespacho.AddRange(
            ND(1, 20, "Bar Principal", "Gold Club Tenares, planta baja", alm, (2, 6), (1, 10)),
            ND(2, 15, "Cocina", "Gold Club Tenares, área de cocina", alm, (5, 4), (6, 3)),
            ND(3, 10, "Salón de Eventos", "Gold Club Tenares, segundo nivel", alm2, (0, 20), (3, 8), (7, 30)),
            ND(4, 6, "Restaurante", "Gold Club Tenares, restaurante", alm, (1, 5), (0, 10)),
            ND(5, 2, "Área de Piscina", "Gold Club Tenares, zona exterior", alm2, (3, 6), (2, 4)));

        //  Informes de baja 
        InformeBaja B(int dias, int prod, int cant, MotivoBaja m, string obs, Usuario u)
        {
            var f = hoy.AddDays(-dias);
            Mover(ps[prod], TipoMovimiento.Baja, cant, $"BAJA-{f:yyyyMMdd}-{prod + 1}", f, u);
            return new InformeBaja { Fecha = f, Producto = ps[prod], Cantidad = cant, Motivo = m, Observaciones = obs, CreadoPor = u };
        }
        db.InformesBaja.AddRange(
            B(12, 5, 2, MotivoBaja.Vencimiento, "Lote con fecha vencida detectado en revisión", alm),
            B(9, 7, 10, MotivoBaja.Merma, "Paquetes húmedos por filtración", alm),
            B(7, 6, 1, MotivoBaja.Dano, "Envase roto durante el traslado", alm2),
            B(4, 0, 3, MotivoBaja.Dano, "Cajas aplastadas en el almacén", alm),
            B(1, 1, 2, MotivoBaja.Perdida, "Diferencia detectada en conteo físico", alm2));

        //  Reportes de incidentes 
        ReporteIncidente I(int dias, string guia, int prod, int cant, string desc, Usuario u) =>
            new() { Fecha = hoy.AddDays(-dias), NumeroGuia = guia, Producto = ps[prod], CantidadAfectada = cant, Descripcion = desc, CreadoPor = u };
        db.ReportesIncidente.AddRange(
            I(25, "GUIA-2026-0101", 0, 2, "Dos cajas llegaron con empaque dañado.", alm),
            I(18, "GUIA-2026-0114", 3, 1, "Faltó una caja respecto a la guía del proveedor.", alm2),
            I(11, "GUIA-2026-0120", 2, 2, "Dos botellas rotas en el transporte.", alm),
            I(5, "GUIA-2026-0133", 4, 1, "Saco de arroz con humedad.", alm2),
            I(3, "GUIA-2026-0138", 7, 5, "Cinco paquetes recibidos abiertos.", alm));

        //  ordenes de compra 
        OrdenCompra OC(int n, int dias, int prov, EstadoOrdenCompra est, string obs, params (int prod, int cant, decimal precio, string? lote, int? venc)[] items)
        {
            var oc = new OrdenCompra { Numero = $"OC-{n:0000}", Fecha = hoy.AddDays(-dias), Proveedor = provs[prov],
                                    Estado = est, Observaciones = obs, CreadoPor = alm };
            foreach (var it in items)
                oc.Detalles.Add(new OrdenCompraDetalle
                {
                    Producto = ps[it.prod], Cantidad = it.cant, PrecioUnitario = it.precio, Lote = it.lote,
                    FechaVencimiento = it.venc is null ? null : hoy.AddDays(it.venc.Value)
                });
            return oc;
        }
        db.OrdenesCompra.AddRange(
            OC(1, 5, 0, EstadoOrdenCompra.Creada, "Reposición de bebidas", (0, 60, 280, "L-2611A", 300), (1, 40, 520, "L-2611B", 150)),
            OC(2, 4, 1, EstadoOrdenCompra.Aprobada, "Reposición de cerveza y ron", (3, 50, 900, "L-2611C", 120), (2, 24, 650, null, null)),
            OC(3, 3, 2, EstadoOrdenCompra.Aprobada, "Arroz bajo el mínimo y aceite", (4, 30, 1100, "L-2611D", 220), (5, 24, 480, "L-2611E", 180)),
            OC(4, 2, 3, EstadoOrdenCompra.Recibida, "Productos de limpieza", (6, 30, 210, "L-2611F", 400)),
            OC(5, 1, 4, EstadoOrdenCompra.Creada, "Vasos para eventos", (7, 100, 95, null, null)));

        await db.SaveChangesAsync();
    }
}