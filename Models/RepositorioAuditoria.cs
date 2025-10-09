using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioAuditoria : RepositorioBase
    {
        public int Registrar(Auditoria a)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                // SQL CORREGIDO: 'RegistroId', 'DatosAnteriores', 'DatosNuevos'
                string sql = @"INSERT INTO auditoria (tabla, operacion, RegistroId, DatosAnteriores, DatosNuevos, usuario)
                               VALUES (@tabla, @operacion, @RegistroId, @DatosAnteriores, @DatosNuevos, @usuario)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tabla", a.Tabla);
                    command.Parameters.AddWithValue("@operacion", a.Operacion);
                    command.Parameters.AddWithValue("@RegistroId", a.RegistroId);
                    
                    // Parámetros corregidos
                    command.Parameters.AddWithValue("@DatosAnteriores", a.DatosAnteriores ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DatosNuevos", a.DatosNuevos ?? (object)DBNull.Value);
                    
                    command.Parameters.AddWithValue("@usuario", a.Usuario ?? (object)DBNull.Value);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public IList<Auditoria> ObtenerTodos()
        {
            var res = new List<Auditoria>();
            using (var connection = GetConnection())
            {
                // SQL CORREGIDO: 'RegistroId', 'DatosAnteriores', 'DatosNuevos'
                string sql = @"SELECT id, tabla, operacion, RegistroId, fecha, DatosAnteriores, DatosNuevos, usuario
                               FROM auditoria ORDER BY fecha DESC";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(CrearAuditoriaDesdeReader(reader));
                        }
                    }
                }
            }
            return res;
        }

        public Auditoria? ObtenerPorId(int id)
        {
            Auditoria? a = null;
            using (var connection = GetConnection())
            {
                // SQL CORREGIDO: 'RegistroId', 'DatosAnteriores', 'DatosNuevos'
                string sql = @"SELECT id, tabla, operacion, RegistroId, fecha, DatosAnteriores, DatosNuevos, usuario
                               FROM auditoria WHERE id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            a = CrearAuditoriaDesdeReader(reader);
                        }
                    }
                }
            }
            return a;
        }

        public IList<Auditoria> ObtenerPorTablaYRegistro(string tabla, int registroId)
        {
            var res = new List<Auditoria>();
            using (var connection = GetConnection())
            {
                // SQL CORREGIDO: 'RegistroId', 'DatosAnteriores', 'DatosNuevos'
                string sql = @"SELECT id, tabla, operacion, RegistroId, fecha, DatosAnteriores, DatosNuevos, usuario
                               FROM auditoria
                               WHERE tabla = @tabla AND RegistroId = @RegistroId
                               ORDER BY fecha DESC";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    command.Parameters.AddWithValue("@RegistroId", registroId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(CrearAuditoriaDesdeReader(reader));
                        }
                    }
                }
            }
            return res;
        }

        // --- Método Adicional para el Filtrado Eficiente ---
        public IList<Auditoria> ObtenerConFiltros(string? tabla, string? operacion, string? usuario, DateTime? desde, DateTime? hasta)
        {
            var res = new List<Auditoria>();
            using (var connection = GetConnection())
            {
                string sql = "SELECT id, tabla, operacion, RegistroId, fecha, DatosAnteriores, DatosNuevos, usuario FROM auditoria WHERE 1=1 ";
                
                using (var command = new MySqlCommand(sql, connection))
                {
                    if (!string.IsNullOrEmpty(tabla))
                    {
                        command.CommandText += " AND tabla = @tabla";
                        command.Parameters.AddWithValue("@tabla", tabla);
                    }

                    if (!string.IsNullOrEmpty(operacion))
                    {
                        command.CommandText += " AND operacion = @operacion";
                        command.Parameters.AddWithValue("@operacion", operacion);
                    }

                    if (!string.IsNullOrEmpty(usuario))
                    {
                        command.CommandText += " AND usuario LIKE CONCAT('%', @usuario, '%')";
                        command.Parameters.AddWithValue("@usuario", usuario);
                    }

                    if (desde.HasValue)
                    {
                        command.CommandText += " AND fecha >= @desde";
                        command.Parameters.AddWithValue("@desde", desde.Value.Date);
                    }

                    if (hasta.HasValue)
                    {
                        command.CommandText += " AND fecha < @hasta"; 
                        command.Parameters.AddWithValue("@hasta", hasta.Value.Date.AddDays(1)); 
                    }

                    command.CommandText += " ORDER BY fecha DESC";

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(CrearAuditoriaDesdeReader(reader));
                        }
                    }
                }
            }
            return res;
        }


        // Método auxiliar para evitar código repetido al leer los datos
        private Auditoria CrearAuditoriaDesdeReader(MySqlDataReader reader)
        {
            return new Auditoria
            {
                Id = reader.GetInt32("id"),
                Tabla = reader.GetString("tabla"),
                Operacion = reader.GetString("operacion"),
                RegistroId = reader.GetInt32("RegistroId"),
                Fecha = reader.GetDateTime("fecha"),
                
                // LECTURA CORREGIDA: Usando "DatosAnteriores" y "DatosNuevos"
                DatosAnteriores = reader["DatosAnteriores"] is DBNull ? null : reader.GetString("DatosAnteriores"),
                DatosNuevos = reader["DatosNuevos"] is DBNull ? null : reader.GetString("DatosNuevos"),
                
                Usuario = reader["usuario"] is DBNull ? null : reader.GetString("usuario")
            };
        }
    }
}