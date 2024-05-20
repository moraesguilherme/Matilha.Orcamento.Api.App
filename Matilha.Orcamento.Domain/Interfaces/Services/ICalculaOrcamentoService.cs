using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;

namespace Matilha.Orcamento.Domain.Interfaces.Services
{
    public interface ICalculaOrcamentoService
    {
        Task<OrcamentoCalculado> ObtemOrcamentoCalculadoAsync(OrcamentoEntrada orcamento, PeriodoReserva periodoHospedagem);
        Task<int> ObtemQtdCaes(OrcamentoEntrada orcamento);
        Task<decimal> ObtemValorTotalDiasDeSemanaAsync(PeriodoReserva periodoReserva, List<PrecoPorTemporada> precosPorTemporada, List<AltaTemporadaDto> datasAltaTemporada);
        Task<decimal> ObtemValorTotalFinaisDeSemanaAsync(PeriodoReserva periodoReserva, List<PrecoPorTemporada> precosPorTemporada, List<AltaTemporadaDto> datasAltaTemporada);
        Task<decimal> ObtemValorTotalFeriadosAsync(PeriodoReserva periodoReserva, List<PrecoPorTemporada> precosPorTemporada, List<AltaTemporadaDto> datasAltaTemporada);
    }
}
