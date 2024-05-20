using Orcamento.Models.Dtos;

namespace Orcamento.Infraestructure.Repositories.Interfaces
{
    public interface ITemporadaRepository
    {
        Task InserirAltaTemporadaAsync(DateTime data, int mes, int ano, string descricao);
        Task<List<AltaTemporadaDto>> ObterDatasAltaTemporadaAsync(DateTime dataEntrada, DateTime dataSaida);
        Task<List<AltaTemporadaDto>> ObterDatasAltaTemporadaAsync();
        Task ExcluirDataAltaTemporadaAsync(int id);
    }
}
