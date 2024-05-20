namespace Orcamento.Models.Dtos
{
    public class EntradaOrcamentoDto
    {
        public int Id { get; set; }
        public string? NumeroProposta { get; set; }
        public int QtdDeCaes { get; set; }
        public int TemporadaId { get; set; }
        public DateTime DataEntrada { get; set; }
        public TimeSpan HorarioEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public TimeSpan HorarioSaida { get; set; }
        public decimal ValorDiaria { get; set; }
        public string? Observacoes { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ValorAvaliacao { get; set; }
        public decimal ValorAdaptacao { get; set; }
        public bool UtilizaTaxi { get; set; }
        public int QtdTaxi { get; set; }
        public decimal ValorTaxi { get; set; }
        public decimal PorcentagemDesconto { get; set; }
        public decimal ValorFinal { get; set; }
        public int FamiliaId { get; set; }

        // Propriedade de navegação para a entidade Familia (caso exista)
        //public Familia Familia { get; set; }
    }
}
