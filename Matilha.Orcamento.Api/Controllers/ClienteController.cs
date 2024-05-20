using Microsoft.AspNetCore.Mvc;
using Matilha.Orcamento.Domain.Interfaces.Services;

namespace Orcamento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarClientes([FromQuery] string telefone)
        {
            var clientes = await _clienteService.BuscarClientesPorTelefoneAsync(telefone);
            return Ok(clientes);
        }
    }
}
