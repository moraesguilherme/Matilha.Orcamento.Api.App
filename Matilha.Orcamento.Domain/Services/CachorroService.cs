using Matilha.Orcamento.Domain.Interfaces.Services;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Entities;

namespace Matilha.Orcamento.Domain.Services
{
    public class CachorroService : ICachorroService
    {
        private readonly ICachorroRepository _caesRepository;

        public CachorroService(ICachorroRepository caesRepository)
        {
            _caesRepository = caesRepository;
        }
        public async Task<List<CaoOrcamento>> InsereCaesOrcamentoAsync(OrcamentoEntrada orcamentoEntrada, int clienteId)
        {
            List<CaoOrcamento> caoCadastrado = await _caesRepository.BuscarCaesAsync(clienteId);
            List<CaoOrcamento> novoCao = new List<CaoOrcamento>();

            foreach (var cao in orcamentoEntrada.Cliente.CaoOrcamento)
            {
                if (!caoCadastrado.Any(c => c.Id == cao.Id))
                {
                    int insertedId = await _caesRepository.InsereCaesOrcamentoAsync(cao, clienteId);
                    cao.Id = insertedId;
                    novoCao.Add(cao);
                }
            }

            return novoCao;
        }

        public async Task<List<CaoOrcamento>> AtualizaCaesOrcamentoAsync(OrcamentoEntrada orcamento)
        {
            List<CaoOrcamento> caoCadastrado = await _caesRepository.BuscarCaesAsync(orcamento.Cliente.Id);
            List<CaoOrcamento> novoCao = new List<CaoOrcamento>();

            foreach (var cao in orcamento.Cliente.CaoOrcamento)
            {
                var caoExistente = caoCadastrado.FirstOrDefault(c => c.Id == cao.Id);

                if (caoExistente != null)
                {
                    await _caesRepository.AtualizaCaesOrcamentoAsync(cao);
                    novoCao.Add(cao);
                }
                else
                {
                    int insertedId = await _caesRepository.InsereCaesOrcamentoAsync(cao, orcamento.Cliente.Id);
                    cao.Id = insertedId;
                    novoCao.Add(cao);
                }
            }

            return novoCao;
        }
    }
}
