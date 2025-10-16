using System.Data;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioAuditoria
    {
        // 🚨 CORRECCIÓN: Se cambió 'Database=inmobiliaria' por 'Database=inmobiliaria_db' 
        // para que coincida con el nombre real de la base de datos.
        private readonly string connectionString = "Server=localhost;Database=inmobiliaria_db;User=root;Password=;";

        public void Registrar(Auditoria a)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO auditoria 
                            (Tabla, Operacion, RegistroId, DatosAnteriores, DatosNuevos, Fecha, Usuario)
                            VALUES (@tabla, @operacion, @registroId, @datosAnteriores, @datosNuevos, NOW(), @usuario)";
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@tabla", a.Tabla);
                command.Parameters.AddWithValue("@operacion", a.Operacion);
                command.Parameters.AddWithValue("@registroId", a.RegistroId);
                command.Parameters.AddWithValue("@datosAnteriores", (object?)a.DatosAnteriores ?? DBNull.Value);
                command.Parameters.AddWithValue("@datosNuevos", (object?)a.DatosNuevos ?? DBNull.Value);
                command.Parameters.AddWithValue("@usuario", a.Usuario);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        // 🔹 Método simplificado para registrar desde controladores
        public void RegistrarCambio(string tabla, string operacion, int registroId, object? datosAntes, object? datosDespues, string? usuario)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var auditoria = new Auditoria
            {
                Tabla = tabla,
                Operacion = operacion,
                RegistroId = registroId,
                DatosAnteriores = datosAntes != null ? JsonSerializer.Serialize(datosAntes, jsonOptions) : null,
                DatosNuevos = datosDespues != null ? JsonSerializer.Serialize(datosDespues, jsonOptions) : null,
                Usuario = usuario ?? "Sistema"
            };
            Registrar(auditoria);
        }

        public Auditoria? ObtenerPorId(int id)
        {
            Auditoria? res = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM auditoria WHERE Id = @id";
                var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    res = new Auditoria
                    {
                        Id = reader.GetInt32(nameof(Auditoria.Id)),
                        Tabla = reader.GetString(nameof(Auditoria.Tabla)),
                        Operacion = reader.GetString(nameof(Auditoria.Operacion)),
                        RegistroId = reader.GetInt32(nameof(Auditoria.RegistroId)),
                        DatosAnteriores = reader.IsDBNull(reader.GetOrdinal(nameof(Auditoria.DatosAnteriores))) ? null : reader.GetString(nameof(Auditoria.DatosAnteriores)),
                        DatosNuevos = reader.IsDBNull(reader.GetOrdinal(nameof(Auditoria.DatosNuevos))) ? null : reader.GetString(nameof(Auditoria.DatosNuevos)),
                        Fecha = reader.GetDateTime(nameof(Auditoria.Fecha)),
                        Usuario = reader.GetString(nameof(Auditoria.Usuario))
                    };
                }
                connection.Close();
            }
            return res;
        }

        // 🔹 Método para obtener registros con filtros opcionales
        public List<Auditoria> ObtenerConFiltros(string? tabla, string? operacion, string? usuario, DateTime? desde, DateTime? hasta)
        {
            var lista = new List<Auditoria>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM auditoria WHERE 1=1";
                if (!string.IsNullOrEmpty(tabla))
                    sql += " AND Tabla = @tabla";
                if (!string.IsNullOrEmpty(operacion))
                    sql += " AND Operacion = @operacion";
                if (!string.IsNullOrEmpty(usuario))
                    sql += " AND Usuario LIKE @usuario";
                if (desde.HasValue)
                    sql += " AND Fecha >= @desde";
                if (hasta.HasValue)
                    sql += " AND Fecha <= @hasta";

                sql += " ORDER BY Fecha DESC";

                var command = new MySqlCommand(sql, connection);

                if (!string.IsNullOrEmpty(tabla))
                    command.Parameters.AddWithValue("@tabla", tabla);
                if (!string.IsNullOrEmpty(operacion))
                    command.Parameters.AddWithValue("@operacion", operacion);
                if (!string.IsNullOrEmpty(usuario))
                    command.Parameters.AddWithValue("@usuario", $"%{usuario}%");
                if (desde.HasValue)
                    command.Parameters.AddWithValue("@desde", desde.Value);
                if (hasta.HasValue)
                    command.Parameters.AddWithValue("@hasta", hasta.Value);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var a = new Auditoria
                    {
                        Id = reader.GetInt32("Id"),
                        Tabla = reader.GetString("Tabla"),
                        Operacion = reader.GetString("Operacion"),
                        RegistroId = reader.GetInt32("RegistroId"),
                        DatosAnteriores = reader.IsDBNull(reader.GetOrdinal("DatosAnteriores")) ? null : reader.GetString("DatosAnteriores"),
                        DatosNuevos = reader.IsDBNull(reader.GetOrdinal("DatosNuevos")) ? null : reader.GetString("DatosNuevos"),
                        Fecha = reader.GetDateTime("Fecha"),
                        Usuario = reader.GetString("Usuario")
                    };
                    lista.Add(a);
                }
                connection.Close();
            }
            return lista;
        }
    }
}
