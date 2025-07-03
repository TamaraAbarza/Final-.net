using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ABM_inmobiliaria.Models
{
    public class RepositorioInquilino
    {
        readonly string ConnectionString = "Server=localhost;Database=inmobiliaria_db;User=root;Password=;";

        public RepositorioInquilino()
        {

        }

        public IList<Inquilino> GetInquilinos()
        {
            var inquilinos = new List<Inquilino>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"SELECT {nameof(Inquilino.Id)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)}, {nameof(Inquilino.Telefono)}, {nameof(Inquilino.Email)}, {nameof(Inquilino.Dni)} FROM Inquilino";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inquilinos.Add(new Inquilino
                            {
                                Id = reader.GetInt32(nameof(Inquilino.Id)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                                Email = reader.GetString(nameof(Inquilino.Email)),
                                Dni = reader.GetString(nameof(Inquilino.Dni))
                            });

                        }
                    }
                }
                connection.Close();
            }

            return inquilinos;

        }

        public Inquilino? GetInquilino(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $"SELECT {nameof(Inquilino.Id)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)}, {nameof(Inquilino.Telefono)}, {nameof(Inquilino.Email)}, {nameof(Inquilino.Dni)} FROM Inquilino WHERE {nameof(Inquilino.Id)} = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Inquilino
                            {
                                Id = reader.GetInt32(nameof(Inquilino.Id)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                                Email = reader.GetString(nameof(Inquilino.Email)),
                                Dni = reader.GetString(nameof(Inquilino.Dni))
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return null;
        }

        public void InsertarInquilino(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"INSERT INTO inquilino({nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)},
			{nameof(Inquilino.Dni)}, {nameof(Inquilino.Telefono)}, {nameof(Inquilino.Email)})
				VALUES (@{nameof(Inquilino.Nombre)}, @{nameof(Inquilino.Apellido)}, @{nameof(Inquilino.Dni)},
				@{nameof(Inquilino.Telefono)}, @{nameof(Inquilino.Email)});";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(@$"{nameof(Inquilino.Nombre)}", inquilino.Nombre);
                    command.Parameters.AddWithValue(@$"{nameof(Inquilino.Apellido)}", inquilino.Apellido);
                    command.Parameters.AddWithValue(@$"{nameof(Inquilino.Dni)}", inquilino.Dni);
                    command.Parameters.AddWithValue(@$"{nameof(Inquilino.Telefono)}", inquilino.Telefono);
                    command.Parameters.AddWithValue(@$"{nameof(Inquilino.Email)}", inquilino.Email);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void ActualizarInquilino(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"UPDATE inquilino SET 
                    {nameof(Inquilino.Nombre)} = @{nameof(Inquilino.Nombre)}, 
                    {nameof(Inquilino.Apellido)} =  @{nameof(Inquilino.Apellido)}, 
                     {nameof(Inquilino.Dni)} =  @{nameof(Inquilino.Dni)}, 
                    {nameof(Inquilino.Telefono)} = @{nameof(Inquilino.Telefono)}, 
                    {nameof(Inquilino.Email)} = @{nameof(Inquilino.Email)} 
                    WHERE {nameof(Inquilino.Id)} = @Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
                      command.Parameters.AddWithValue("@Dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@Telefono", inquilino.Telefono);
                    command.Parameters.AddWithValue("@Email", inquilino.Email);
                    command.Parameters.AddWithValue("@Id", inquilino.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void EliminarInquilino(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"DELETE FROM inquilino WHERE {nameof(Inquilino.Id)} = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

    }
}