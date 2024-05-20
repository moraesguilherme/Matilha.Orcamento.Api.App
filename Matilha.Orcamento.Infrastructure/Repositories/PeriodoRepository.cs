using Dapper;
using Orcamento.Infraestructure.Data;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Entities;
using Orcamento.Models.Enums;
using System.Data;

namespace Matilha.Orcamento.Domain.Repositories
{
    public class PeriodoRepository : IPeriodoRepository
    {
        private readonly AppDbContext _dbContext;

        public PeriodoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SalvaFeriadosNacionais(List<Feriado> feriados, int ano)
        {
            foreach (var feriado in feriados)
            {
                await _dbContext.Connection.ExecuteAsync("InsertFeriadoNacional", new
                {
                    Data = feriado.Date,
                    Nome = feriado.Name,
                    Tipo = feriado.Type,
                    Nivel = feriado.Level,
                    Ano = ano
                }, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<List<DateTime>> ObtemFeriadosDoAnoAsync(DateTime dataEntrada, DateTime dataSaida)
        {
            string query = "SELECT Data, Ano FROM FeriadosNacionais WHERE Ano IN (@AnoInicio, @AnoFim)";
            return (await _dbContext.Connection.QueryAsync<DateTime>(query, new { AnoInicio = dataEntrada.Year, AnoFim = dataSaida.Year })).ToList();
        }

        public async Task<List<PrecoPorTemporada>> ObtemPrecoPorTemporada()
        {
            string query =
                "SELECT TipoTemporada, " +
                "   ValorDiaDeSemana, " +
                "   ValorFinalDeSemana, " +
                "   ValorFeriado, " +
                "   ValorMeioPeriodoSemana,  " +
                "   ValorPernoite, " +
                "   ValorMeioPeriodoFimDeSemana, " +
                "   TemporadaId " +
                "FROM PrecoTemporada";
            return (await _dbContext.Connection.QueryAsync<PrecoPorTemporada>(query)).AsList();
        }
    }
}
