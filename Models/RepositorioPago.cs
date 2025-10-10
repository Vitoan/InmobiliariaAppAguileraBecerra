using MySql.Data.MySqlClient;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioPago : RepositorioBase
    {
        public int Alta(Pago p)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = @"INSERT INTO pago (NumeroPago, FechaPago, Importe, Detalle, Anulado, Contrato_Id)
                               VALUES (@numero, @fecha, @importe, @detalle, @anulado, @contratoId);
                               SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@numero", p.Numero);
                    command.Parameters.AddWithValue("@fecha", p.Fecha);
                    command.Parameters.AddWithValue("@importe", p.Importe);
                    command.Parameters.AddWithValue("@detalle", p.Detalle);
                    command.Parameters.AddWithValue("@anulado", p.Anulado);
                    command.Parameters.AddWithValue("@contratoId", p.ContratoId);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.Id = res;
                }
            }
            return res;
        }

        public int Modificacion(Pago p)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = @"UPDATE pago 
                               SET Detalle=@detalle, Anulado=@anulado 
                               WHERE Id=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@detalle", p.Detalle);
                    command.Parameters.AddWithValue("@anulado", p.Anulado);
                    command.Parameters.AddWithValue("@id", p.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                string sql = "UPDATE pago SET Anulado = 1 WHERE Id=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Pago? ObtenerPorId(int id)
        {
            Pago? p = null;
            using (var connection = GetConnection())
            {
                string sql = @"SELECT Id, NumeroPago, FechaPago, Importe, Detalle, Anulado, Contrato_Id 
                               FROM pago WHERE Id=@id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                Numero = reader.GetInt32("NumeroPago"),
                                Fecha = reader.GetDateTime("FechaPago"),
                                Importe = reader.GetDecimal("Importe"),
                                Detalle = reader.GetString("Detalle"),
                                Anulado = reader.GetBoolean("Anulado"),
                                ContratoId = reader.GetInt32("Contrato_Id")
                            };
                        }
                    }
                }
            }
            return p;
        }

        public IList<Pago> ObtenerPorContrato(int contratoId)
        {
            var lista = new List<Pago>();
            using (var connection = GetConnection())
            {
                string sql = @"SELECT Id, NumeroPago, FechaPago, Importe, Detalle, Anulado, Contrato_Id 
                               FROM pago WHERE Contrato_Id=@contratoId ORDER BY NumeroPago;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@contratoId", contratoId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                Numero = reader.GetInt32("NumeroPago"),
                                Fecha = reader.GetDateTime("FechaPago"),
                                Importe = reader.GetDecimal("Importe"),
                                Detalle = reader.GetString("Detalle"),
                                Anulado = reader.GetBoolean("Anulado"),
                                ContratoId = reader.GetInt32("Contrato_Id")
                            });
                        }
                    }
                }
            }
            return lista;
        }
 // ======================================================
        // 🔹 CONTAR PAGOS REALIZADOS DE UN CONTRATO
        // ======================================================
        public int ContarPorContrato(int contratoId)
{
    int cantidad = 0;
    using (var connection = GetConnection())
    {
        string sql = "SELECT COUNT(*) FROM pago WHERE contrato_id=@id";
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", contratoId);
            connection.Open();
            cantidad = Convert.ToInt32(command.ExecuteScalar() ?? 0);
        }
    }
    return cantidad;
}

        public IList<Pago> ObtenerTodos()
        {
            var res = new List<Pago>();
            using (var connection = GetConnection())
            {
                string sql = @"SELECT Id, Contrato_Id, FechaPago, Importe, NumeroPago, Detalle, Anulado
                               FROM pago";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pago = new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                ContratoId = reader.GetInt32("Contrato_Id"),
                                Fecha = reader.GetDateTime("FechaPago"),
                                Importe = reader.GetDecimal("Importe"),
                                Numero = reader.GetInt32("NumeroPago"),
                                Detalle = reader.GetString("Detalle"),
                                Anulado = reader.GetBoolean("Anulado")
                            };
                            res.Add(pago);
                        }
                    }
                }
            }
            return res;
        }
    }
}
