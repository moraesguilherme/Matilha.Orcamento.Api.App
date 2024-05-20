using Dapper;
using Orcamento.Infraestructure.Data;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Dtos;
using Orcamento.Models.Enums;
using System.Data;

namespace Matilha.Orcamento.Domain.Repositories
{
    public class TemporadaRepository : ITemporadaRepository
    {
        private readonly AppDbContext _dbContext;

        public TemporadaRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InserirAltaTemporadaAsync(DateTime data, int mes, int ano, string descricao)
        {
            await _dbContext.Connection.ExecuteAsync("InsertAltaTemporada", new
            {
                TemporadaId = TemporadaId.Alta,
                Data = data,
                Mes = mes,
                Ano = ano,
                Descricao = descricao
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<AltaTemporadaDto>> ObterDatasAltaTemporadaAsync(DateTime dataEntrada, DateTime dataSaida)
        {
            string query = "SELECT * FROM AltaTemporada WHERE Data BETWEEN @DataInicio AND @DataFim";
            return (await _dbContext.Connection.QueryAsync<AltaTemporadaDto>(query, new { DataInicio = dataEntrada, DataFim = dataSaida })).AsList();
        }

        public async Task<List<AltaTemporadaDto>> ObterDatasAltaTemporadaAsync()
        {
            string query = "SELECT * FROM AltaTemporada";
            return (await _dbContext.Connection.QueryAsync<AltaTemporadaDto>(query)).AsList();
        }

        public async Task ExcluirDataAltaTemporadaAsync(int id)
        {
            string query = "DELETE FROM AltaTemporada WHERE Id = @Id";
            await _dbContext.Connection.ExecuteAsync(query, new { Id = id });
        }


    }
}
