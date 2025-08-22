using MySql.Data.MySqlClient;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public abstract class RepositorioBase
    {
        protected readonly string connectionString = "Server=localhost;User=root;Password=;Database=inmobiliaria_db;SslMode=none";

        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}