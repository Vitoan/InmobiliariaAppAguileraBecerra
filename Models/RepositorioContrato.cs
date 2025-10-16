using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioContrato : RepositorioBase
    {
        public RepositorioContrato() : base() { }

        // 📋 Alta
        public int Alta(Contrato c)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = @"INSERT INTO contrato 
                               (FechaInicio, FechaFin, Monto, Vigente, InquilinoId, InmuebleId)
                               VALUES (@FechaInicio, @FechaFin, @Monto, @Vigente, @InquilinoId, @InmuebleId);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("@Monto", c.Monto);
                    command.Parameters.AddWithValue("@Vigente", c.Vigente);
                    command.Parameters.AddWithValue("@InquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@InmuebleId", c.InmuebleId);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        // 🔍 Obtener todos
        public List<Contrato> ObtenerTodos()
        {
            List<Contrato> lista = new List<Contrato>();
            using (var connection = GetConnection())
            {
                string sql = @"SELECT Id, FechaInicio, FechaFin, Monto, FechaFinAnticipada, Multa, Vigente, InquilinoId, InmuebleId 
                               FROM contrato;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var c = new Contrato
                        {
                            Id = reader.GetInt32(nameof(Contrato.Id)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            FechaFinAnticipada = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.FechaFinAnticipada))) ? null : reader.GetDateTime(nameof(Contrato.FechaFinAnticipada)),
                            Multa = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.Multa))) ? null : reader.GetDecimal(nameof(Contrato.Multa)),
                            Vigente = reader.GetBoolean(nameof(Contrato.Vigente)),
                            InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
                            InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId))
                        };
                        lista.Add(c);
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        // 🔍 Obtener por ID
        public Contrato ObtenerPorId(int id)
        {
            Contrato c = null;
            using (var connection = GetConnection())
            {
                string sql = @"SELECT Id, FechaInicio, FechaFin, Monto, FechaFinAnticipada, Multa, Vigente, InquilinoId, InmuebleId 
                               FROM contrato WHERE Id=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        c = new Contrato
                        {
                            Id = reader.GetInt32(nameof(Contrato.Id)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            FechaFinAnticipada = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.FechaFinAnticipada))) ? null : reader.GetDateTime(nameof(Contrato.FechaFinAnticipada)),
                            Multa = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.Multa))) ? null : reader.GetDecimal(nameof(Contrato.Multa)),
                            Vigente = reader.GetBoolean(nameof(Contrato.Vigente)),
                            InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
                            InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId))
                        };
                    }
                    connection.Close();
                }
            }
            return c;
        }

        // ✏️ Modificación general
        public int Modificacion(Contrato c)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = @"UPDATE contrato SET 
                               FechaInicio=@FechaInicio,
                               FechaFin=@FechaFin,
                               Monto=@Monto,
                               InquilinoId=@InquilinoId,
                               InmuebleId=@InmuebleId
                               WHERE Id=@Id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("@Monto", c.Monto);
                    command.Parameters.AddWithValue("@InquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@InmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@Id", c.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        // ❌ Baja (eliminar)
        public int Baja(int id)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = "DELETE FROM contrato WHERE Id=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
// ❓ Verificar superposición (nuevo método)

        public bool ExisteSuperposicion(Contrato c)
{
    bool existe = false;
    using (var connection = GetConnection())
    {
        string sql = @"
            SELECT COUNT(*) 
            FROM contrato
            WHERE InmuebleId = @InmuebleId
              AND Id <> @Id
              AND Vigente = 1
              AND (
                    (FechaInicio < @FechaFin AND FechaFin > @FechaInicio)
                  );
        ";
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@InmuebleId", c.InmuebleId);
            command.Parameters.AddWithValue("@Id", c.Id); // si es nuevo, Id = 0
            command.Parameters.AddWithValue("@FechaInicio", c.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin", c.FechaFin);
            connection.Open();
            int count = Convert.ToInt32(command.ExecuteScalar() ?? 0);
            existe = count > 0;
        }
    }
    return existe;
}


        // 🔴 Finalizar anticipadamente (nuevo método)
        public int FinalizarAnticipado(Contrato c)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = @"UPDATE contrato 
                               SET FechaFinAnticipada=@FechaFinAnticipada, 
                                   Multa=@Multa, 
                                   Vigente=FALSE
                               WHERE Id=@Id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FechaFinAnticipada", c.FechaFinAnticipada);
                    command.Parameters.AddWithValue("@Multa", c.Multa);
                    command.Parameters.AddWithValue("@Id", c.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
    }
}
