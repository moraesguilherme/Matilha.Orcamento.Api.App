using Orcamento.Models.Entities;

namespace Matilha.Orcamento.Domain.Interfaces.Services
{
    public interface ICachorroService
    {
        Task<List<CaoOrcamento>> InsereCaesOrcamentoAsync(OrcamentoEntrada orcamentoEntrada, int clienteId);
        Task<List<CaoOrcamento>> AtualizaCaesOrcamentoAsync(OrcamentoEntrada orcamentoEntrada);
    }
}
