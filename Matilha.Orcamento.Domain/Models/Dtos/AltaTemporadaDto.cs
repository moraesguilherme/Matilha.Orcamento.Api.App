namespace Orcamento.Models.Dtos
{
    public class AltaTemporadaDto
    {
        public int Id { get; set; }
        public int TemporadaId { get; set; }
        public DateTime Data { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string Descricao { get; set; }
        public int EmpresaId { get; set; }
    }
}
