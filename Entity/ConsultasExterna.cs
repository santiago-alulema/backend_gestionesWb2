using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class ConsultasExterna
{
    [Key]
    public string IdDeudor { get; set; } = null!;

    public string JsonRespuesta { get; set; } = null!;

    public DateTime FechaConsulta { get; set; }

    public string? IdUsuario { get; set; }

    public virtual Deudores IdDeudorNavigation { get; set; } = null!;

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
