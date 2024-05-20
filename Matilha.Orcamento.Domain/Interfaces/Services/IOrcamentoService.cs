using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Matilha.Orcamento.Domain.Interfaces.Services
{
    public interface IOrcamentoService
    {
        Task<OrcamentoDto> CriaOrcamentoAsync(OrcamentoEntrada orcamentoEntrada);
        Task<OrcamentoDto> ConsultaOrcamentoAsync(string numeroProposta);
        Task<OrcamentoDto> AtualizarOrcamentoAsync(int orcamentoId, OrcamentoEntrada orcamentoEntrada);
        Task AtualizarStatusOrcamentoAsync(int orcamentoId, StatusProposta novoStatus);
        Task<int> InsertPrecoTemporadaAsync(PrecoTemporada precoTemporada);

    }
}
