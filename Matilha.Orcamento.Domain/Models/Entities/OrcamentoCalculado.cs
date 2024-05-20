namespace Orcamento.Models.Entities
{
    public class OrcamentoCalculado
    {
        public decimal TotalDiasSemana { get; set; }
        public decimal TotalFinaisDeSemana { get; set; }
        public decimal TotalFeriados { get; set; }
        public decimal TotalServicosAdicionais { get; set; }
        public decimal TotalUnitarioPorCaes { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAntesDescontos { get; set; }
        public decimal ValorTotalGeral { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ValorTotalAvaliacao { get; set; }
        public decimal ValorTotalAdaptacao {  get; set; }
        public decimal ValorTotalTaxi {  get; set; }
        public decimal ValorTotalDeDescontos { get; set; }
    }

}
