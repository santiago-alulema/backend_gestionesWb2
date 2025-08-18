using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class CompromisosPago
{
    [Key]
    public string IdCompromiso { get; set; }
    public Guid? IdDeuda { get; set; }
    public DateOnly FechaCompromiso { get; set; }
    public decimal MontoComprometido { get; set; }
    public bool? Estado { get; set; } 
    public bool? IncumplioCompromisoPago { get; set; }
    public DateOnly? FechaCumplimientoReal { get; set; }
    public string? Observaciones { get; set; }
    public string IdUsuario { get; set; } = null!;
    public string IdTipoTarea { get; set; } = null!;
    public string HoraRecordatorio { get; set; } = null!;
    public virtual Deuda? IdDeudaNavigation { get; set; }
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    public virtual TipoTarea IdTipoTareaNavigation { get; set; } = null!;

}
