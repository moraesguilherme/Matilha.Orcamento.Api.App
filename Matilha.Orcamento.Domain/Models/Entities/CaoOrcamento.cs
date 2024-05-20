using System.Reflection.PortableExecutable;

namespace Orcamento.Models.Entities
{
    public class CaoOrcamento
    {
        public int Id { get; set; }
        public string NomeCachorro { get; set; }
        public string Raca { get; set; }
        public char Sexo { get; set; }
        public int Idade { get; set; }
        public bool Castrado { get; set; }
        public decimal Peso { get; set; }
        public bool Ativo { get; set; }
        public bool Deletado { get; set; }
    }
}
