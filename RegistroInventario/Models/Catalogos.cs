


using System.ComponentModel.DataAnnotations;

namespace GoldCrew.Data.Models;

public enum UnidadMedida
{
    Unidad,
    Kilogramo,
    Gramo,
    Litro,
    Mililitro,
    Metro,
    Centimetro,
    Caja,
    Paquete
}

public class Usuario
{
    public int Id{get; set;}
    [Required, StringLength(40)]
    public string NombreUsuario{get; set;} = "";
    [Required, StringLength(100)]
    public string NombreCompleto{get; set;}= "";
    [Required]
    public string ClaveHash{get; set;}="";
    public RolUsuario Rol{get; set;}
    public bool ACtivo {get; set;}=true;

    public class Categoria
    {
        public int Id{get; set;}
        [Required(ErrorMessage = "El nombre de a categoria es obligatorio"), StringLength(80)]
        public string Nombre{get; set;}= "";
        [StringLength(250)]
        public string? Descripcion{get; set;}
        public bool Activo {get; set;} = true;
    }

    public class Proveedor
    {
        public int Id{get; set;}
        [Required(ErrorMessage = "el nombre del proveedor es obligatorio"), StringLength(120)]
        public string Nombre{get; set;}= "";
        [Required, StringLength(11,MinimumLength =9)]
        public string Rnc{get; set;}="";
        [StringLength(20)]
        public string? Telefono{get; set;}
        [EmailAddress, StringLength(120)]
        public string? Correo{get; set;}
        [StringLength(250)]
        public string? Direccion {get; set;}
        public bool Activo{get; set;}= true;
    }

    public class Ubicacion
    {
        public int Id {get; set;}
        [Required(ErrorMessage ="El codigo debe ser obligatorio"),StringLength(20)]
        public string Codigo{get; set;}="";
        [StringLength(250)]
        public string? Descripcion{get; set;}
        public bool Activo {get; set;}= true;

    }


    public class Producto
    {
        public int Id{get; set;}
        [Required(ErrorMessage ="El codigo es obligatorii"),StringLength(30)]
        public string Codigo{get; set;}="";
        [Required(ErrorMessage ="El nombre es obligatorio"), StringLength(120)]
        public string Nombre{get; set;}="";
        public int CategoriaId{get; set;}
        public Categoria? Categoria{get; set;}
        public int ProveedorId{get; set;}
        public Proveedor? Proveedor{get; set;}
        public int? UbicacionId{get; set;}
        public Ubicacion? Ubicacion{get; set;}
        public UnidadMedida UnidadMedida{get; set;}
        [Range(0, double.MaxValue)]
        public decimal PrecioCompra{get; set;}
        [Range(0, double.MaxValue)]
        public decimal PrecioVenta{get;set;}
        public int Existencia{get; set;}
        [Range(0, int.MaxValue)]
        public int StockMinimo{get; set;}
        public DateTime? FechaVencimiento{get;set;}
        public DateTime FechaRegistro{get; set;}= DateTime.Today;
        public bool Descontinuado{get; set;}
        public bool Activo{get; set;}=true;
    }
}