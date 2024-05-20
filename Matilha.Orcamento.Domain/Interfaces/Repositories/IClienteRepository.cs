using Orcamento.Models.Entities;

namespace Orcamento.Infraestructure.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        Task<List<ClienteOrcamento>> BuscarClientePorTelefoneAsync(string telefone);
        Task<int> InsereClienteOrcamentoAsync(OrcamentoEntrada orcamentoEntrada);
        Task<ClienteOrcamento> ConsultarClienteOrcamentoAsync(int orcamentoId);
        Task<bool> BuscaRelacaoAsync(int clienteOrcamentoId, int cachorroId, int orcamentoId);
    }
}
