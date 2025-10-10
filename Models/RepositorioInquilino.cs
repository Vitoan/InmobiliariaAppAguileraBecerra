using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioInquilino : RepositorioBase
    {
        public int Alta(Inquilino i)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    // CORRECCIÓN: Nombres de columna en snake_case en la tabla
                    string sql = @"INSERT INTO inquilino (nombre, apellido, dni, telefono, email)
                                   VALUES (@nombre, @apellido, @dni, @telefono, @email);
                                   SELECT LAST_INSERT_ID();";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@nombre", i.Nombre);
                        command.Parameters.AddWithValue("@apellido", i.Apellido);
                        command.Parameters.AddWithValue("@dni", i.DNI);
                        command.Parameters.AddWithValue("@telefono", (object?)i.Telefono ?? DBNull.Value);
                        command.Parameters.AddWithValue("@email", (object?)i.Email ?? DBNull.Value);
                        connection.Open();
                        res = Convert.ToInt32(command.ExecuteScalar() ?? 0);
                        i.Id = res;
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new Exception("El DNI ya está registrado.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el inquilino.", ex);
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    // CORRECCIÓN: La columna es inquilino_id, no Inquilino_Id
                    string sqlChequeo = "SELECT COUNT(*) FROM contrato WHERE inquilino_id = @id";
                    using (var cmdChequeo = new MySqlCommand(sqlChequeo, connection))
                    {
                        cmdChequeo.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmdChequeo.ExecuteScalar() ?? 0);
                        if (count > 0)
                            return 0;
                    }

                    // CORRECCIÓN: La columna es id, no Id
                    string sql = "DELETE FROM inquilino WHERE id = @id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        res = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el inquilino.", ex);
            }
            return res;
        }

        public int Modificacion(Inquilino i)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    // CORRECCIÓN: Nombres de columna en snake_case
                    string sql = @"UPDATE inquilino 
                                   SET nombre=@nombre, apellido=@apellido, dni=@dni, telefono=@telefono, email=@email 
                                   WHERE id = @id"; // WHERE id también corregido
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@nombre", i.Nombre);
                        command.Parameters.AddWithValue("@apellido", i.Apellido);
                        command.Parameters.AddWithValue("@dni", i.DNI);
                        command.Parameters.AddWithValue("@telefono", (object?)i.Telefono ?? DBNull.Value);
                        command.Parameters.AddWithValue("@email", (object?)i.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@id", i.Id);
                        connection.Open();
                        res = command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new Exception("El DNI ya está registrado.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el inquilino.", ex);
            }
            return res;
        }

        public IList<Inquilino> ObtenerTodos()
        {
            var res = new List<Inquilino>();
            try
            {
                using (var connection = GetConnection())
                {
                    // CORRECCIÓN: Uso de alias AS para mapear snake_case a PascalCase
                    string sql = @"SELECT id AS Id, nombre AS Nombre, apellido AS Apellido, dni AS DNI, telefono AS Telefono, email AS Email 
                                   FROM inquilino";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res.Add(new Inquilino
                                {
                                    // La lectura en C# sigue usando PascalCase gracias a los alias AS
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    DNI = reader.GetString("DNI") ?? "",
                                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los inquilinos.", ex);
            }
            return res;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? i = null;
            try
            {
                using (var connection = GetConnection())
                {
                    // CORRECCIÓN: Uso de alias AS para mapear snake_case a PascalCase y WHERE id
                    string sql = @"SELECT id AS Id, nombre AS Nombre, apellido AS Apellido, dni AS DNI, telefono AS Telefono, email AS Email 
                                   FROM inquilino 
                                   WHERE id = @id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                i = new Inquilino
                                {
                                    // La lectura en C# sigue usando PascalCase gracias a los alias AS
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    DNI = reader.GetString("DNI") ?? "",
                                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el inquilino con ID {id}.", ex);
            }
            return i;
        }
    }
}