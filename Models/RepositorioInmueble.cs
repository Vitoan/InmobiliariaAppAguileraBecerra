using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioInmueble : RepositorioBase
    {
        public int Alta(Inmueble i)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = @"INSERT INTO inmueble 
                               (Direccion, Uso, Tipo, Ambientes, Latitud, Longitud, Precio, Disponible, PropietarioId, Portada)
                               VALUES (@direccion, @uso, @tipo, @ambientes, @latitud, @longitud, @precio, @disponible, @propietarioId, @portada);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@uso", i.Uso);
                    command.Parameters.AddWithValue("@tipo", i.Tipo);
                    command.Parameters.AddWithValue("@ambientes", i.Ambientes);
                    command.Parameters.AddWithValue("@latitud", i.Latitud);
                    command.Parameters.AddWithValue("@longitud", i.Longitud);
                    command.Parameters.AddWithValue("@precio", i.Precio);
                    command.Parameters.AddWithValue("@disponible", i.Disponible);
                    command.Parameters.AddWithValue("@propietarioId", i.PropietarioId);
                    command.Parameters.AddWithValue("@portada", string.IsNullOrEmpty(i.Portada) ? DBNull.Value : i.Portada);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar() ?? 0);
                    i.Id = res;
                }
            }
            return res;
        }

        public int Modificacion(Inmueble i)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = @"UPDATE inmueble
                               SET Direccion=@direccion, Uso=@uso, Tipo=@tipo, Ambientes=@ambientes, 
                                   Latitud=@latitud, Longitud=@longitud, Precio=@precio, Disponible=@disponible, 
                                   PropietarioId=@propietarioId, Portada=@portada
                               WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@uso", i.Uso);
                    command.Parameters.AddWithValue("@tipo", i.Tipo);
                    command.Parameters.AddWithValue("@ambientes", i.Ambientes);
                    command.Parameters.AddWithValue("@latitud", i.Latitud);
                    command.Parameters.AddWithValue("@longitud", i.Longitud);
                    command.Parameters.AddWithValue("@precio", i.Precio);
                    command.Parameters.AddWithValue("@disponible", i.Disponible);
                    command.Parameters.AddWithValue("@propietarioId", i.PropietarioId);
                    command.Parameters.AddWithValue("@portada", string.IsNullOrEmpty(i.Portada) ? DBNull.Value : i.Portada);
                    command.Parameters.AddWithValue("@id", i.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerTodos()
        {
            var res = new List<Inmueble>();
            using (var connection = GetConnection())
            {
                string sql = @"SELECT i.Id, Direccion, Uso, Tipo, Ambientes, Latitud, Longitud, Precio, Disponible, PropietarioId, Portada, p.Nombre, p.Apellido, p.DNI
                               FROM inmueble i
                               INNER JOIN propietario p ON i.PropietarioId = p.Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion") ?? "",
                                Uso = reader.GetString("Uso") ?? "",
                                Tipo = reader.GetInt32("Tipo"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio = reader.GetDecimal("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Portada = reader.IsDBNull(reader.GetOrdinal("Portada")) ? null : reader.GetString("Portada"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Duenio = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    DNI = reader.GetString("DNI") ?? ""
                                }
                            });
                        }
                    }
                }
            }
            return res;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? i = null;
            using (var connection = GetConnection())
            {
                string sql = @"SELECT Id, Direccion, Uso, Tipo, Ambientes, Latitud, Longitud, Precio, Disponible, PropietarioId, Portada
                               FROM inmueble WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            i = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion") ?? "",
                                Uso = reader.GetString("Uso") ?? "",
                                Tipo = reader.GetInt32("Tipo"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio = reader.GetDecimal("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Portada = reader.IsDBNull(reader.GetOrdinal("Portada")) ? null : reader.GetString("Portada"),
                                PropietarioId = reader.GetInt32("PropietarioId")
                            };
                        }
                    }
                }
            }
            return i;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                connection.Open();

                string sqlChequeo = "SELECT COUNT(*) FROM contrato WHERE InmuebleId = @id AND Vigente = TRUE";
                using (var cmdChequeo = new MySqlCommand(sqlChequeo, connection))
                {
                    cmdChequeo.Parameters.AddWithValue("@id", id);
                    int count = Convert.ToInt32(cmdChequeo.ExecuteScalar() ?? 0);
                    if (count > 0) return 0;
                }

                // Borrar archivo de portada si existe
                string sqlGetPortada = "SELECT Portada FROM inmueble WHERE Id=@id";
                using (var cmdGet = new MySqlCommand(sqlGetPortada, connection))
                {
                    cmdGet.Parameters.AddWithValue("@id", id);
                    var portada = cmdGet.ExecuteScalar() as string;
                    if (!string.IsNullOrEmpty(portada))
                    {
                        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", portada.TrimStart('/'));
                        if (File.Exists(ruta)) File.Delete(ruta);
                    }
                }

                string sqlDelete = "DELETE FROM inmueble WHERE Id = @id";
                using (var command = new MySqlCommand(sqlDelete, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (var connection = GetConnection())
            {
                string sql = @"
                    SELECT i.Id, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Latitud, i.Longitud, i.Precio, i.Disponible, i.PropietarioId, i.Portada,
                           p.Nombre, p.Apellido, p.DNI
                    FROM inmueble i
                    INNER JOIN propietario p ON i.PropietarioId = p.Id
                    ORDER BY i.Id
                    LIMIT @tamPagina OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion") ?? "",
                                Uso = reader.GetString("Uso") ?? "",
                                Tipo = reader.GetInt32("Tipo"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio = reader.GetDecimal("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Portada = reader.IsDBNull(reader.GetOrdinal("Portada")) ? null : reader.GetString("Portada"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Duenio = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    DNI = reader.GetString("DNI") ?? ""
                                }
                            });
                        }
                    }
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (var connection = GetConnection())
            {
                string sql = "SELECT COUNT(*) FROM inmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }

        public IList<Inmueble> BuscarPorPropietario(int idPropietario)
        {
            List<Inmueble> res = new List<Inmueble>();
            using (var connection = GetConnection())
            {
                string sql = @"
                    SELECT i.Id, Direccion, Uso, Tipo, Ambientes, Latitud, Longitud, Precio, Disponible, PropietarioId, Portada, 
                        p.Nombre, p.Apellido
                    FROM inmueble i 
                    JOIN propietario p ON i.PropietarioId = p.Id
                    WHERE PropietarioId=@idPropietario";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPropietario", idPropietario);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion") ?? "",
                                Uso = reader.GetString("Uso") ?? "",
                                Tipo = reader.GetInt32("Tipo"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio = reader.GetDecimal("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Portada = reader.IsDBNull(reader.GetOrdinal("Portada")) ? null : reader.GetString("Portada"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Duenio = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? ""
                                }
                            });
                        }
                    }
                }
            }
            return res;
        }


        public int ModificarPortada(int id, string url)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = @"UPDATE inmueble SET Portada=@portada WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@portada", string.IsNullOrEmpty(url) ? DBNull.Value : url);
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerDisponibles()
        {
            var res = new List<Inmueble>();
            using (var connection = GetConnection())
            {
                string sql = @"SELECT Id, Direccion, Uso, Tipo, Ambientes, Latitud, Longitud,
                                      Precio, Disponible, PropietarioId, Habilitado, Portada
                            FROM inmueble
                            WHERE Disponible = 1 AND Habilitado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Uso = reader.GetString("Uso"),
                                Tipo = reader.GetInt32("Tipo"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? (decimal?)null : reader.GetDecimal("Latitud"),
                                Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? (decimal?)null : reader.GetDecimal("Longitud"),
                                Precio = reader.GetDecimal("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Habilitado = reader.GetBoolean("Habilitado"),
                                Portada = reader.IsDBNull(reader.GetOrdinal("Portada")) ? null : reader.GetString("Portada")
                            };
                            res.Add(inmueble);
                        }
                    }
                }
            }
            return res;
        }
        
        public IList<Inmueble> ObtenerDisponiblesPaginados(int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (var connection = GetConnection())
            {
                string sql = @"
                    SELECT i.Id, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Latitud, i.Longitud, 
                           i.Precio, i.Disponible, i.PropietarioId, i.Portada,
                           p.Nombre, p.Apellido, p.DNI
                    FROM inmueble i
                    INNER JOIN propietario p ON i.PropietarioId = p.Id
                    WHERE i.Disponible = 1
                    ORDER BY i.Id
                    LIMIT @tamPagina OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion") ?? "",
                                Uso = reader.GetString("Uso") ?? "",
                                Tipo = reader.GetInt32("Tipo"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio = reader.GetDecimal("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Portada = reader.IsDBNull(reader.GetOrdinal("Portada")) ? null : reader.GetString("Portada"),
                                Duenio = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    DNI = reader.GetString("DNI") ?? ""
                                }
                            });
                        }
                    }
                }
            }
            return res;
        }

        public int ObtenerCantidadDisponibles()
        {
            int res = 0;
            using (var connection = GetConnection())
            {
                string sql = "SELECT COUNT(*) FROM inmueble WHERE Disponible = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }

    }
}
