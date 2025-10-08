using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class Deuda
{
    [Key]
    public Guid IdDeuda { get; set; } = Guid.NewGuid();
    public string? IdDeudor { get; set; }
    public Decimal? DeudaCapital { get; set; }
    public Decimal? Interes { get; set; }
    public Decimal? GastosCobranzas { get; set; }
    public Decimal? SaldoDeuda { get; set; }
    public int? Descuento { get; set; }
    public Decimal? MontoCobrar { get; set; } = 0m;
    public Decimal? MontoCobrarPartes { get; set; } = 0m;
    public DateOnly? FechaVenta { get; set; }
    public DateOnly? FechaUltimoPago { get; set; }
    public string? Estado { get; set; }
    public int? DiasMora { get; set; }
    public string? NumeroFactura { get; set; }
    public string? Clasificacion { get; set; }
    public int? Creditos { get; set; }
    public int? NumeroCuotas { get; set; }
    public string? TipoDocumento { get; set; }
    public Decimal? ValorCuota { get; set; }
    public string? Tramo { get; set; }
    public Decimal? UltimoPago { get; set; }
    public string? ProductoDescripcion { get; set; }
    public string? Agencia { get; set; }
    public string? Ciudad { get; set; }
    public string? Empresa { get; set; }
    public string? CodigoEmpresa { get; set; }
    public bool? EsActivo { get; set; }
    public string? CodigoOperacion { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public virtual ICollection<AsignacionesCartera> AsignacionesCarteras { get; set; } = new List<AsignacionesCartera>();
    public virtual ICollection<CompromisosPago> CompromisosPagos { get; set; } = new List<CompromisosPago>();
    public virtual ICollection<Gestione> Gestiones { get; set; } = new List<Gestione>();
    public virtual Deudores? IdDeudorNavigation { get; set; }
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    public string? IdUsuario { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;

}
