using Orcamento.Models.Enums;

namespace Orcamento.Models.Entities
{
    public class OrcamentoEntrada
    {
        public ClienteOrcamento Cliente { get; set; }
        //public TemporadaId Temporada { get; set; }
        public DateTime DataEntrada { get; set; }
        public TimeSpan HorarioEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public TimeSpan HorarioSaida { get; set; }
        public string? Observacoes { get; set; } 
        public int QtdAvaliacoes { get; set; }
        public decimal ValorAvaliacao { get; set; }
        public int QtdAdaptacao {  get; set; }
        public decimal ValorAdaptacao { get; set; }
        public bool UtilizaTaxi { get; set; }
        public int QtdTaxi { get; set; }
        public decimal ValorTaxi { get; set; }
        public decimal Desconto { get; set; }
        public TipoDesconto TipoDesconto { get; set; }
        public DateTime DataExpiracao { get; set; }
     }
}
