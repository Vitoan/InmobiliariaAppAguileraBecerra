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
                    string sql = @"INSERT INTO inquilino (Nombre, Apellido, DNI, Telefono, Email)
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
                    string sqlChequeo = "SELECT COUNT(*) FROM contrato WHERE Inquilino_Id = @id";
                    using (var cmdChequeo = new MySqlCommand(sqlChequeo, connection))
                    {
                        cmdChequeo.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmdChequeo.ExecuteScalar() ?? 0);
                        if (count > 0)
                            return 0;
                    }

                    string sql = "DELETE FROM inquilino WHERE Id = @id";
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
                    string sql = @"UPDATE inquilino 
                                   SET Nombre=@nombre, Apellido=@apellido, DNI=@dni, Telefono=@telefono, Email=@email 
                                   WHERE Id = @id";
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
                    string sql = @"SELECT Id, Nombre, Apellido, DNI, Telefono, Email FROM inquilino";
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
                    string sql = @"SELECT Id, Nombre, Apellido, DNI, Telefono, Email FROM inquilino WHERE Id = @id";
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