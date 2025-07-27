using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class Deuda
{
    [Key]
    public Guid IdDeuda { get; set; } = Guid.Empty;

    public string? IdDeudor { get; set; }

    public decimal MontoOriginal { get; set; }

    public decimal SaldoActual { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public DateOnly? FechaAsignacion { get; set; }

    public string? Estado { get; set; }

    public string? Descripcion { get; set; }
    
    public string? NumeroFactura { get; set; } = string.Empty; 
    
    public Decimal? TotalFactura { get; set; } = Decimal.Zero;
    
    public string? NumeroAutorizacion { get; set; } = string.Empty ;

    public Decimal? SaldoDeuda { get; set; } = Decimal.Zero; 
    public int? NumeroCuotas { get; set; } = 0;
    public int? CuotaActual { get; set; } = 0;
    public Decimal? ValorCuota { get; set; } = Decimal.Zero;
    public String? Tramo { get; set; } = string.Empty;
    public Decimal? UltimoPago { get; set; } = Decimal.Zero;
    public string Empresa { get; set; } = string.Empty;



    public virtual ICollection<AsignacionesCartera> AsignacionesCarteras { get; set; } = new List<AsignacionesCartera>();

    public virtual ICollection<CompromisosPago> CompromisosPagos { get; set; } = new List<CompromisosPago>();

    public virtual ICollection<Gestione> Gestiones { get; set; } = new List<Gestione>();

    public virtual Deudores? IdDeudorNavigation { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
