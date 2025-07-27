using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class Pago
{
    [Key]
    public int IdPago { get; set; }

    public Guid IdDeuda { get; set; } = Guid.Empty;

    public DateOnly? FechaPago { get; set; }

    public decimal MontoPagado { get; set; }

    public string? MedioPago { get; set; }

    public string? Observaciones { get; set; }

    public virtual Deuda IdDeudaNavigation { get; set; } = null!;
}
