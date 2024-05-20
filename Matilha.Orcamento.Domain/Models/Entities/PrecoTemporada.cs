using Orcamento.Models.Enums;

namespace Orcamento.Models.Entities
{
    public class PrecoTemporada
    {
        public string TipoTemporada { get; set; }
        public decimal ValorDiaDeSemana { get; set; }
        public decimal ValorFinalDeSemana { get; set; }
        public decimal ValorFeriado { get; set; }
        public decimal ValorMeioPeriodoSemana { get; set; }
        public decimal ValorPernoite { get; set; }
        public decimal ValorMeioPeriodoFimDeSemana { get; set; }
        public TemporadaId TemporadaId { get; set; }
    }
}