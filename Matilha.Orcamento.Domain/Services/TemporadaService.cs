using Matilha.Orcamento.Domain.Interfaces.Services;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Orcamento.Models.Dtos;
using Orcamento.Models.Entities;
using Orcamento.Models.Enums;

namespace Matilha.Orcamento.Domain.Services
{
    public class TemporadaService : ITemporadaService
    {
        private readonly ITemporadaRepository _altaTemporadaRepository;

        public TemporadaService(ITemporadaRepository altaTemporadaRepository)
        {
            _altaTemporadaRepository = altaTemporadaRepository;
        }

        public async Task InserirAltaTemporadaAsync(AltaTemporada altaTemporada)
        {
            try
            {
                int anoInicio = altaTemporada.InicioTemporada.Year;
                int anoFim = altaTemporada.FimTemporada.Year;

                for (int ano = anoInicio; ano <= anoFim; ano++)
                {
                    DateTime inicioAno = new DateTime(ano, 1, 1);
                    DateTime fimAno = new DateTime(ano, 12, 31);

                    DateTime inicioTemporada = ano == anoInicio ? altaTemporada.InicioTemporada : inicioAno;
                    DateTime fimTemporada = ano == anoFim ? altaTemporada.FimTemporada : fimAno;

                    int mesInicio = inicioTemporada.Month;
                    int mesFim = fimTemporada.Month;

                    foreach (var data in CalculaRangeDeDatas(inicioTemporada, fimTemporada))
                    {
                        await _altaTemporadaRepository.InserirAltaTemporadaAsync(data, mesInicio, ano, altaTemporada.Descricao);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inserir alta temporada: " + ex.Message);
            }
        }

        private static IEnumerable<DateTime> CalculaRangeDeDatas(DateTime inicio, DateTime fim)
        {
            for (DateTime data = inicio; data <= fim; data = data.AddDays(1))
            {
                yield return data;
            }
        }

        public async Task<PrecoPorTemporada> GetPrecoPorTemporadaAsync(DateTime diaReservado, List<PrecoPorTemporada> precosPorTemporada, List<AltaTemporadaDto> datasAltaTemporada)
        {
            bool isAltaTemporada = datasAltaTemporada.Any(dto => dto.Data.Date == diaReservado.Date);

            var precoPorTemporada = isAltaTemporada ?
                precosPorTemporada.FirstOrDefault(preco => preco.TemporadaId == TemporadaId.Alta) :
                precosPorTemporada.FirstOrDefault(preco => preco.TemporadaId == TemporadaId.Baixa);

            return precoPorTemporada;
        }

        public async Task<List<AltaTemporadaDto>> ObterDatasAltaTemporadaAsync()
        {
            return await _altaTemporadaRepository.ObterDatasAltaTemporadaAsync();
        }

        public async Task ExcluirDataAltaTemporadaAsync(int id)
        {
            await _altaTemporadaRepository.ExcluirDataAltaTemporadaAsync(id);
        }

    }
}
