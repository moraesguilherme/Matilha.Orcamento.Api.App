using Orcamento.Models.Entities;

namespace Orcamento.Infraestructure.Repositories.Interfaces
{
    public interface ICachorroRepository
    {
        Task<List<CaoOrcamento>> BuscarCaesAsync(int clienteId);
        Task<int> InsereCaesOrcamentoAsync(CaoOrcamento caoOrcamento, int clientId);
        Task<List<CaoOrcamento>> ConsultarCaoOrcamentoAsync(int orcamentoId);
        Task AtualizaCaesOrcamentoAsync(CaoOrcamento caoOrcamento);
    }
}
