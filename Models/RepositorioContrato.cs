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
                string sql = @"
SELECT c.*, 
       c.InquilinoId, c.InmuebleId,
       i.Apellido AS InquilinoApellido,
       i.Nombre AS InquilinoNombre,
       inm.Direccion AS InmuebleDireccion
FROM contrato c
INNER JOIN inquilino i ON c.InquilinoId = i.Id
INNER JOIN inmueble inm ON c.InmuebleId = inm.Id;
";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var c = new Contrato
                        {
                            Id = reader.GetInt32("Id"),
                            FechaInicio = reader.GetDateTime("FechaInicio"),
                            FechaFin = reader.GetDateTime("FechaFin"),
                            Monto = reader.GetDecimal("Monto"),
                            InquilinoId = reader.GetInt32("InquilinoId"),
                            InmuebleId = reader.GetInt32("InmuebleId"),
                            Inquilino = new Inquilino
                            {
                                Nombre = reader.GetString("InquilinoNombre"), // Ahora se asigna a Nombre
                                Apellido = reader.GetString("InquilinoApellido") // Agregamos la asignación de Apellido
                            },
                            Inmueble = new Inmueble { Direccion = reader.GetString("InmuebleDireccion") }
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
            using (var connection = GetConnection()
            )
            {
                string sql = @"
SELECT c.*,
       i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
       inm.Direccion AS InmuebleDireccion
FROM contrato c
INNER JOIN inquilino i ON c.InquilinoId = i.Id
INNER JOIN inmueble inm ON c.InmuebleId = inm.Id
WHERE c.Id = @id;
"; // Consulta con JOIN para traer Inquilino e Inmueble
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
                            InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
                            // **NUEVO: Llenar los objetos Inquilino e Inmueble**
                            Inquilino = new Inquilino
                            {
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido")
                            },
                            Inmueble = new Inmueble
                            {
                                Direccion = reader.GetString("InmuebleDireccion")
                            }
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
        // 🏢 Obtener contratos por Inmueble ID
        public List<Contrato> ObtenerPorInmueble(int inmuebleId)
        {
            List<Contrato> lista = new List<Contrato>();
            using (var connection = GetConnection())
            {
                string sql = @"
SELECT 
    c*,
    c.InquilinoId, c.InmuebleId, 
    i.Apellido AS InquilinoApellido,
    i.Nombre AS InquilinoNombre,
    inm.Direccion AS InmuebleDireccion
FROM contrato c
INNER JOIN inquilino i ON c.InquilinoId = i.Id
INNER JOIN inmueble inm ON c.InmuebleId = inm.Id
WHERE c.InmuebleId = @InmuebleId
ORDER BY c.FechaInicio DESC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InmuebleId", inmuebleId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Mapeo del Contrato (similar a ObtenerPorId)
                        var c = new Contrato
                        {
                            // Cambia las cadenas por nameof() para asegurar la coincidencia.
                            Id = reader.GetInt32(nameof(Contrato.Id)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            Vigente = reader.GetBoolean(nameof(Contrato.Vigente)), // <--- Verifica esta columna.

                            // El manejo de nulos YA USA nameof(), lo cual es bueno.
                            FechaFinAnticipada = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.FechaFinAnticipada))) ? null : reader.GetDateTime(nameof(Contrato.FechaFinAnticipada)),
                            Multa = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.Multa))) ? null : reader.GetDecimal(nameof(Contrato.Multa)),

                            InquilinoId = reader.GetInt32("InquilinoId"),
                            InmuebleId = reader.GetInt32("InmuebleId"),

                            // Los JOIN usan alias, así que estos están bien:
                            Inquilino = new Inquilino
                            {
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido")
                            },
                            Inmueble = new Inmueble { Direccion = reader.GetString("InmuebleDireccion") }
                        };

                        lista.Add(c);
                    }
                    connection.Close();
                }
            }
            return lista;
        }
        // 📅 Obtener contratos vigentes en una fecha específica
        public List<Contrato> ObtenerVigentesEnFecha(DateTime fecha)
        {
            List<Contrato> lista = new List<Contrato>();
            using (var connection = GetConnection())
            {
                string sql = @"
SELECT c.*, c.Vigente, c.FechaFinAnticipada, c.Multa,
       i.Apellido AS InquilinoApellido,
       i.Nombre AS InquilinoNombre,
       inm.Direccion AS InmuebleDireccion
FROM contrato c
INNER JOIN inquilino i ON c.InquilinoId = i.Id
INNER JOIN inmueble inm ON c.InmuebleId = inm.Id
WHERE c.FechaInicio <= @fecha AND c.FechaFin >= @fecha
      AND c.Vigente = 1; 
";
                using (var command = new MySqlCommand(sql, connection))
                {
                    // Nota: Usamos .Date para asegurar que la hora no interfiera
                    command.Parameters.AddWithValue("@fecha", fecha.Date);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var c = new Contrato
                        {
                            Id = reader.GetInt32("Id"),
                            FechaInicio = reader.GetDateTime("FechaInicio"),
                            FechaFin = reader.GetDateTime("FechaFin"),
                            Monto = reader.GetDecimal("Monto"),
                            Vigente = reader.GetBoolean("Vigente"),

                            // Manejo de valores nulos
                            FechaFinAnticipada = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.FechaFinAnticipada))) ? null : reader.GetDateTime(nameof(Contrato.FechaFinAnticipada)),
                            Multa = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.Multa))) ? null : reader.GetDecimal(nameof(Contrato.Multa)),

                            InquilinoId = reader.GetInt32("InquilinoId"),
                            InmuebleId = reader.GetInt32("InmuebleId"),

                            // Objetos relacionados (JOIN)
                            Inquilino = new Inquilino
                            {
                                Nombre = reader.GetString("InquilinoNombre"),
                                Apellido = reader.GetString("InquilinoApellido")
                            },
                            Inmueble = new Inmueble { Direccion = reader.GetString("InmuebleDireccion") }
                        };

                        lista.Add(c);
                    }
                    connection.Close();
                }
            }
            return lista;
        }

    }
}
