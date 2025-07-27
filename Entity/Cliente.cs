using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class Cliente
{
    [Key]
    public string IdCliente { get; set; } = null!;

    public string Nombre { get; set; } = null!;
    public string Cedula { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? TelefonoContacto { get; set; }

    public string? Correo { get; set; }

    public virtual ICollection<AsignacionesCartera> AsignacionesCarteras { get; set; } = new List<AsignacionesCartera>();
}
