using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Orcamento.Infraestructure.Repositories.Interfaces
{
    public interface IPeriodoRepository
    {
        Task<List<PrecoPorTemporada>> ObtemPrecoPorTemporada();
        Task SalvaFeriadosNacionais(List<Feriado> feriados, int ano);
        Task<List<DateTime>> ObtemFeriadosDoAnoAsync(DateTime dataEntrada, DateTime dataSaida);
    }
}
