using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;

namespace Matilha.Orcamento.Domain.Interfaces.Services
{
    public interface ITemporadaService
    {
        Task InserirAltaTemporadaAsync(AltaTemporada altaTemporada);
        Task<PrecoPorTemporada> GetPrecoPorTemporadaAsync(DateTime diaReservado, List<PrecoPorTemporada> precosPorTemporada, List<AltaTemporadaDto> datasAltaTemporada);
        Task<List<AltaTemporadaDto>> ObterDatasAltaTemporadaAsync();
        Task ExcluirDataAltaTemporadaAsync(int id);
    }
}
