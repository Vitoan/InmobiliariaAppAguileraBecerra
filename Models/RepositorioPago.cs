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
                string sql = @"INSERT INTO pago (Numero, Fecha, Importe, Detalle, Anulado, ContratoId)
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
                string sql = @"SELECT Id, Numero, Fecha, Importe, Detalle, Anulado, ContratoId 
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
                                Numero = reader.GetInt32("Numero"),
                                Fecha = reader.GetDateTime("Fecha"),
                                Importe = reader.GetDecimal("Importe"),
                                Detalle = reader.GetString("Detalle"),
                                Anulado = reader.GetBoolean("Anulado"),
                                ContratoId = reader.GetInt32("ContratoId")
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
                string sql = @"SELECT Id, Numero, Fecha, Importe, Detalle, Anulado, ContratoId 
                               FROM pago WHERE ContratoId=@contratoId ORDER BY Numero;";
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
                                Numero = reader.GetInt32("Numero"),
                                Fecha = reader.GetDateTime("Fecha"),
                                Importe = reader.GetDecimal("Importe"),
                                Detalle = reader.GetString("Detalle"),
                                Anulado = reader.GetBoolean("Anulado"),
                                ContratoId = reader.GetInt32("ContratoId")
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}
