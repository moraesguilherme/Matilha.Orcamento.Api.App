using Dapper;
using Orcamento.Infraestructure.Data;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;
using System.Data;

namespace Matilha.Orcamento.Domain.Repositories
{
    public class OrcamentoRepository : IOrcamentoRepository
    {
        private readonly AppDbContext _dbContext;

        public OrcamentoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SalvaOrcamentoAsync(OrcamentoDto orcamentoDto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NumeroProposta", orcamentoDto.NumeroProposta);
            parameters.Add("@StatusProposta", (int)orcamentoDto.StatusProposta);
            parameters.Add("@DataProposta", orcamentoDto.DataProposta);
            parameters.Add("@DataExpiracao", orcamentoDto.DataExpiracao);
            parameters.Add("@QtdDeCaes", orcamentoDto.QtdDeCaes);
            parameters.Add("@DataEntrada", orcamentoDto.DataEntrada);
            parameters.Add("@HorarioEntrada", orcamentoDto.HorarioEntrada);
            parameters.Add("@DataSaida", orcamentoDto.DataSaida);
            parameters.Add("@HorarioSaida", orcamentoDto.HorarioSaida);
            parameters.Add("@Observacoes", orcamentoDto.Observacoes);
            parameters.Add("@ValorHospedageUnitarioPorCao", orcamentoDto.ValorHospedageUnitarioPorCao);
            parameters.Add("@Subtotal", orcamentoDto.Subtotal);
            parameters.Add("@QtdAvaliacao", orcamentoDto.QtdAvaliacao);
            parameters.Add("@ValorUnitarioAvaliacao", orcamentoDto.ValorUnitarioAvaliacao);
            parameters.Add("@ValorTotalAvaliacao", orcamentoDto.ValorTotalAvaliacao);
            parameters.Add("@QtdAdaptacao", orcamentoDto.QtdAdaptacao);
            parameters.Add("@ValorUnitarioAdaptacao", orcamentoDto.ValorUnitarioAdaptacao);
            parameters.Add("@ValorTotalAdaptacao", orcamentoDto.ValorTotalAdaptacao);
            parameters.Add("@UtilizaTaxi", orcamentoDto.UtilizaTaxi);
            parameters.Add("@QtdTaxi", orcamentoDto.QtdTaxi);
            parameters.Add("@ValorViagemTaxi", orcamentoDto.ValorViagemTaxi);
            parameters.Add("@ValorTotalTaxi", orcamentoDto.ValorTotalTaxi);
            parameters.Add("@Desconto", orcamentoDto.Desconto);
            parameters.Add("@ValorFinal", orcamentoDto.ValorFinal);
            parameters.Add("@ClienteOrcamentoId", orcamentoDto.ClienteOrcamentoId);
            parameters.Add("@ValorTotalDescontos", orcamentoDto.ValorTotalDescontos);
            parameters.Add("@TipoDesconto", orcamentoDto.TipoDesconto);
            parameters.Add("@OrcamentoId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbContext.Connection.ExecuteAsync("InsertOrcamento", parameters, commandType: CommandType.StoredProcedure);

            int idOrcamento = parameters.Get<int>("@OrcamentoId");

            return idOrcamento;
        }

        public async Task<string> ObtemUltimaPropostaAsync()
        {
            string query = "SELECT TOP 1 NumeroProposta FROM Orcamento_New ORDER BY Id DESC";

            return await _dbContext.Connection.ExecuteScalarAsync<string>(query);
        }

        public async Task SalvaRelacionamentoOrcamentoAsync(OrcamentoDto orcamentoDto, CaoOrcamento cao)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ClienteOrcamentoId", orcamentoDto.Cliente.Id);
            parameters.Add("@CachorroId", cao.Id);
            parameters.Add("@OrcamentoId", orcamentoDto.Id);
            parameters.Add("@DataOrcamento", orcamentoDto.DataProposta);
            parameters.Add("@NumeroProposta", orcamentoDto.NumeroProposta);
            parameters.Add("@Ativo", cao.Ativo);
            parameters.Add("@Deletado", cao.Deletado);
            parameters.Add("@DataAtualizacao", orcamentoDto.DataAtualizacao);

            await _dbContext.Connection.ExecuteAsync("InsertOrcamentoClienteECachorro", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<OrcamentoDto> ConsultaOrcamentoPorPropostaAsync(string numeroPorposta)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NumeroProposta", numeroPorposta);

            var result = await _dbContext.Connection.QueryAsync<OrcamentoDto>(
                sql: "ConsultarOrcamentoPorProposta",
                param: parameters
            );

            return result.FirstOrDefault();
        }

        public async Task<OrcamentoDto> ConsultaOrcamentoPorIdAsync(int orcamentoId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrcamentoId", orcamentoId);

            var result = await _dbContext.Connection.QueryAsync<OrcamentoDto>(
                sql: "ConsultarOrcamentoPorId",
                param: parameters
            );

            return result.FirstOrDefault();
        }

        public async Task AtualizaOrcamentoAsync(OrcamentoDto orcamentoDto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", orcamentoDto.Id);
            parameters.Add("@DataExpiracao", orcamentoDto.DataExpiracao);
            parameters.Add("@QtdDeCaes", orcamentoDto.QtdDeCaes);
            parameters.Add("@DataEntrada", orcamentoDto.DataEntrada);
            parameters.Add("@HorarioEntrada", orcamentoDto.HorarioEntrada);
            parameters.Add("@DataSaida", orcamentoDto.DataSaida);
            parameters.Add("@HorarioSaida", orcamentoDto.HorarioSaida);
            parameters.Add("@Observacoes", orcamentoDto.Observacoes);
            parameters.Add("@ValorHospedageUnitarioPorCao", orcamentoDto.ValorHospedageUnitarioPorCao);
            parameters.Add("@Subtotal", orcamentoDto.Subtotal);
            parameters.Add("@QtdAvaliacao", orcamentoDto.QtdAvaliacao);
            parameters.Add("@ValorUnitarioAvaliacao", orcamentoDto.ValorUnitarioAvaliacao);
            parameters.Add("@ValorTotalAvaliacao", orcamentoDto.ValorTotalAvaliacao);
            parameters.Add("@ValorUnitarioAdaptacao", orcamentoDto.ValorUnitarioAdaptacao);
            parameters.Add("@ValorTotalAdaptacao", orcamentoDto.ValorTotalAdaptacao);
            parameters.Add("@UtilizaTaxi", orcamentoDto.UtilizaTaxi);
            parameters.Add("@QtdTaxi", orcamentoDto.QtdTaxi);
            parameters.Add("@ValorViagemTaxi", orcamentoDto.ValorViagemTaxi);
            parameters.Add("@ValorTotalTaxi", orcamentoDto.ValorTotalTaxi);
            parameters.Add("@Desconto", orcamentoDto.Desconto);
            parameters.Add("@ValorFinal", orcamentoDto.ValorFinal);
            parameters.Add("@QtdAdaptacao", orcamentoDto.QtdAdaptacao);
            parameters.Add("@ValorTotalDescontos", orcamentoDto.ValorTotalDescontos);
            parameters.Add("@TipoDesconto", orcamentoDto.TipoDesconto);
            parameters.Add("@DataAtualizacao", orcamentoDto.DataAtualizacao);

            await _dbContext.Connection.ExecuteAsync("UpdateOrcamento", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task AtualizaRelacionamentoOrcamentoAsync(OrcamentoDto orcamento)
        {
            var parametersList = new List<DynamicParameters>();
            var cao = orcamento.Cliente.CaoOrcamento[0];

            var parameters = new DynamicParameters();
            parameters.Add("@ClienteOrcamentoId", orcamento.Cliente.Id);
            parameters.Add("@CachorroId", cao.Id);
            parameters.Add("@OrcamentoId", orcamento.Id);
            parameters.Add("@DataAtualizacao", orcamento.DataAtualizacao);
            parameters.Add("@Ativo", cao.Ativo);
            parameters.Add("@Deletado", cao.Deletado);

            parametersList.Add(parameters);

            await _dbContext.Connection.ExecuteAsync("UpdateOrcamentoClienteECachorro", parametersList, commandType: CommandType.StoredProcedure);
        }

        public async Task AtualizarStatusOrcamentoAsync(int orcamentoId, int novoStatus)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrcamentoId", orcamentoId);
            parameters.Add("@StatusProposta", novoStatus);

            await _dbContext.Connection.ExecuteAsync("UpdateStatusOrcamento", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertPrecoTemporadaAsync(PrecoTemporada precoTemporada)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TipoTemporada", precoTemporada.TipoTemporada);
            parameters.Add("@ValorDiaDeSemana", precoTemporada.ValorDiaDeSemana);
            parameters.Add("@ValorFinalDeSemana", precoTemporada.ValorFinalDeSemana);
            parameters.Add("@ValorFeriado", precoTemporada.ValorFeriado);
            parameters.Add("@ValorMeioPeriodoSemana", precoTemporada.ValorMeioPeriodoSemana);
            parameters.Add("@ValorPernoite", precoTemporada.ValorPernoite);
            parameters.Add("@ValorMeioPeriodoFimDeSemana", precoTemporada.ValorMeioPeriodoFimDeSemana);
            parameters.Add("@TemporadaId", precoTemporada.TemporadaId);

            var result = await _dbContext.Connection.QuerySingleAsync<int>(
                "InsertPrecoTemporada",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

    }
}
