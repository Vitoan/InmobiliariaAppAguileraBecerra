using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioPropietario : RepositorioBase
    {
        public int Alta(Propietario p)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"INSERT INTO propietario (Nombre, Apellido, DNI, Telefono, Email, Clave)
                                   VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave);
                                   SELECT LAST_INSERT_ID();";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@nombre", p.Nombre);
                        command.Parameters.AddWithValue("@apellido", p.Apellido);
                        command.Parameters.AddWithValue("@dni", p.DNI);
                        command.Parameters.AddWithValue("@telefono", p.Telefono);
                        command.Parameters.AddWithValue("@email", p.Email);
                        command.Parameters.AddWithValue("@clave", p.Clave);
                        connection.Open();
                        res = Convert.ToInt32(command.ExecuteScalar() ?? 0);
                        p.Id = res;
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new Exception("El DNI ya está registrado.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el propietario.", ex);
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
                    string sqlChequeo = "SELECT COUNT(*) FROM inmueble WHERE PropietarioId = @id";
                    using (var cmdChequeo = new MySqlCommand(sqlChequeo, connection))
                    {
                        cmdChequeo.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmdChequeo.ExecuteScalar() ?? 0);
                        if (count > 0)
                            return 0;
                    }

                    string sqlDelete = "DELETE FROM propietario WHERE Id = @id";
                    using (var command = new MySqlCommand(sqlDelete, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        res = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el propietario.", ex);
            }
            return res;
        }

        public int Modificacion(Propietario p)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"UPDATE propietario
                                   SET Nombre=@nombre, Apellido=@apellido, DNI=@dni, Telefono=@telefono, Email=@email, Clave=@clave
                                   WHERE Id = @id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@nombre", p.Nombre);
                        command.Parameters.AddWithValue("@apellido", p.Apellido);
                        command.Parameters.AddWithValue("@dni", p.DNI);
                        command.Parameters.AddWithValue("@telefono", p.Telefono);
                        command.Parameters.AddWithValue("@email", p.Email);
                        command.Parameters.AddWithValue("@clave", p.Clave);
                        command.Parameters.AddWithValue("@id", p.Id);
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
                throw new Exception("Error al modificar el propietario.", ex);
            }
            return res;
        }

        public IList<Propietario> ObtenerTodos()
        {
            var res = new List<Propietario>();
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"SELECT Id, Nombre, Apellido, DNI, Telefono, Email, Clave FROM propietario";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res.Add(new Propietario
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    DNI = reader.GetString("DNI") ?? "",
                                    Telefono = reader.GetString("Telefono") ?? "",
                                    Email = reader.GetString("Email") ?? "",
                                    Clave = reader.GetString("Clave") ?? ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los propietarios.", ex);
            }
            return res;
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? p = null;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"SELECT Id, Nombre, Apellido, DNI, Telefono, Email, Clave FROM propietario WHERE Id=@id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                p = new Propietario
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    DNI = reader.GetString("DNI") ?? "",
                                    Telefono = reader.GetString("Telefono") ?? "",
                                    Email = reader.GetString("Email") ?? "",
                                    Clave = reader.GetString("Clave") ?? ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el propietario con ID {id}.", ex);
            }
            return p;
        }
    }
}