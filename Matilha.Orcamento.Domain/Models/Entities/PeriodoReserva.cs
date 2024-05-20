using Orcamento.Models.Enums;
using System;

namespace Orcamento.Models.Entities
{
    public class PeriodoReserva
    {
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public List<DiasReservados> Periodo { get; set; }
        public int QtdTotalDeDias { get; set; }
        public int QtdDiasDeSemana { get; set; }
        public int QtdFinaisDeSemana { get; set; }
        public int QtdFeriados { get; set; }
        public List<DateTime> DiasDeSemana { get; set; }
        public List<DateTime> DiasDeFinalDeSemana { get; set; }
        public List<DateTime> DiasDeFeriado { get; set; }
        public HorarioCheckin PeriodoCheckin { get; set; }
        public HorarioCheckout PeriodoCheckout { get; set; }
    }
}
