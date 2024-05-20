using Matilha.Orcamento.Domain.Interfaces.Services;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Matilha.Orcamento.Domain.Services
{
    public class OrcamentoService : IOrcamentoService
    {
        private readonly IPeriodoService _periodoService;
        private readonly IClienteService _clienteService;
        private readonly ICachorroService _cachorroService;
        private readonly ICalculaOrcamentoService _calculoOrcamentoService;
        private readonly IClienteRepository _clienteRepository;
        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly ICachorroRepository _cachorroRepository;
        private readonly IPeriodoRepository _periodoRepository;
        private readonly ITemporadaRepository _temporadaRepository;

        public OrcamentoService(
            IPeriodoService periodoService,
            IClienteService clienteService,
            ICachorroService cachorroService,
            ICalculaOrcamentoService calculaOrcamentoService,
            IClienteRepository clienteRepository,
            IOrcamentoRepository orcamentoRepository,
            ICachorroRepository cachorroRepository,
            IPeriodoRepository periodoRepository,
            ITemporadaRepository temporadaRepository)
        {
            _periodoService = periodoService;
            _clienteService = clienteService;
            _cachorroService = cachorroService;
            _calculoOrcamentoService = calculaOrcamentoService;
            _clienteRepository = clienteRepository;
            _orcamentoRepository = orcamentoRepository;
            _cachorroRepository = cachorroRepository;
            _periodoRepository = periodoRepository;
            _temporadaRepository = temporadaRepository;
        }

        public async Task<OrcamentoDto> CriaOrcamentoAsync(OrcamentoEntrada orcamentoEntrada)
        {
            string numeroProposta = await GerarNumeroPropostaAsync();

            int clienteId = await _clienteService.InsereNovoClienteAsync(orcamentoEntrada);

            var periodoReserva = await _periodoService.ObtemPeriodoAsync(orcamentoEntrada.DataEntrada, orcamentoEntrada.HorarioEntrada, orcamentoEntrada.DataSaida, orcamentoEntrada.HorarioSaida);

            var orcamentoCalculado = await _calculoOrcamentoService.ObtemOrcamentoCalculadoAsync(orcamentoEntrada, periodoReserva);

            var orcamento = await MontaOrcamentoAsync(orcamentoEntrada, orcamentoCalculado, clienteId, numeroProposta, periodoReserva);

            await _cachorroService.InsereCaesOrcamentoAsync(orcamentoEntrada, clienteId);

            var orcamentoId = await _orcamentoRepository.SalvaOrcamentoAsync(orcamento);

            await InsereRelacionamentoAsync(orcamento, orcamentoId);

            return orcamento;
        }

        private async Task InsereRelacionamentoAsync(OrcamentoDto orcamento, int orcamentoId)
        {
            orcamento.Id = orcamentoId;

            foreach (var cao in orcamento.Cliente.CaoOrcamento)
            {
                await _orcamentoRepository.SalvaRelacionamentoOrcamentoAsync(orcamento, cao);
            }
        }

        private async Task<string> GerarNumeroPropostaAsync()
        {
            string ultimaPropostaSalva = await _orcamentoRepository.ObtemUltimaPropostaAsync();

            int proximoNumero = string.IsNullOrEmpty(ultimaPropostaSalva) ? 1 :
                                int.TryParse(ultimaPropostaSalva.Split('-')[1], out int numero) ? numero + 1 : 1;

            string formatoMesAno = DateTime.Now.ToString("MMyyyy");
            string novaProposta = $"{formatoMesAno}-{proximoNumero}";

            return novaProposta;
        }

        public async Task<OrcamentoDto> MontaOrcamentoAsync(OrcamentoEntrada orcamentoEntrada, OrcamentoCalculado orcamentoCalculado, int clienteId, string numeroProposta, PeriodoReserva periodoReserva)
        {
            DateTime dataProposta = DateTime.Now;
            int qtdCaes = await _calculoOrcamentoService.ObtemQtdCaes(orcamentoEntrada);

            return new OrcamentoDto
            {
                Cliente = orcamentoEntrada.Cliente,
                NumeroProposta = numeroProposta,
                StatusProposta = StatusProposta.Analise,
                DataProposta = dataProposta,
                DataExpiracao = orcamentoEntrada.DataExpiracao,
                QtdDeCaes = qtdCaes,
                //TemporadaId = orcamentoEntrada.Temporada,
                DataEntrada = orcamentoEntrada.DataEntrada,
                HorarioEntrada = orcamentoEntrada.HorarioEntrada,
                DataSaida = orcamentoEntrada.DataSaida,
                HorarioSaida = orcamentoEntrada.HorarioSaida,
                PeriodoReserva = periodoReserva,
                Observacoes = orcamentoEntrada.Observacoes,
                ValorHospedageUnitarioPorCao = orcamentoCalculado.TotalUnitarioPorCaes,
                Subtotal = orcamentoCalculado.Subtotal,
                QtdAvaliacao = orcamentoEntrada.QtdAvaliacoes,
                ValorUnitarioAvaliacao = orcamentoEntrada.ValorAvaliacao,
                ValorTotalAvaliacao = orcamentoCalculado.ValorTotalAvaliacao,
                QtdAdaptacao = orcamentoEntrada.QtdAdaptacao,
                ValorUnitarioAdaptacao = orcamentoEntrada.ValorAdaptacao,
                ValorTotalAdaptacao = orcamentoCalculado.ValorTotalAdaptacao,
                UtilizaTaxi = orcamentoEntrada.UtilizaTaxi,
                QtdTaxi = orcamentoEntrada.QtdTaxi,
                ValorViagemTaxi = orcamentoEntrada.ValorTaxi,
                ValorTotalTaxi = orcamentoCalculado.ValorTotalTaxi,
                Desconto = orcamentoEntrada.Desconto,
                ValorFinal = orcamentoCalculado.ValorTotalGeral,
                ClienteOrcamentoId = clienteId,
                ValorTotalDescontos = orcamentoCalculado.ValorTotalDeDescontos,
                TipoDesconto = orcamentoEntrada.TipoDesconto,
                DataAtualizacao = DateTime.UtcNow
            };
        }

        public async Task<OrcamentoDto> ConsultaOrcamentoAsync(string numeroProposta)
        {
            var orcamento = await _orcamentoRepository.ConsultaOrcamentoPorPropostaAsync(numeroProposta);

            await VerificaVencimentoDaProposta(orcamento);

            var periodoReserva = await _periodoService.ObtemPeriodoAsync(orcamento.DataEntrada, orcamento.HorarioEntrada, orcamento.DataSaida, orcamento.HorarioSaida);
            orcamento.PeriodoReserva = periodoReserva;

            var clientes = await _clienteRepository.ConsultarClienteOrcamentoAsync(orcamento.Id);
            orcamento.Cliente = clientes;

            var caes = await _cachorroRepository.ConsultarCaoOrcamentoAsync(orcamento.Id);
            orcamento.Cliente.CaoOrcamento = caes;

            await ConsultaValoresAsync(periodoReserva);

            return orcamento;
        }

        private async Task ConsultaValoresAsync(PeriodoReserva periodoReserva)
        {
            List<AltaTemporadaDto> datasAltaTemporada = await _temporadaRepository.ObterDatasAltaTemporadaAsync(periodoReserva.Checkin.Date, periodoReserva.Checkout.Date);

            var precosPorTemporada = await _periodoRepository.ObtemPrecoPorTemporada();

            await _calculoOrcamentoService.ObtemValorTotalDiasDeSemanaAsync(periodoReserva, precosPorTemporada, datasAltaTemporada);

            await _calculoOrcamentoService.ObtemValorTotalFinaisDeSemanaAsync(periodoReserva, precosPorTemporada, datasAltaTemporada);

            await _calculoOrcamentoService.ObtemValorTotalFeriadosAsync(periodoReserva, precosPorTemporada, datasAltaTemporada);
        }

        private async Task VerificaVencimentoDaProposta(OrcamentoDto orcamento)
        {
            if (orcamento.DataExpiracao < DateTime.Today || orcamento.StatusProposta == StatusProposta.Analise)
            {
                await AtualizarStatusOrcamentoAsync(orcamento.Id, StatusProposta.Expirada);
                orcamento.StatusProposta = StatusProposta.Expirada;
            }
        }

        public async Task<OrcamentoDto> AtualizarOrcamentoAsync(int orcamentoId, OrcamentoEntrada orcamento)
        {
            var periodoReserva = await _periodoService.ObtemPeriodoAsync(orcamento.DataEntrada, orcamento.HorarioEntrada, orcamento.DataSaida, orcamento.HorarioSaida);

            var orcamentoCalculado = await _calculoOrcamentoService.ObtemOrcamentoCalculadoAsync(orcamento, periodoReserva);

            var orcamentoAtualizado = await MontaOrcamentoAtualizadoAsync(orcamentoId, orcamento, orcamentoCalculado, periodoReserva);

            await _cachorroService.AtualizaCaesOrcamentoAsync(orcamento);

            await AtualizaRelacionamentoOrcamentoAsync(orcamentoAtualizado);

            await _orcamentoRepository.AtualizaOrcamentoAsync(orcamentoAtualizado);

            return orcamentoAtualizado;
        }

        public async Task<OrcamentoDto> MontaOrcamentoAtualizadoAsync(int orcamentoId, OrcamentoEntrada orcamento, OrcamentoCalculado orcamentoCalculado, PeriodoReserva periodoReserva)
        {
            var propostaExistente = await _orcamentoRepository.ConsultaOrcamentoPorIdAsync(orcamentoId);
            int qtdCaes = await _calculoOrcamentoService.ObtemQtdCaes(orcamento);

            return new OrcamentoDto
            {
                Id = orcamentoId,
                Cliente = orcamento.Cliente,
                NumeroProposta = propostaExistente.NumeroProposta,
                StatusProposta = propostaExistente.StatusProposta,
                DataProposta = propostaExistente.DataProposta,
                DataExpiracao = orcamento.DataExpiracao,
                QtdDeCaes = qtdCaes,
                DataEntrada = orcamento.DataEntrada,
                HorarioEntrada = orcamento.HorarioEntrada,
                DataSaida = orcamento.DataSaida,
                HorarioSaida = orcamento.HorarioSaida,
                PeriodoReserva = periodoReserva,
                Observacoes = orcamento.Observacoes,
                ValorHospedageUnitarioPorCao = orcamentoCalculado.TotalUnitarioPorCaes,
                Subtotal = orcamentoCalculado.Subtotal,
                QtdAvaliacao = orcamento.QtdAvaliacoes,
                ValorUnitarioAvaliacao = orcamento.ValorAvaliacao,
                ValorTotalAvaliacao = orcamentoCalculado.ValorTotalAvaliacao,
                QtdAdaptacao = orcamento.QtdAdaptacao,
                ValorUnitarioAdaptacao = orcamento.ValorAdaptacao,
                ValorTotalAdaptacao = orcamentoCalculado.ValorTotalAdaptacao,
                UtilizaTaxi = orcamento.UtilizaTaxi,
                QtdTaxi = orcamento.QtdTaxi,
                ValorViagemTaxi = orcamento.ValorTaxi,
                ValorTotalTaxi = orcamentoCalculado.ValorTotalTaxi,
                Desconto = orcamento.Desconto,
                ValorFinal = orcamentoCalculado.ValorTotalGeral,
                ValorTotalDescontos = orcamentoCalculado.ValorTotalDeDescontos,
                TipoDesconto = orcamento.TipoDesconto,
                DataAtualizacao = DateTime.UtcNow
            };
        }

        public async Task AtualizaRelacionamentoOrcamentoAsync(OrcamentoDto orcamento)
        {
            foreach (var cao in orcamento.Cliente.CaoOrcamento)
            {
                bool relacaoExistente = await _clienteRepository.BuscaRelacaoAsync(orcamento.Cliente.Id, cao.Id, orcamento.Id);

                if (relacaoExistente)
                {
                    await _orcamentoRepository.AtualizaRelacionamentoOrcamentoAsync(orcamento);
                }
                else
                {
                    await _orcamentoRepository.SalvaRelacionamentoOrcamentoAsync(orcamento, cao);
                }
            }
        }

        public async Task AtualizarStatusOrcamentoAsync(int orcamentoId, StatusProposta novoStatus)
        {
            await _orcamentoRepository.AtualizarStatusOrcamentoAsync(orcamentoId, (int)novoStatus);
        }

        public async Task<int> InsertPrecoTemporadaAsync(PrecoTemporada precoTemporada)
        {
            return await _orcamentoRepository.InsertPrecoTemporadaAsync(precoTemporada);
        }
    }
}
