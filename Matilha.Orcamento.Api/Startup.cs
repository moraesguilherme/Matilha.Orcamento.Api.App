using Orcamento.Infraestructure.Data;
using Microsoft.OpenApi.Models;
using Orcamento.Infraestructure.Repositories.Interfaces;
using Matilha.Orcamento.Domain.Repositories;
using Matilha.Orcamento.Domain.Interfaces.Services;
using Matilha.Orcamento.Domain.Services;

namespace Orcamento
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<AppDbContext>();
            
            services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IPeriodoRepository, PeriodoRepository>();
            services.AddScoped<ICachorroRepository, CachorroRepository>();
            services.AddScoped<ITemporadaRepository, TemporadaRepository>();
            services.AddScoped<IOrcamentoService, OrcamentoService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<ICachorroService, CachorroService>();
            services.AddScoped<ICalculaOrcamentoService, CalculaOrcamentoService>();
            services.AddScoped<IPeriodoService, PeriodoService>();
            services.AddScoped<ITemporadaService, TemporadaService>();
            services.AddControllers();
            services.AddHttpClient<PeriodoService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orcamento API", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orcamento API V1");
            });

            app.UseRouting();

            app.UseCors("AllowAllOrigins");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
