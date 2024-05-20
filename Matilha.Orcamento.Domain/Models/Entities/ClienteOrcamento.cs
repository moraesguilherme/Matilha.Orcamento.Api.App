namespace Orcamento.Models.Entities
{
    public class ClienteOrcamento
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; }
        public string Email { get; set; }
        public string TelefoneCliente { get; set; }
        public List<CaoOrcamento> CaoOrcamento { get; set; }
    }
}
