using Matilha.Orcamento.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Orcamento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoService _orcamentoService;

        public OrcamentoController(IOrcamentoService orcamentoService)
        {
            _orcamentoService = orcamentoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarOrcamento([FromBody] OrcamentoEntrada orcamento)
        {
            var orcamentoDtoResult = await _orcamentoService.CriaOrcamentoAsync(orcamento);
            return Ok(orcamentoDtoResult);
        }

        [HttpGet("{numeroProposta}")]
        public async Task<IActionResult> BuscarOrcamentoPorNumeroProposta(string numeroProposta)
        {
            var orcamentoDto = await _orcamentoService.ConsultaOrcamentoAsync(numeroProposta);
            if (orcamentoDto == null)
            {
                return NotFound();
            }

            return Ok(orcamentoDto);
        }

        [HttpPut("{orcamentoId}")]
        public async Task<IActionResult> AtualizaOrcamentoAsync(int orcamentoId, [FromBody] OrcamentoEntrada orcamento)
        {
            var orcamentoAtualizado = await _orcamentoService.AtualizarOrcamentoAsync(orcamentoId, orcamento);
            return Ok(orcamentoAtualizado);
        }

        [HttpPut("{orcamentoId}/status")]
        public async Task<IActionResult> AtualizaStatusOrcamentoAsync(int orcamentoId, [FromBody] StatusProposta novoStatus)
        {
            await _orcamentoService.AtualizarStatusOrcamentoAsync(orcamentoId, novoStatus);
            return Ok();
        }

        [HttpPost("precos")]
        public async Task<IActionResult> InsertPrecoTemporada([FromBody] PrecoTemporada precoTemporada)
        {
            var precoTemporadaId = await _orcamentoService.InsertPrecoTemporadaAsync(precoTemporada);

            return Ok(new { Id = precoTemporadaId });
        }
    }
}
