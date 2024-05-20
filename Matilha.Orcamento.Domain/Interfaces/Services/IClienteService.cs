using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;
using System.Threading.Tasks;

namespace Matilha.Orcamento.Domain.Interfaces.Services
{
    public interface IClienteService
    {
        Task<List<ClienteOrcamento>> BuscarClientesPorTelefoneAsync(string telefone);
        Task<int> InsereClienteOrcamentoAsync(OrcamentoEntrada orcamentoEntrada);
        Task<int> InsereNovoClienteAsync(OrcamentoEntrada orcamentoEntrada);
    }
}
