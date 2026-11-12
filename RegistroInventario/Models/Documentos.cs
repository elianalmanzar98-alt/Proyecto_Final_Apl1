


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GoldCrew.Data.Enums;
using static GoldCrew.Data.Models.Usuario;

namespace   GoldCrew.Data.Models;

public class ReporteIncidente
{
    public int Id{get; set;}
    public DateTime Fecha{get; set;} = DateTime.Today;
    [Required, StringLength(30)]
    public string NumeroGuia{get; set;}="";
    public int ProductoId{get; set;}
    public Producto? Producto{get; set;}
    [Range(1, int.MaxValue, ErrorMessage ="LA Cantidad debe ser mayor a cero (0)")]
    public int CantidadAfectada{get; set;}
    [Required, StringLength(500)]
    public string Descripcion{get; set;}="";
    public int CreadoPorId{get; set;}
    public Usuario? CreadoPor{get; set;}
    public bool Activo{get; set;}= true;
}


public class InformeBaja
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    [Range(1, int.MaxValue)] public int Cantidad { get; set; }
    public MotivoBaja Motivo { get; set; }
    [StringLength(500)] public string? Observaciones { get; set; }
    public int CreadoPorId { get; set; }
    public Usuario? CreadoPor { get; set; }
    public bool Activo { get; set; } = true;
}

public class OrdenCompra
{
    public int Id { get; set; }
    [Required, StringLength(20)] public string Numero { get; set; } = "";   // OC-0001
    public DateTime Fecha { get; set; } = DateTime.Today;
    public int ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }
    public EstadoOrdenCompra Estado { get; set; } = EstadoOrdenCompra.Creada;
    [StringLength(500)] public string? Observaciones { get; set; }
    public int CreadoPorId { get; set; }
    public Usuario? CreadoPor { get; set; }
    public bool Activo { get; set; } = true;
    public List<OrdenCompraDetalle> Detalles { get; set; } = new();
    [NotMapped] public decimal Total => Detalles.Sum(d => d.Subtotal);
}

public class OrdenCompraDetalle
{
    public int Id { get; set; }
    public int OrdenCompraId { get; set; }
    public OrdenCompra? OrdenCompra { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    [Range(1, int.MaxValue)] public int Cantidad { get; set; }
    [Range(0, double.MaxValue)] public decimal PrecioUnitario { get; set; }
    [StringLength(30)] public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    [NotMapped] public decimal Subtotal => Cantidad * PrecioUnitario;
}

public class NotaDespacho
{
    public int Id { get; set; }
    [Required, StringLength(20)] public string Numero { get; set; } = "";   // ND-0001
    public DateTime Fecha { get; set; } = DateTime.Today;
    [Required, StringLength(120)] public string Destinatario { get; set; } = "";
    [Required, StringLength(250)] public string DireccionEntrega { get; set; } = "";
    [StringLength(500)] public string? Observaciones { get; set; }
    public int ResponsableId { get; set; }
    public Usuario? Responsable { get; set; }
    public bool Activo { get; set; } = true;
    public List<NotaDespachoDetalle> Detalles { get; set; } = new();
}

public class NotaDespachoDetalle
{
     public int Id { get; set; }
    public int NotaDespachoId { get; set; }
    public NotaDespacho? NotaDespacho { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    [Range(1, int.MaxValue)] public int Cantidad { get; set; }
}

public class MovimientoInventario
{
     public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; }
    public int ExistenciaResultante { get; set; }
    [StringLength(30)] public string Referencia { get; set; } = "";   // OC-0001, ND-0003, BAJA-0002
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
