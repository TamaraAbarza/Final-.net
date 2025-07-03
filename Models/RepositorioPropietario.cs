using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ABM_inmobiliaria.Models
{
    public class RepositorioPropietario
    {
        readonly string ConnectionString = "Server=localhost;Database=inmobiliaria_db;User=root;Password=;";

        public RepositorioPropietario()
        {

        }

        public IList<Propietario> GetPropietarios()
        {
            var propietarios = new List<Propietario>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"SELECT {nameof(Propietario.Id)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)} FROM Propietario";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            propietarios.Add(new Propietario
                            {
                                Id = reader.GetInt32(nameof(Propietario.Id)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                Dni = reader.GetString(nameof(Propietario.Dni)),
                                Telefono = reader.GetString(nameof(Propietario.Telefono)),
                                Email = reader.GetString(nameof(Propietario.Email))
                            });

                        }
                    }
                }
                connection.Close();
            }

            return propietarios;

        }

        public Propietario? GetPropietario(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $"SELECT {nameof(Propietario.Id)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)} FROM Propietario WHERE {nameof(Propietario.Id)} = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Propietario
                            {
                                Id = reader.GetInt32(nameof(Propietario.Id)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                Dni = reader.GetString(nameof(Propietario.Dni)),
                                Telefono = reader.GetString(nameof(Propietario.Telefono)),
                                Email = reader.GetString(nameof(Propietario.Email))
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return null;
        }

        public void InsertarPropietario(Propietario propietario)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                //var sql = "INSERT INTO propietario (Nombre, Apellido, Dni, Telefono, Email) VALUES (@Nombre, @Apellido, @Dni, @Telefono, @Email)";
                var sql = @$"INSERT INTO propietario({nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)},
			{nameof(Propietario.Telefono)}, {nameof(Propietario.Email)})
				VALUES (@{nameof(Propietario.Nombre)}, @{nameof(Propietario.Apellido)}, @{nameof(Propietario.Dni)},
				@{nameof(Propietario.Telefono)}, @{nameof(Propietario.Email)});
				SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(@$"{nameof(Propietario.Nombre)}", propietario.Nombre);
                    command.Parameters.AddWithValue(@$"{nameof(Propietario.Apellido)}", propietario.Apellido);
                    command.Parameters.AddWithValue(@$"{nameof(Propietario.Dni)}", propietario.Dni);
                    command.Parameters.AddWithValue(@$"{nameof(Propietario.Telefono)}", propietario.Telefono);
                    command.Parameters.AddWithValue(@$"{nameof(Propietario.Email)}", propietario.Email);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void ActualizarPropietario(Propietario propietario)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"UPDATE propietario SET 
                    {nameof(Propietario.Nombre)} = @{nameof(Propietario.Nombre)}, 
                    {nameof(Propietario.Apellido)} =  @{nameof(Propietario.Apellido)}, 
                    {nameof(Propietario.Dni)} = @{nameof(Propietario.Dni)}, 
                    {nameof(Propietario.Telefono)} = @{nameof(Propietario.Telefono)}, 
                    {nameof(Propietario.Email)} = @{nameof(Propietario.Email)} 
                    WHERE {nameof(Propietario.Id)} = @Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@Dni", propietario.Dni);
                    command.Parameters.AddWithValue("@Telefono", propietario.Telefono);
                    command.Parameters.AddWithValue("@Email", propietario.Email);
                    command.Parameters.AddWithValue("@Id", propietario.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void EliminarPropietario(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"DELETE FROM propietario WHERE {nameof(Propietario.Id)} = @Id";

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