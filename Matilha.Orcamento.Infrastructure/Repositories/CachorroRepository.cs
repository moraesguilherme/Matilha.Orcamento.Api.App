using Dapper;
using Orcamento.Infraestructure.Data;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Entities;
using System.Data;

namespace Matilha.Orcamento.Domain.Repositories
{
    public class CachorroRepository : ICachorroRepository
    {
        private readonly AppDbContext _dbContext;

        public CachorroRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<List<CaoOrcamento>> BuscarCaesAsync(int clienteId)
        {
            string query = "SELECT Id, NomeCachorro, Raca, Idade, Castrado, Sexo, Peso, Ativo, Deletado FROM CachorroOrcamento WHERE ClienteOrcamentoId = @ClienteId";
            return (await _dbContext.Connection.QueryAsync<CaoOrcamento>(query, new { ClienteId = clienteId })).AsList();
        }

        public async Task<int> InsereCaesOrcamentoAsync(CaoOrcamento caoOrcamento, int clientId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ClienteOrcamentoId", clientId);
            parameters.Add("@NomeCachorro", caoOrcamento.NomeCachorro);
            parameters.Add("@Raca", caoOrcamento.Raca);
            parameters.Add("@Idade", caoOrcamento.Idade);
            parameters.Add("@Castrado", caoOrcamento.Castrado);
            parameters.Add("@Sexo", caoOrcamento.Sexo);
            parameters.Add("@Peso", caoOrcamento.Peso);
            parameters.Add("@Ativo", caoOrcamento.Ativo);
            parameters.Add("@Deletado", caoOrcamento.Deletado);

            parameters.Add("@InsertedId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbContext.Connection.ExecuteAsync("InsertCachorroOrcamento", parameters, commandType: CommandType.StoredProcedure);

            int insertedId = parameters.Get<int>("@InsertedId");

            return insertedId;
        }

        public async Task<List<CaoOrcamento>> ConsultarCaoOrcamentoAsync(int orcamentoId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrcamentoId", orcamentoId);

            var result = await _dbContext.Connection.QueryAsync<CaoOrcamento>(
                sql: "ConsultarCaoOrcamento",
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task AtualizaCaesOrcamentoAsync(CaoOrcamento caoOrcamento)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", caoOrcamento.Id);
            parameters.Add("@Idade", caoOrcamento.Idade);
            parameters.Add("@Castrado", caoOrcamento.Castrado);
            parameters.Add("@Peso", caoOrcamento.Peso);
            parameters.Add("@Ativo", caoOrcamento.Ativo);
            parameters.Add("@Deletado", caoOrcamento.Deletado);
            // colocar data atualizacao
            await _dbContext.Connection.ExecuteAsync("UpdateCachorroOrcamento", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
