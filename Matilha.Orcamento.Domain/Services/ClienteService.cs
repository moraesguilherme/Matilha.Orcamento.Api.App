using Matilha.Orcamento.Domain.Interfaces.Services;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Entities;

namespace Matilha.Orcamento.Domain.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly ICachorroRepository _caesRepository;

        public ClienteService(IClienteRepository clienteRepository, IOrcamentoRepository orcamentoRepository, ICachorroRepository caesRepository)
        {
            _clienteRepository = clienteRepository;
            _orcamentoRepository = orcamentoRepository;
            _caesRepository = caesRepository;
        }
        public async Task<List<ClienteOrcamento>> BuscarClientesPorTelefoneAsync(string telefone)
        {
            string telefoneLimpo = new string(telefone.Where(char.IsDigit).ToArray());

            var clientes = await _clienteRepository.BuscarClientePorTelefoneAsync(telefoneLimpo);

            if (clientes != null && clientes.Any())
            {
                foreach (var cliente in clientes)
                {
                    cliente.CaoOrcamento = await _caesRepository.BuscarCaesAsync(cliente.Id);
                }
            }

            return clientes;
        }

        public async Task<int> InsereClienteOrcamentoAsync(OrcamentoEntrada orcamentoEntrada)
        {
            int clienteId = await _clienteRepository.InsereClienteOrcamentoAsync(orcamentoEntrada);
            orcamentoEntrada.Cliente.Id = clienteId;
            return clienteId;
        }

        public async Task<int> InsereNovoClienteAsync(OrcamentoEntrada orcamentoEntrada)
        {
            return orcamentoEntrada.Cliente.Id != 0 ? 0 : await Task.Run(() => InsereClienteOrcamentoAsync(orcamentoEntrada));
        }

    }
}
