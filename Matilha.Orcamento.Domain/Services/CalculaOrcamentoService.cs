using Matilha.Orcamento.Domain.Interfaces.Services;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Matilha.Orcamento.Domain.Services
{
    public class CalculaOrcamentoService : ICalculaOrcamentoService
    {
        private readonly IPeriodoRepository _periodoRepository;
        private readonly ITemporadaRepository _temporadaRepository;
        private readonly ITemporadaService _temporadaService;

        public CalculaOrcamentoService(IPeriodoRepository periodoRepository, ITemporadaRepository temporadaRepository, ITemporadaService temporadaService)
        {
            _periodoRepository = periodoRepository;
            _temporadaRepository = temporadaRepository;
            _temporadaService = temporadaService;
        }
        public async Task<OrcamentoCalculado> ObtemOrcamentoCalculadoAsync(OrcamentoEntrada orcamento, PeriodoReserva periodoReserva)
        {
            int qtdCaes = await ObtemQtdCaes(orcamento);
            List<AltaTemporadaDto> datasAltaTemporada = await _temporadaRepository.ObterDatasAltaTemporadaAsync(periodoReserva.Checkin.Date, periodoReserva.Checkout.Date);

            var precosPorTemporada = await _periodoRepository.ObtemPrecoPorTemporada();

            decimal valorTotalDiasSemana = await ObtemValorTotalDiasDeSemanaAsync(periodoReserva, precosPorTemporada, datasAltaTemporada);
            decimal valorTotalFinaisDeSemana = await ObtemValorTotalFinaisDeSemanaAsync(periodoReserva, precosPorTemporada, datasAltaTemporada);
            decimal valorTotalFeriados = await ObtemValorTotalFeriadosAsync(periodoReserva, precosPorTemporada, datasAltaTemporada);

            decimal valorTotalAvaliacao = await ObtemValorTotalAvaliacaoAsync(orcamento);
            decimal valorTotalAdaptacao = await ObtemValorTotalAdaptacaoAsync(orcamento);
            decimal valorTotalTaxi = await ObtemValorTaxiAsync(orcamento);
            decimal valorTotalServicosAdicionais = await ObtemValorServicosAdicionaisAsync(orcamento);

            decimal subtotal = await ObtemSubtotalAsync(qtdCaes, valorTotalDiasSemana, valorTotalFinaisDeSemana, valorTotalFeriados);
            decimal totalSemDescontos = await ObtemTotalSemDescontoAsync(valorTotalServicosAdicionais, subtotal);
            decimal totalGeral = await ObtemTotalGeralAsync(orcamento, valorTotalServicosAdicionais, subtotal);
            decimal totalDeDescontos = await ObtemTotalDeDescontoAsync(orcamento, subtotal);
            decimal totalUnitarioPorCaes = await ObtemTotalUnitaioAsync(qtdCaes, subtotal);

            return new OrcamentoCalculado
            {
                TotalDiasSemana = valorTotalDiasSemana,
                TotalFinaisDeSemana = valorTotalFinaisDeSemana,
                TotalFeriados = valorTotalFeriados,
                TotalServicosAdicionais = valorTotalServicosAdicionais,
                TotalUnitarioPorCaes = totalUnitarioPorCaes,
                Subtotal = subtotal,
                TotalAntesDescontos = totalSemDescontos,
                ValorTotalGeral = totalGeral,
                ValorTotalAvaliacao = valorTotalAvaliacao,
                ValorTotalAdaptacao = valorTotalAdaptacao,
                ValorTotalTaxi = valorTotalTaxi,
                ValorTotalDeDescontos = totalDeDescontos
            };
        }

        public async Task<int> ObtemQtdCaes(OrcamentoEntrada orcamento)
        {
            return orcamento.Cliente.CaoOrcamento
                      .Where(cao => cao.Ativo && !cao.Deletado)
                      .Count();
        }

        private async Task<decimal> ObtemTotalUnitaioAsync(int qtdCaes, decimal subtotal)
        {
            return subtotal / qtdCaes;
        }

        private async Task<decimal> ObtemTotalDeDescontoAsync(OrcamentoEntrada orcamento, decimal subtotal)
        {
            decimal totalDesconto = orcamento.TipoDesconto == TipoDesconto.Porcentagem ?
                                    subtotal * (orcamento.Desconto / 100) :
                                    orcamento.TipoDesconto == TipoDesconto.Reais ?
                                    orcamento.Desconto :
                                    0;

            return totalDesconto;
        }

        private async Task<decimal> ObtemTotalGeralAsync(OrcamentoEntrada orcamento, decimal valorTotalServicosAdicionais, decimal subtotal)
        {
            decimal totalDesconto = orcamento.TipoDesconto == TipoDesconto.Porcentagem ?
                                    subtotal - subtotal * (orcamento.Desconto / 100) + valorTotalServicosAdicionais :
                                    orcamento.TipoDesconto == TipoDesconto.Reais ?
                                    subtotal - orcamento.Desconto + valorTotalServicosAdicionais : 0;

            return totalDesconto;
        }

        private async Task<decimal> ObtemTotalSemDescontoAsync(decimal valorTotalServicosAdicionais, decimal subtotal)
        {
            return valorTotalServicosAdicionais + subtotal;
        }

        private async Task<decimal> ObtemSubtotalAsync(int qtdCaes, decimal valorTotalDiasSemana, decimal valorTotalFinaisDeSemana, decimal valorTotalFeriados)
        {
            return (valorTotalDiasSemana + valorTotalFinaisDeSemana + valorTotalFeriados) * qtdCaes;
        }

        private async Task<decimal> ObtemValorServicosAdicionaisAsync(OrcamentoEntrada orcamento)
        {
            return orcamento.QtdAvaliacoes * orcamento.ValorAvaliacao +
                                               orcamento.QtdAdaptacao * orcamento.ValorAdaptacao +
                                               (orcamento.UtilizaTaxi ? orcamento.QtdTaxi * orcamento.ValorTaxi : 0);
        }

        private async Task<decimal> ObtemValorTaxiAsync(OrcamentoEntrada orcamento)
        {
            return orcamento.QtdTaxi * orcamento.ValorTaxi;
        }

        private async Task<decimal> ObtemValorTotalAdaptacaoAsync(OrcamentoEntrada orcamento)
        {
            return orcamento.QtdAdaptacao * orcamento.ValorAdaptacao;
        }

        private async Task<decimal> ObtemValorTotalAvaliacaoAsync(OrcamentoEntrada orcamento)
        {
            return orcamento.QtdAvaliacoes * orcamento.ValorAvaliacao;
        }

        public async Task<decimal> ObtemValorTotalFeriadosAsync(PeriodoReserva periodoReserva, List<PrecoPorTemporada> precosPorTemporada, List<AltaTemporadaDto> datasAltaTemporada)
        {
            decimal valorTotal = 0;

            foreach (var diaReservado in periodoReserva.DiasDeFeriado)
            {
                var precoPorTemporada = await _temporadaService.GetPrecoPorTemporadaAsync(diaReservado, precosPorTemporada, datasAltaTemporada);
                decimal valorDiario = precoPorTemporada.ValorFeriado;
                valorTotal += valorDiario;

                var diaReservadoInfo = periodoReserva.Periodo.FirstOrDefault(d => d.DataReserva.Date == diaReservado.Date);
                if (diaReservadoInfo != null)
                {
                    diaReservadoInfo.ValorDiaria = valorDiario;
                    diaReservadoInfo.TemporadaId = precoPorTemporada.TemporadaId;
                    diaReservadoInfo.Feriado = true;
                }
            }

            return valorTotal;
        }

        public async Task<decimal> ObtemValorTotalDiasDeSemanaAsync(PeriodoReserva periodoReserva, List<PrecoPorTemporada> precosPorTemporada, List<AltaTemporadaDto> datasAltaTemporada)
        {
            decimal valorTotal = 0;

            foreach (var diaReservado in periodoReserva.DiasDeSemana)
            {
                var precoPorTemporada = await _temporadaService.GetPrecoPorTemporadaAsync(diaReservado, precosPorTemporada, datasAltaTemporada);
                decimal valorDiario = 0;

                if (diaReservado.Date == periodoReserva.Checkin.Date)
                {
                    if (periodoReserva.PeriodoCheckin == HorarioCheckin.Manha)
                    {
                        valorDiario = precoPorTemporada.ValorDiaDeSemana;
                    }
                    else if (periodoReserva.PeriodoCheckin == HorarioCheckin.Tarde)
                    {
                        valorDiario = precoPorTemporada.ValorMeioPeriodoSemana;
                    }
                    else if (periodoReserva.PeriodoCheckin == HorarioCheckin.Noite)
                    {
                        valorDiario = precoPorTemporada.ValorPernoite;
                    }
                }
                else if (diaReservado.Date == periodoReserva.Checkout.Date)
                {
                    if (periodoReserva.PeriodoCheckout == HorarioCheckout.Manha)
                    {
                        valorDiario = 0;
                    }
                    else if (periodoReserva.PeriodoCheckout == HorarioCheckout.Tarde)
                    {
                        valorDiario = precoPorTemporada.ValorMeioPeriodoSemana;
                    }
                    else if (periodoReserva.PeriodoCheckout == HorarioCheckout.Noite)
                    {
                        valorDiario = precoPorTemporada.ValorDiaDeSemana;
                    }
                }
                else
                {
                    valorDiario = precoPorTemporada.ValorDiaDeSemana;
                }

                valorTotal += valorDiario;

                var diaReservadoInfo = periodoReserva.Periodo.FirstOrDefault(d => d.DataReserva.Date == diaReservado.Date);
                if (diaReservadoInfo != null)
                {
                    diaReservadoInfo.ValorDiaria = valorDiario;
                    diaReservadoInfo.TemporadaId = precoPorTemporada.TemporadaId;
                }
            }

            return valorTotal;
        }

        public async Task<decimal> ObtemValorTotalFinaisDeSemanaAsync(PeriodoReserva periodoReserva, List<PrecoPorTemporada> precosPorTemporada, List<AltaTemporadaDto> datasAltaTemporada)
        {
            decimal valorTotal = 0;

            foreach (var diaReservado in periodoReserva.DiasDeFinalDeSemana)
            {
                var precoPorTemporada = await _temporadaService.GetPrecoPorTemporadaAsync(diaReservado, precosPorTemporada, datasAltaTemporada);
                decimal valorDiario = 0;

                if (diaReservado.Date == periodoReserva.Checkin.Date)
                {
                    if (periodoReserva.PeriodoCheckin == HorarioCheckin.Manha)
                    {
                        valorDiario = precoPorTemporada.ValorFinalDeSemana;
                    }
                    else if (periodoReserva.PeriodoCheckin == HorarioCheckin.Tarde)
                    {
                        valorDiario = precoPorTemporada.ValorMeioPeriodoFimDeSemana;
                    }
                }
                else if (diaReservado.Date == periodoReserva.Checkout.Date)
                {
                    if (periodoReserva.PeriodoCheckout == HorarioCheckout.Tarde)
                    {
                        valorDiario = precoPorTemporada.ValorMeioPeriodoFimDeSemana;
                    }
                    else if (periodoReserva.PeriodoCheckout == HorarioCheckout.Noite)
                    {
                        valorDiario = precoPorTemporada.ValorFinalDeSemana;
                    }
                }
                else
                {
                    valorDiario = precoPorTemporada.ValorFinalDeSemana;
                }

                valorTotal += valorDiario;

                var diaReservadoInfo = periodoReserva.Periodo.FirstOrDefault(d => d.DataReserva.Date == diaReservado.Date);
                if (diaReservadoInfo != null)
                {
                    diaReservadoInfo.ValorDiaria = valorDiario;
                    diaReservadoInfo.TemporadaId = precoPorTemporada.TemporadaId;
                }
            }

            return valorTotal;
        }

    }
}
