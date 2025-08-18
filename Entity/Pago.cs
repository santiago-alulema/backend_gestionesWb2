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
    public DateTime? FechaRegistro { get; set; }

    public decimal MontoPagado { get; set; }
    public string? MedioPago { get; set; }
    public string? NumeroDocumenro { get; set; }
    public string? Observaciones { get; set; }
    public string? FormaPagoId { get; set; }
    public string? IdUsuario { get; set; }
    public FormaPago FormaPagoNavigation { get; set; }
    public virtual Deuda IdDeudaNavigation { get; set; } = null!;
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;


    public string? IdBancosPago { get; set; }
    public virtual BancosPagos BancosNavigation { get; set; } = null!;

    public string? IdTipoCuentaBancaria { get; set; }
    public virtual TipoCuentaBancaria TipoCuentaBancariaNavigation { get; set; } = null!;
    public string? IdTipoTransaccion { get; set; }
    public virtual TipoTransaccion TipoTransaccionNavigation { get; set; } = null!;

    public string? IdAbonoLiquidacion { get; set; }
    public virtual AbonoLiquidacion AbonoLiquidacionNavigation { get; set; } = null!;



}
