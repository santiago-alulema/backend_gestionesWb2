using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace gestiones_backend.Entity;

public partial class Usuario
{
    [Key]
    public string IdUsuario { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public string Contrasena { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string CodigoUsuario { get; set; } = null!;
    public virtual ICollection<CompromisosPago> CompromisosPagos { get; set; } = new List<CompromisosPago>();
    public virtual ICollection<Gestione> Gestiones { get; set; } = new List<Gestione>();
    public virtual ICollection<Deudores> Deudores { get; set; } = new List<Deudores>();
    public virtual ICollection<Deuda> Deudas { get; set; } = new List<Deuda>();
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    //public virtual ICollection<MensajeWhatsappUsuario> MensajesWhatsapp { get; set; } = new List<MensajeWhatsappUsuario>();
}
