using Dapper;
using Orcamento.Infraestructure.Data;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Entities;
using System.Data;

namespace Matilha.Orcamento.Domain.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _dbContext;

        public ClienteRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<List<ClienteOrcamento>> BuscarClientePorTelefoneAsync(string telefone)
        {
            string query = "SELECT * FROM ClienteOrcamento WHERE REPLACE(REPLACE(REPLACE(TelefoneCliente, '-', ''), '.', ''), ' ', '') = @Telefone";
            return (await _dbContext.Connection.QueryAsync<ClienteOrcamento>(query, new { Telefone = telefone })).AsList();
        }

        public async Task<bool> BuscaRelacaoAsync(int clienteOrcamentoId, int cachorroId, int orcamentoId)
        {
            string query = @"SELECT COUNT(*) 
                     FROM OrcamentoClienteECachorro 
                     WHERE ClienteOrcamentoId = @ClienteOrcamentoId 
                     AND CachorroId = @CachorroId 
                     AND OrcamentoId = @OrcamentoId";

            var parameters = new { ClienteOrcamentoId = clienteOrcamentoId, CachorroId = cachorroId, OrcamentoId = orcamentoId };

            int count = await _dbContext.Connection.ExecuteScalarAsync<int>(query, parameters);

            return count > 0;
        }

        public async Task<int> InsereClienteOrcamentoAsync(OrcamentoEntrada orcamentoEntrada)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NomeCliente", orcamentoEntrada.Cliente.NomeCliente);
            parameters.Add("@Email", orcamentoEntrada.Cliente.Email);
            parameters.Add("@TelefoneCliente", orcamentoEntrada.Cliente.TelefoneCliente);
            parameters.Add("@ClienteId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbContext.Connection.ExecuteAsync("InsertClienteOrcamento", parameters, commandType: CommandType.StoredProcedure);

            int clienteOrcamentoId = parameters.Get<int>("@ClienteId");

            return clienteOrcamentoId;
        }

        public async Task<ClienteOrcamento> ConsultarClienteOrcamentoAsync(int orcamentoId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrcamentoId", orcamentoId);

            var result = await _dbContext.Connection.QuerySingleOrDefaultAsync<ClienteOrcamento>(
                sql: "ConsultarClienteOrcamento",
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}
