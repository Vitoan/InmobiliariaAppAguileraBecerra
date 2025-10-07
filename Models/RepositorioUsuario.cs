using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioUsuario : RepositorioBase
    {
        public int Alta(Usuario e)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"INSERT INTO usuario 
                                   (Nombre, Apellido, Avatar, Email, Clave, Rol)
                                   VALUES (@nombre, @apellido, @avatar, @email, @clave, @rol);
                                   SELECT LAST_INSERT_ID();";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@nombre", e.Nombre);
                        command.Parameters.AddWithValue("@apellido", e.Apellido);
                        command.Parameters.AddWithValue("@avatar", string.IsNullOrEmpty(e.Avatar) ? DBNull.Value : e.Avatar);
                        command.Parameters.AddWithValue("@email", e.Email);
                        command.Parameters.AddWithValue("@clave", e.Clave);
                        command.Parameters.AddWithValue("@rol", e.Rol);

                        connection.Open();
                        res = Convert.ToInt32(command.ExecuteScalar() ?? 0);
                        e.Id = res;
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new Exception("El email ya está registrado.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el usuario.", ex);
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
                    string sql = "DELETE FROM usuario WHERE Id=@id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        res = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el usuario.", ex);
            }
            return res;
        }

        public int Modificacion(Usuario e)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"UPDATE usuario 
                                   SET Nombre=@nombre, Apellido=@apellido, Avatar=@avatar, Email=@email, Clave=@clave, Rol=@rol
                                   WHERE Id=@id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@nombre", e.Nombre);
                        command.Parameters.AddWithValue("@apellido", e.Apellido);
                        command.Parameters.AddWithValue("@avatar", string.IsNullOrEmpty(e.Avatar) ? DBNull.Value : e.Avatar);
                        command.Parameters.AddWithValue("@email", e.Email);
                        command.Parameters.AddWithValue("@clave", e.Clave);
                        command.Parameters.AddWithValue("@rol", e.Rol);
                        command.Parameters.AddWithValue("@id", e.Id);

                        connection.Open();
                        res = command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new Exception("El email ya está registrado.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el usuario.", ex);
            }
            return res;
        }

        public IList<Usuario> ObtenerTodos()
        {
            var res = new List<Usuario>();
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol 
                                   FROM usuario
                                   ORDER BY Id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res.Add(new Usuario
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    Avatar = reader["Avatar"] is DBNull ? null : reader.GetString("Avatar"),
                                    Email = reader.GetString("Email") ?? "",
                                    Clave = reader.GetString("Clave") ?? "",
                                    Rol = reader.GetInt32("Rol")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios.", ex);
            }
            return res;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? e = null;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol 
                                   FROM usuario
                                   WHERE Id=@id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                e = new Usuario
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    Avatar = reader["Avatar"] is DBNull ? null : reader.GetString("Avatar"),
                                    Email = reader.GetString("Email") ?? "",
                                    Clave = reader.GetString("Clave") ?? "",
                                    Rol = reader.GetInt32("Rol")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el usuario con ID {id}.", ex);
            }
            return e;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? e = null;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol 
                                   FROM usuario
                                   WHERE Email=@Email";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                e = new Usuario
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre") ?? "",
                                    Apellido = reader.GetString("Apellido") ?? "",
                                    Avatar = reader["Avatar"] is DBNull ? null : reader.GetString("Avatar"),
                                    Email = reader.GetString("Email") ?? "",
                                    Clave = reader.GetString("Clave") ?? "",
                                    Rol = reader.GetInt32("Rol")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el usuario con Email {email}.", ex);
            }
            return e;
        }
    }
}
