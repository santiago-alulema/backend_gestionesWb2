using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity;


public partial class DeudorTelefono
{
    [Key]

    public String IdDeudorTelefonos { get; set; }

    public string IdDeudor { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public DateTime FechaAdicion { get; set; }

    public bool? EsValido { get; set; }

    public string? Origen { get; set; }

    public string? Observacion { get; set; }


    public virtual Deudores IdDeudorNavigation { get; set; } = null!;
}
