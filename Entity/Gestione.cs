using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


// Gestione.cs
public partial class Gestione
{
    [Key]
    public string IdGestion { get; set; }

    public Guid IdDeuda { get; set; } = Guid.Empty;
    public DateTime FechaGestion { get; set; }
    public string Descripcion { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string IdUsuarioGestiona { get; set; } = null!;

    // Corregido nombre de propiedad
    public string IdTipoContactoResultado { get; set; } = null!;
    public string IdTipoResultado { get; set; } = null!;
    public string IdRespuestaTipoContacto { get; set; } = null!;

    // Navegaciones
    public virtual Deuda IdDeudaNavigation { get; set; } = null!;
    public virtual TipoResultado IdTipoResultadoNavigation { get; set; } = null!;
    public virtual Usuario IdUsuarioGestionaNavigation { get; set; } = null!;
    public virtual TipoContactoResultado IdTipoContactoResultadoNavigation { get; set; } = null!;
    public virtual RespuestaTipoContacto RespuestaTipoContactoNavigation { get; set; } = null!;
}
