using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class Gestione
{
    [Key]

    public string  IdGestion { get; set; }

    public Guid IdDeuda { get; set; } = Guid.Empty;

    public DateTime FechaGestion { get; set; }

    public string IdTipoGestion { get; set; }

    public string Descripcion { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string IdUsuarioGestiona { get; set; } = null!;
    public string IdTipoContactoGestion { get; set; } = null!;
    public virtual Deuda IdDeudaNavigation { get; set; } = null!;
    public virtual TiposGestion IdTipoGestionNavigation { get; set; } = null!;
    public virtual Usuario IdUsuarioGestionaNavigation { get; set; } = null!;
    public virtual TipoContactoGestion IdTipoContactoGestionNavigation { get; set; } = null!;

    public string IdRespuestaTipoContacto { get; set; } = null!;
    public virtual RespuestaTipoContacto RespuestaTipoContactoNavigation { get; set; } = null!;


}
