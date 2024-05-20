using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Orcamento.Models.Dtos
{
    public class OrcamentoDto
    {
        public int Id { get; set; }
        public ClienteOrcamento Cliente { get; set; }
        public string NumeroProposta { get; set; }
        public StatusProposta StatusProposta { get; set; }
        public DateTime? DataProposta { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public int QtdDeCaes { get; set; }
        public TemporadaId TemporadaId { get; set; }
        public DateTime DataEntrada { get; set; }
        public TimeSpan HorarioEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public TimeSpan HorarioSaida { get; set; }
        public PeriodoReserva PeriodoReserva { get; set; }
        public string Observacoes { get; set; }
        public decimal? ValorHospedageUnitarioPorCao { get; set; }
        public decimal? Subtotal { get; set; }
        public int? QtdAvaliacao { get; set; }
        public decimal? ValorUnitarioAvaliacao { get; set; }
        public decimal? ValorTotalAvaliacao { get; set; }
        public int? QtdAdaptacao { get; set; }
        public decimal? ValorUnitarioAdaptacao { get; set; }
        public decimal? ValorTotalAdaptacao { get; set; }
        public bool? UtilizaTaxi { get; set; }
        public int? QtdTaxi { get; set; }
        public decimal? ValorViagemTaxi { get; set; }
        public decimal? ValorTotalTaxi { get; set; }
        public decimal? Desconto { get; set; }
        public decimal? ValorFinal { get; set; }
        public int ClienteOrcamentoId { get; set; }
        public decimal ValorTotalDescontos { get; set; }
        public TipoDesconto TipoDesconto { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
