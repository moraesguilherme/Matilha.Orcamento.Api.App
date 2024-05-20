using Orcamento.Models.Entities;

namespace Matilha.Orcamento.Domain.Interfaces.Services
{
    public interface IPeriodoService
    {
        Task<PeriodoReserva> ObtemPeriodoAsync(DateTime dataCheckin, TimeSpan HorarioCheckin, DateTime DataCheckout, TimeSpan HorarioCheckout);
    }
}
