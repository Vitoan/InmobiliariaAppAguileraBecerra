using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using InmobiliariaApp.Models; // Usamos el namespace de tu modelo
using System.Data;

namespace InmobiliariaApp.Repositorios
{
    // Repositorio que maneja las operaciones de la base de datos para Propietario de forma directa con SQL.
    public class RepositorioPropietario
    {
        // Cadena de conexión hardcodeada para MySQL en XAMPP.
        private readonly string _connectionString = "Server=localhost;Database=inmobiliaria_db;Uid=root;Pwd=;";

        // Método para registrar un nuevo propietario (Alta).
        public int Alta(Propietario p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(_connectionString))
            {
                // Usamos LAST_INSERT_ID() para obtener el ID del registro recién insertado en MySQL.
                string sql = @"INSERT INTO propietario (Nombre, Apellido, Dni, Telefono, Email, Clave)
                           VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave);
                           SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.DNI); // Usamos DNI del nuevo modelo
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@clave", p.Clave);

                    connection.Open();
                    // ExecuteScalar se usa para consultas que devuelven un único valor.
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.Id = res; // Asignamos el ID al nuevo modelo
                }
            }
            return res;
        }
        
        // Método para eliminar un propietario (Baja).
        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM propietario WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Método para modificar un propietario (Modificación).
        public int Modificacion(Propietario p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = @"UPDATE propietario
                           SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, Clave=@clave
                           WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.DNI); // Usamos DNI del nuevo modelo
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@clave", p.Clave);
                    command.Parameters.AddWithValue("@id", p.Id); // Usamos Id del nuevo modelo

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Método para obtener todos los propietarios.
        public IList<Propietario> ObtenerTodos()
        {
            var res = new List<Propietario>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave
                           FROM propietario";
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
                                Id = reader.GetInt32("Id"), // Mapeamos Id a Id
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("Dni"), // Mapeamos Dni a DNI
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                                Clave = reader.IsDBNull(reader.GetOrdinal("Clave")) ? null : reader.GetString("Clave")
                            });
                        }
                    }
                }
            }
            return res;
        }

        // Método para obtener una lista paginada de propietarios.
        public IList<Propietario> ObtenerLista(int paginaNro, int tamPagina)
        {
            var res = new List<Propietario>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                // Usamos OFFSET y LIMIT para la paginación en MySQL.
                string sql = @"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave
                           FROM propietario
                           ORDER BY Id
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
                            res.Add(new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("Dni"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                                Clave = reader.IsDBNull(reader.GetOrdinal("Clave")) ? null : reader.GetString("Clave")
                            });
                        }
                    }
                }
            }
            return res;
        }

        // Método para obtener la cantidad total de propietarios.
        public int ObtenerCantidad()
        {
            int res = 0;
            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT COUNT(Id) FROM propietario";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }

        // Método para obtener un propietario por su ID.
        public Propietario ObtenerPorId(int id)
        {
            Propietario p = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave
                           FROM propietario
                           WHERE Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("Dni"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                                Clave = reader.IsDBNull(reader.GetOrdinal("Clave")) ? null : reader.GetString("Clave")
                            };
                        }
                    }
                }
            }
            return p;
        }

        // Método para obtener un propietario por su dirección de correo electrónico.
        public Propietario ObtenerPorEmail(string email)
        {
            Propietario p = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave
                           FROM propietario
                           WHERE Email=@email";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("Dni"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                                Clave = reader.IsDBNull(reader.GetOrdinal("Clave")) ? null : reader.GetString("Clave")
                            };
                        }
                    }
                }
            }
            return p;
        }

        // Método para buscar propietarios por nombre o apellido.
        public IList<Propietario> BuscarPorNombre(string nombre)
        {
            List<Propietario> res = new List<Propietario>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                // Usamos el comodín % para la búsqueda de cadenas.
                string sql = @"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave
                           FROM propietario
                           WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", "%" + nombre + "%");
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("Dni"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                                Clave = reader.IsDBNull(reader.GetOrdinal("Clave")) ? null : reader.GetString("Clave")
                            });
                        }
                    }
                }
            }
            return res;
        }
    }
}
