using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace gestiones_backend.Entity;

public partial class Usuario
{
    [Key]
    public string IdUsuario { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;

    /// <summary>
    /// g Gestor A Admin
    /// </summary>
    public string Rol { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public virtual ICollection<CompromisosPago> CompromisosPagos { get; set; } = new List<CompromisosPago>();

    public virtual ICollection<Gestione> Gestiones { get; set; } = new List<Gestione>();

    public virtual ICollection<Deudores> Deudores { get; set; } = new List<Deudores>();
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
