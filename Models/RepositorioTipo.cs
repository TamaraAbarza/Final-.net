using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ABM_inmobiliaria.Models
{
    public class RepositorioTipo
    {

        readonly string ConnectionString = "Server=localhost;Database=inmobiliaria_db;User=root;Password=;";
        public RepositorioTipo()
        {
        }

        public IList<Tipo> GetTipo()
        {
            var listaTipos = new List<Tipo>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @"SELECT id, tipoInmueble FROM Tipo";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaTipos.Add(new Tipo
                            {
                                Id = reader.GetInt32("id"),
                                TipoInmueble = reader.GetString("tipoInmueble"),
                            });

                        }
                    }
                }
                connection.Close();
            }

            return listaTipos;

        }

        public void InsertarTipo(Tipo tipo)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO tipo (tipoInmueble)
                VALUES (@tipoInmueble)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tipoInmueble", tipo.TipoInmueble);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public Tipo? GetTipo(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @"SELECT tipoInmueble
                            FROM tipo 
                            WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Tipo
                            {
                                TipoInmueble = reader.GetString("tipoInmueble")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void ActualizarTipo(Tipo tipo)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE tipo SET tipoInmueble = @tipoInmueble
                 WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", tipo.Id);
                    command.Parameters.AddWithValue("@tipoInmueble", tipo.TipoInmueble);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarTipo(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"DELETE FROM tipo WHERE {nameof(Tipo.Id)} = @Id";

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