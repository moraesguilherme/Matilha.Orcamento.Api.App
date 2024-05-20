using Matilha.Orcamento.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Orcamento.Models.Entities;

namespace Orcamento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemporadaController : ControllerBase
    {
        private readonly ITemporadaService _temporadaService;

        public TemporadaController(ITemporadaService temporadaService)
        {
            _temporadaService = temporadaService;
        }

        [HttpPost("AltaTemporada")]
        public async Task<IActionResult> CadastrarAltaTemporadaAsync([FromBody] AltaTemporada altaTemporada)
        {
            await _temporadaService.InserirAltaTemporadaAsync(altaTemporada);
            return Ok("Alta temporada cadastrada com sucesso.");
        }

        [HttpGet("datas-alta-temporada")]
        public async Task<IActionResult> ObterTodasAsDatasAltaTemporada()
        {
            var datasAltaTemporada = await _temporadaService.ObterDatasAltaTemporadaAsync();
            return Ok(datasAltaTemporada);
        }

        [HttpDelete("datas-alta-temporada/{id}")]
        public async Task<IActionResult> ExcluirDataAltaTemporada(int id)
        {
            await _temporadaService.ExcluirDataAltaTemporadaAsync(id);
            return NoContent();
        }
    }
}
