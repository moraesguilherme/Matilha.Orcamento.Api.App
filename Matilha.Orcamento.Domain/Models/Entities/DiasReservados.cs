using Orcamento.Models.Enums;

namespace Orcamento.Models.Entities
{
    public class DiasReservados
    {
        public DateTime DataReserva { get; set; }
        public string DiaDaSemana { get; set; }
        public decimal ValorDiaria { get; set; }
        public TemporadaId TemporadaId { get; set; }
        public bool Feriado { get; set; } = false;
    }
}
