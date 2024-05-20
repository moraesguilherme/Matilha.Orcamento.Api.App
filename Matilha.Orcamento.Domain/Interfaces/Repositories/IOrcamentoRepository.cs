using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Orcamento.Infraestructure.Repositories.Interfaces
{
    public interface IOrcamentoRepository
    {
        Task<string> ObtemUltimaPropostaAsync();
        Task<int> SalvaOrcamentoAsync(OrcamentoDto orcamentoDto);
        Task SalvaRelacionamentoOrcamentoAsync(OrcamentoDto orcamentoDto, CaoOrcamento caoOrcamento);
        Task<OrcamentoDto> ConsultaOrcamentoPorPropostaAsync(string numeroPorposta);
        Task AtualizaOrcamentoAsync(OrcamentoDto orcamentoDto);
        Task AtualizaRelacionamentoOrcamentoAsync(OrcamentoDto orcamento);
        Task<OrcamentoDto> ConsultaOrcamentoPorIdAsync(int orcamentoId);
        Task AtualizarStatusOrcamentoAsync(int orcamentoId, int novoStatus);
        Task<int> InsertPrecoTemporadaAsync(PrecoTemporada precoTemporada);
    }
}
