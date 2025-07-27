using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class AsignacionesCartera
{

    [Key]
    public int IdAsignacion { get; set; }

    public Guid IdDeuda { get; set; } = Guid.Empty;

    public string IdCliente { get; set; } = null!;

    public DateOnly FechaAsignacion { get; set; }

    public DateOnly? FechaRetiro { get; set; }

    public string? Estado { get; set; }

    public string? Observaciones { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Deuda IdDeudaNavigation { get; set; } = null!;
}
