using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class Deudores
{
    [Key]

    public string IdDeudor { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public string Telefono { get; set; } = null!;

    public string? Correo { get; set; }

    public string? Descripcion { get; set; }
    public string? IdUsuario { get; set; }

    public virtual ICollection<Deuda> Deuda { get; set; } = new List<Deuda>();

    public virtual ICollection<DeudorTelefono> DeudorTelefonos { get; set; } = new List<DeudorTelefono>();
    public virtual Usuario Usuario { get; set; } = null!;
}
