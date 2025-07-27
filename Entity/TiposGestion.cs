using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class TiposGestion
{
    [Key]
    public string IdTipoGestion { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }
    public Boolean? Estado { get; set; }

    public string? IdPadre { get; set; }

    /// <summary>
    /// P Padre H Hijo
    /// </summary>
    public string TipoGestion { get; set; } = null!;

    public virtual ICollection<Gestione> Gestiones { get; set; } = new List<Gestione>();

    public virtual TiposGestion? IdPadreNavigation { get; set; }

    public virtual ICollection<TiposGestion> InverseIdPadreNavigation { get; set; } = new List<TiposGestion>();
}
