using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using System;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioContrato : RepositorioBase
    {
        // ======================================================
        // 🔹 ALTA DE CONTRATO
        // ======================================================
        public int Alta(Contrato c)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"INSERT INTO contrato 
                                   (FechaInicio, FechaFin, Monto, InquilinoId, InmuebleId, Vigente)
                                   VALUES (@fechaInicio, @fechaFin, @monto, @inquilinoId, @inmuebleId, @vigente);
                                   SELECT LAST_INSERT_ID();";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                        command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
                        command.Parameters.AddWithValue("@monto", c.Monto);
                        command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                        command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                        command.Parameters.AddWithValue("@vigente", c.Vigente);
                        connection.Open();
                        res = Convert.ToInt32(command.ExecuteScalar());
                        c.Id = res;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el contrato.", ex);
            }
            return res;
        }

        // ======================================================
        // 🔹 MODIFICAR CONTRATO
        // ======================================================
        public int Modificacion(Contrato c)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"UPDATE contrato
                                   SET FechaInicio=@fechaInicio, FechaFin=@fechaFin, Monto=@monto, 
                                       InquilinoId=@inquilinoId, InmuebleId=@inmuebleId, Vigente=@vigente,
                                       FechaFinAnticipada=@fechaFinAnticipada, Multa=@multa
                                   WHERE Id = @id;";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                        command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
                        command.Parameters.AddWithValue("@monto", c.Monto);
                        command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                        command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                        command.Parameters.AddWithValue("@vigente", c.Vigente);
                        command.Parameters.AddWithValue("@fechaFinAnticipada", c.FechaFinAnticipada);
                        command.Parameters.AddWithValue("@multa", c.Multa);
                        command.Parameters.AddWithValue("@id", c.Id);
                        connection.Open();
                        res = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el contrato.", ex);
            }
            return res;
        }

        // ======================================================
        // 🔹 BAJA LÓGICA (TERMINAR CONTRATO)
        // ======================================================
        public int Baja(int id)
        {
            int res = -1;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"UPDATE contrato SET Vigente = 0 WHERE Id = @id;";
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
                throw new Exception("Error al dar de baja el contrato.", ex);
            }
            return res;
        }

        // ======================================================
        // 🔹 OBTENER TODOS LOS CONTRATOS
        // ======================================================
        public IList<Contrato> ObtenerTodos()
        {
            var res = new List<Contrato>();
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"
                        SELECT 
                            c.Id AS Id,
                            c.FechaInicio AS FechaInicio,
                            c.FechaFin AS FechaFin,
                            c.Monto AS Monto,
                            c.InquilinoId AS InquilinoId,
                            c.InmuebleId AS InmuebleId,
                            c.Vigente AS Vigente,
                            c.FechaFinAnticipada AS FechaFinAnticipada,
                            c.Multa AS Multa,
                            i.Nombre AS InquilinoNombre,
                            i.Apellido AS InquilinoApellido,
                            inm.Direccion AS InmuebleDireccion
                        FROM contrato c
                        INNER JOIN inquilino i ON c.InquilinoId = i.Id
                        INNER JOIN inmueble inm ON c.InmuebleId = inm.Id;";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var contrato = new Contrato
                                {
                                    Id = reader.GetInt32("Id"),
                                    FechaInicio = reader.GetDateTime("FechaInicio"),
                                    FechaFin = reader.GetDateTime("FechaFin"),
                                    Monto = reader.GetDecimal("Monto"),
                                    InquilinoId = reader.GetInt32("InquilinoId"),
                                    InmuebleId = reader.GetInt32("InmuebleId"),
                                    Vigente = reader.GetBoolean("Vigente"),
                                    FechaFinAnticipada = reader.IsDBNull(reader.GetOrdinal("FechaFinAnticipada"))
                                        ? null : reader.GetDateTime("FechaFinAnticipada"),
                                    Multa = reader.IsDBNull(reader.GetOrdinal("Multa"))
                                        ? null : reader.GetDecimal("Multa"),
                                    Inquilino = new Inquilino
                                    {
                                        Id = reader.GetInt32("InquilinoId"),
                                        Nombre = reader.GetString("InquilinoNombre"),
                                        Apellido = reader.GetString("InquilinoApellido")
                                    },
                                    Inmueble = new Inmueble
                                    {
                                        Id = reader.GetInt32("InmuebleId"),
                                        Direccion = reader.GetString("InmuebleDireccion")
                                    }
                                };
                                res.Add(contrato);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los contratos.", ex);
            }
            return res;
        }

        // ======================================================
        // 🔹 OBTENER CONTRATO POR ID
        // ======================================================
        public Contrato? ObtenerPorId(int id)
        {
            Contrato? c = null;
            try
            {
                using (var connection = GetConnection())
                {
                    string sql = @"
                        SELECT 
                            c.Id AS Id,
                            c.FechaInicio AS FechaInicio,
                            c.FechaFin AS FechaFin,
                            c.Monto AS Monto,
                            c.InquilinoId AS InquilinoId,
                            c.InmuebleId AS InmuebleId,
                            c.Vigente AS Vigente,
                            c.FechaFinAnticipada AS FechaFinAnticipada,
                            c.Multa AS Multa,
                            i.Nombre AS InquilinoNombre,
                            i.Apellido AS InquilinoApellido,
                            inm.Direccion AS InmuebleDireccion,
                            inm.Disponible
                        FROM contrato c
                        INNER JOIN inquilino i ON c.InquilinoId = i.Id
                        INNER JOIN inmueble inm ON c.InmuebleId = inm.Id
                        WHERE c.Id = @id;";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                c = new Contrato
                                {
                                    Id = reader.GetInt32("Id"),
                                    FechaInicio = reader.GetDateTime("FechaInicio"),
                                    FechaFin = reader.GetDateTime("FechaFin"),
                                    Monto = reader.GetDecimal("Monto"),
                                    InquilinoId = reader.GetInt32("InquilinoId"),
                                    InmuebleId = reader.GetInt32("InmuebleId"),
                                    Vigente = reader.GetBoolean("Vigente"),
                                    FechaFinAnticipada = reader.IsDBNull(reader.GetOrdinal("FechaFinAnticipada"))
                                        ? null : reader.GetDateTime("FechaFinAnticipada"),
                                    Multa = reader.IsDBNull(reader.GetOrdinal("Multa"))
                                        ? null : reader.GetDecimal("Multa"),
                                    Inquilino = new Inquilino
                                    {
                                        Id = reader.GetInt32("InquilinoId"),
                                        Nombre = reader.GetString("InquilinoNombre") ?? "",
                                        Apellido = reader.GetString("InquilinoApellido") ?? ""
                                    },
                                    Inmueble = new Inmueble
                                    {
                                        Id = reader.GetInt32("InmuebleId"),
                                        Direccion = reader.GetString("InmuebleDireccion") ?? "",
                                        Disponible = reader.GetBoolean("Disponible")
                                    }
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el contrato con ID {id}.", ex);
            }
            return c;
        }
    }
}
