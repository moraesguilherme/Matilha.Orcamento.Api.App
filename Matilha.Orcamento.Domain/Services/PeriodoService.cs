using Matilha.Orcamento.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Matilha.Orcamento.Domain.Services
{
    public class PeriodoService : IPeriodoService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IPeriodoRepository _periodoRepository;


        public PeriodoService(IConfiguration configuration, HttpClient httpClient, IPeriodoRepository periodoRepository)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _periodoRepository = periodoRepository;
        }

        public async Task<PeriodoReserva> ObtemPeriodoAsync(DateTime dataCheckin, TimeSpan horarioCheckin, DateTime dataCheckout, TimeSpan horarioCheckout)
        {
            DateTime checkIn = dataCheckin.Date.Add(horarioCheckin);
            DateTime checkOut = dataCheckout.Date.Add(horarioCheckout);

            List<DateTime> feriados = await ListaFeriadoAsync(checkIn, checkOut);
            List<DateTime> semana = await ListaDiasDaSemanaAsync(checkIn, checkOut, feriados);
            List<DateTime> finalDeSemana = await ListaFinalDeSemanaAsync(checkIn, checkOut, feriados);
            List<DateTime> periodo = await ListaPeriodoAsync(checkIn, checkOut);

            var periodoComDiasDaSemana = await ObterPeriodoComDiasDaSemanaAsync(periodo);

            var periodoReserva = MontaPeriodoReserva(checkIn, checkOut, periodoComDiasDaSemana, feriados, semana, finalDeSemana);

            return periodoReserva;
        }

        private async Task<List<DiasReservados>> ObterPeriodoComDiasDaSemanaAsync(List<DateTime> periodo)
        {
            var periodoComDiasDaSemana = new List<DiasReservados>();
            foreach (var data in periodo)
            {
                string diaDaSemana = await ObterDiaDaSemanaAsync(data);
                periodoComDiasDaSemana.Add(new DiasReservados { DataReserva = data, DiaDaSemana = diaDaSemana });
            }
            return periodoComDiasDaSemana;
        }

        private PeriodoReserva MontaPeriodoReserva(DateTime checkIn, DateTime checkOut, List<DiasReservados> periodoComDiasDaSemana, List<DateTime> feriados, List<DateTime> semana, List<DateTime> finalDeSemana)
        {
            TimeSpan diferencaTotalDeDias = checkOut - checkIn;
            int qtdDias = (int)Math.Ceiling(diferencaTotalDeDias.TotalDays);
            int qtdFeriados = feriados.Count;
            int qtdDiasDeSemana = semana.Count(data => !feriados.Any(feriado => feriado.Date == data.Date));
            int qtdFinaisDeSemana = finalDeSemana.Count(data => !feriados.Any(feriado => feriado.Date == data.Date));
            var periodoCheckin = DeterminarPeriodoCheckin(checkIn.TimeOfDay);
            var periodoCheckout = DeterminarPeriodoCheckout(checkOut.TimeOfDay);

            var periodoReserva = new PeriodoReserva
            {
                Checkin = checkIn,
                Checkout = checkOut,
                Periodo = periodoComDiasDaSemana,
                QtdTotalDeDias = qtdDias,
                QtdDiasDeSemana = qtdDiasDeSemana,
                QtdFinaisDeSemana = qtdFinaisDeSemana,
                QtdFeriados = qtdFeriados,
                PeriodoCheckin = periodoCheckin,
                PeriodoCheckout = periodoCheckout,
                DiasDeSemana = semana,
                DiasDeFeriado = feriados,
                DiasDeFinalDeSemana = finalDeSemana
            };
            return periodoReserva;
        }

        private async Task<List<DateTime>> ListaPeriodoAsync(DateTime dataEntrada, DateTime dataSaida)
        {
            List<DateTime> diasDaSemana = new List<DateTime>();
            for (DateTime data = dataEntrada; data <= dataSaida; data = data.AddDays(1))
            {
                diasDaSemana.Add(data.Date == dataSaida.Date ? dataSaida : data);
            }

            return diasDaSemana;
        }

        private async Task<List<DateTime>> ListaDiasDaSemanaAsync(DateTime checkin, DateTime checkout, List<DateTime> feriados)
        {
            List<DateTime> diasDeSemana = new List<DateTime>();

            for (DateTime data = checkin.Date; data.Date <= checkout.Date; data = data.AddDays(1))
            {
                if (data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday && !feriados.Any(f => f.Date == data.Date))
                {
                    diasDeSemana.Add(data);
                }
            }

            return diasDeSemana;
        }

        private async Task<List<DateTime>> ListaFinalDeSemanaAsync(DateTime dataEntrada, DateTime dataSaida, List<DateTime> feriados)
        {
            List<DateTime> finalDeSemana = new List<DateTime>();

            for (DateTime data = dataEntrada.Date; data <= dataSaida.Date; data = data.AddDays(1))
            {
                if ((data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday) && !feriados.Any(f => f.Date == data.Date))
                {
                    finalDeSemana.Add(data);
                }
            }

            return finalDeSemana;
        }

        public async Task<List<DateTime>> ListaFeriadoAsync(DateTime dataEntrada, DateTime dataSaida)
        {
            List<DateTime> feriadosDoAno = await _periodoRepository.ObtemFeriadosDoAnoAsync(dataEntrada, dataSaida);

            var validaFeriadosAnuais = Enumerable.Range(dataEntrada.Year, dataSaida.Year - dataEntrada.Year + 1)
                              .Except(feriadosDoAno.Select(feriado => feriado.Year));

            if (validaFeriadosAnuais.Any())
            {
                await BuscarFeriadosDoAnoAsync(dataSaida.Year);
                feriadosDoAno = await _periodoRepository.ObtemFeriadosDoAnoAsync(dataEntrada, dataSaida);
            }

            List<DateTime> feriados = new List<DateTime>();

            for (DateTime data = dataEntrada; data <= dataSaida; data = data.AddDays(1))
            {
                if (feriadosDoAno.Contains(data.Date))
                {
                    feriados.Add(data);
                }
            }

            return feriados;
        }

        public async Task<string> ObterDiaDaSemanaAsync(DateTime data)
        {
            string[] diasDaSemana =
                { "Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "Sábado" };
            return diasDaSemana[(int)data.DayOfWeek];
        }

        public async Task BuscarFeriadosDoAnoAsync(int ano)
        {
            try
            {
                string anoString = ano.ToString();
                string baseUrl = _configuration.GetValue<string>("ApiSettings:BaseUrl");
                string token = _configuration.GetValue<string>("ApiSettings:Token");
                string url = $"{baseUrl}holidays/{anoString}?token={token}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var feriados = JsonConvert.DeserializeObject<List<Feriado>>(content);

                    await _periodoRepository.SalvaFeriadosNacionais(feriados, ano);
                }
                else
                {
                    Console.WriteLine($"Falha ao consultar a API externa: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro ao consultar a API externa: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public static HorarioCheckin DeterminarPeriodoCheckin(TimeSpan horario)
        {
            return horario >= new TimeSpan(7, 0, 0) && horario < new TimeSpan(12, 0, 0) ?
                HorarioCheckin.Manha :
                horario >= new TimeSpan(12, 0, 0) && horario < new TimeSpan(17, 0, 0) ?
                    HorarioCheckin.Tarde :
                    HorarioCheckin.Noite;
        }

        public static HorarioCheckout DeterminarPeriodoCheckout(TimeSpan horario)
        {
            return horario >= new TimeSpan(7, 0, 0) && horario < new TimeSpan(12, 0, 0) ?
                HorarioCheckout.Manha :
                horario >= new TimeSpan(12, 0, 0) && horario < new TimeSpan(17, 0, 0) ?
                    HorarioCheckout.Tarde :
                    HorarioCheckout.Noite;
        }
    }
}
