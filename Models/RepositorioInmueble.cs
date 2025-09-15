using MySql.Data.MySqlClient;
using System.Data;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioInmueble : RepositorioBase
    {
        public int Alta(Inmueble i)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"INSERT INTO inmueble (Direccion, Uso, Tipo, Ambientes, Latitud, Longitud, Precio, Disponible, PropietarioId)
                                   VALUES (@direccion, @uso, @tipo, @ambientes, @latitud, @longitud, @precio, @disponible, @propietarioId);
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
                        connection.Open();
                        res = Convert.ToInt32(command.ExecuteScalar() ?? 0);
                        i.Id = res;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el inmueble.", ex);
            }
            return res;
        }

        public int Modificacion(Inmueble i)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"UPDATE inmueble
                                   SET Direccion=@direccion, Uso=@uso, Tipo=@tipo, Ambientes=@ambientes, Latitud=@latitud, Longitud=@longitud, Precio=@precio, Disponible=@disponible, PropietarioId=@propietarioId
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
                        command.Parameters.AddWithValue("@id", i.Id);
                        connection.Open();
                        res = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el inmueble.", ex);
            }
            return res;
        }

        public IList<Inmueble> ObtenerTodos()
        {
            var res = new List<Inmueble>();
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"SELECT i.Id, Direccion, Uso, Tipo, Ambientes, Latitud, Longitud, Precio, Disponible, PropietarioId, p.Nombre, p.Apellido, p.DNI
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
                                    Tipo = reader.GetString("Tipo") ?? "",
                                    Ambientes = reader.GetInt32("Ambientes"),
                                    Latitud = reader.GetDecimal("Latitud"),
                                    Longitud = reader.GetDecimal("Longitud"),
                                    Precio = reader.GetDecimal("Precio"),
                                    Disponible = reader.GetBoolean("Disponible"),
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los inmuebles.", ex);
            }
            return res;
        }
        
        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? i = null;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"SELECT Id, Direccion, Uso, Tipo, Ambientes, Latitud, Longitud, Precio, Disponible, PropietarioId
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
                                    Tipo = reader.GetString("Tipo") ?? "",
                                    Ambientes = reader.GetInt32("Ambientes"),
                                    Latitud = reader.GetDecimal("Latitud"),
                                    Longitud = reader.GetDecimal("Longitud"),
                                    Precio = reader.GetDecimal("Precio"),
                                    Disponible = reader.GetBoolean("Disponible"),
                                    PropietarioId = reader.GetInt32("PropietarioId")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el inmueble con ID {id}.", ex);
            }
            return i;
        }

        public int Baja(int id)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string sqlChequeo = "SELECT COUNT(*) FROM contrato WHERE InmuebleId = @id AND Vigente = TRUE";
                    using (var cmdChequeo = new MySqlCommand(sqlChequeo, connection))
                    {
                        cmdChequeo.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmdChequeo.ExecuteScalar() ?? 0);
                        if (count > 0)
                            return 0;
                    }

                    string sqlDelete = "DELETE FROM inmueble WHERE Id = @id";
                    using (var command = new MySqlCommand(sqlDelete, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        res = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el inmueble.", ex);
            }
            return res;
        }
    }
}