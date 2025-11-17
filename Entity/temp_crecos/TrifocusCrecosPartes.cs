using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity.temp_crecos
{
    public class TrifocusCrecosPartes
    {
        [Key]
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? CodOperacion { get; set; }
        public Decimal? ValorLiquidacion { get; set; }
        public Decimal? ValorLiquidacionParte { get; set; }
        public Decimal? ValorPonteAlDia { get; set; }

    }
}
