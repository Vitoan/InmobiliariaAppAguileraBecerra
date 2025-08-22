using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class RepositorioInquilino : RepositorioBase
	{
		public RepositorioInquilino(IConfiguration configuration) : base(configuration)
		{

		}


        public int Alta(Inquilino i){
            int res = -1;
            using (SqlConnection connection = new Connection(connectionString))
            {
                string sql = @"INSERT INTO inquilino 
					(Nombre, Apellido, DNI, Telefono, Email)
					VALUES (@nombre, @apellido, @dni, @telefono, @email);
					SELECT SCOPE_IDENTITY();";

                using(SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", i.nombre);
                    command.Parameters.AddWithValue("@apellido", i.apellido);
                    command.Parameters.AddWithValue("@DNI", i.DNI);
                    command.Parameters.AddWithValue("@telefono", i.telefono);
                    command.Parameters.AddWithValue("@email", i.email);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.Id = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id){
            int res = -1;
            using (SqlConnection connection = new Connection(connectionString))
            {
                string sql = "DELETE FROM inquilino WHERE Id = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Inquilino i){
            int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @"UPDATE Propietarios 
					SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, Clave=@clave
					WHERE IdPropietario = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", i.nombre);
					command.Parameters.AddWithValue("@apellido", i.apellido);
					command.Parameters.AddWithValue("@dni", i.DNI);
					command.Parameters.AddWithValue("@telefono", i.telefono);
					command.Parameters.AddWithValue("@email", i.email);
					command.Parameters.AddWithValue("@id", i.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
            return res;
        }

        public IList<Inquilino> ObtenerTodos()
		{
			IList<Inquilino> res = new List<Inquilino>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @"SELECT 
					Id, nombre, apellido, DNI, telefono, email
					FROM inquilino";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino i = new Inquilino
						{
							Id = reader.GetInt32(nameof(Inquilino.Id)),
							Nombre = reader.GetString("nombre"),
							Apellido = reader.GetString("apellido"),
							Dni = reader.GetString("DNI"),
							Telefono = reader.GetString("telefono"),
							Email = reader.GetString("email")
						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}