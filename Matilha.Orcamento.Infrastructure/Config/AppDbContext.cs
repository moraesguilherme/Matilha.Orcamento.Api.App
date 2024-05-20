using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Orcamento.Infraestructure.Data
{
    public class AppDbContext
    {
        private readonly IConfiguration _config;
        public IDbConnection Connection { get; }

        public AppDbContext(IConfiguration config)
        {
            _config = config;
            string connectionString = _config.GetConnectionString("DefaultConnection");
            Connection = new SqlConnection(connectionString);
        }
    }
}
