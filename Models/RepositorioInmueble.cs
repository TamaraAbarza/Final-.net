using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ABM_inmobiliaria.Models
{
    public class RepositorioInmueble
    {
        public RepositorioInmueble() { }

        readonly string ConnectionString =
            "Server=localhost;Database=inmobiliaria_db;User=root;Password=;";

        public IList<Inmueble> GetInmuebles()
        {
            IList<Inmueble> listaInmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT i.Id, i.Direccion, i.Ambientes, i.Superficie, i.Latitud, i.Longitud, i.IdPropietario,
                i.Disponible, i.IdTipo, i.Uso, i.Precio, p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario,
                t.id AS IdTipoInmueble, t.tipoInmueble AS TipoInmueble
                FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id INNER JOIN tipo t ON i.IdTipo = t.id ORDER BY i.Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Inmueble inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Superficie = reader.GetInt32("Superficie"),
                                Latitud = reader.GetDouble("Latitud"),
                                Longitud = reader.GetDouble("Longitud"),
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Uso = reader.GetString("Uso"),
                                Precio = reader.GetDouble("Precio"),
                                Tipo = new Tipo
                                {
                                    Id = reader.GetInt32("IdTipoInmueble"),
                                    TipoInmueble = reader.GetString("TipoInmueble")
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("IdPropietario"),
                                    Nombre = reader.GetString("NombrePropietario"),
                                    Apellido = reader.GetString("ApellidoPropietario"),
                                }
                            };
                            listaInmuebles.Add(inmueble);
                        }
                        connection.Close();
                    }
                }
                return listaInmuebles;
            }
        }

        public Inmueble? GetInmueble(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT i.Id, i.Direccion, i.Ambientes, i.Superficie, i.Latitud, i.Longitud, i.IdPropietario,
                i.Disponible, i.IdTipo, i.Uso, i.Precio, p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario, t.id AS IdTipoInmueble, t.tipoInmueble AS TipoInmueble
                FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id INNER JOIN tipo t ON i.IdTipo = t.id WHERE i.Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Superficie = reader.GetInt32("Superficie"),
                                Latitud = reader.GetDouble("Latitud"),
                                Longitud = reader.GetDouble("Longitud"),
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Uso = reader.GetString("Uso"),
                                Precio = reader.GetDouble("Precio"),
                                IdTipo = reader.GetInt32("IdTipo"),
                                Tipo = new Tipo
                                {
                                    Id = reader.GetInt32("IdTipoInmueble"),
                                    TipoInmueble = reader.GetString("TipoInmueble")
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("IdPropietario"),
                                    Nombre = reader.GetString("NombrePropietario"),
                                    Apellido = reader.GetString("ApellidoPropietario"),
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void InsertarInmueble(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql =
                    @"INSERT INTO Inmueble (Direccion, Ambientes, Superficie, Latitud, Longitud, IdPropietario, Disponible, IdTipo, Uso, Precio)
                VALUES (@Direccion, @Ambientes, @Superficie, @Latitud, @Longitud, @IdPropietario, @Disponible, @IdTipo, @Uso, @Precio)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@Latitud", inmueble.Latitud);
                    command.Parameters.AddWithValue("@Longitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@IdTipo", inmueble.IdTipo);
                    command.Parameters.AddWithValue("@Disponible", true);
                    command.Parameters.AddWithValue("@Uso", inmueble.Uso);
                    command.Parameters.AddWithValue("@Precio", inmueble.Precio);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarInmueble(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql =
                    @"UPDATE Inmueble SET Direccion = @Direccion, Ambientes = @Ambientes, Superficie = @Superficie,
                      Latitud = @Latitud, Longitud = @Longitud, IdPropietario = @IdPropietario, Disponible = @Disponible,
                      IdTipo = @IdTipo, Uso = @Uso, Precio = @Precio WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@Latitud", inmueble.Latitud);
                    command.Parameters.AddWithValue("@Longitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@Disponible", inmueble.Disponible);
                    command.Parameters.AddWithValue("@IdTipo", inmueble.IdTipo);
                    command.Parameters.AddWithValue("@Uso", inmueble.Uso);
                    command.Parameters.AddWithValue("@Precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@Id", inmueble.Id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ModificarEstadoInmueble(int id, bool disponible)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE Inmueble SET Disponible = @Disponible WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Disponible", disponible);
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarInmueble(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = "DELETE FROM Inmueble WHERE {nameof(Inmueble.Id)} = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        //LISTAR
        public IList<Inmueble> GetInmueblesDisponibles()
        {
            IList<Inmueble> listaInmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT i.Id, i.Direccion, i.Ambientes, i.Superficie, i.Latitud, i.Longitud, i.IdPropietario,
                    i.Disponible, i.IdTipo, i.Uso, i.Precio, p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario,
                    t.id AS IdTipoInmueble, t.tipoInmueble AS TipoInmueble
                    FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id 
                    INNER JOIN tipo t ON i.IdTipo = t.id 
                    WHERE i.Disponible = 1 
                    ORDER BY i.Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Inmueble inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Superficie = reader.GetInt32("Superficie"),
                                Latitud = reader.GetDouble("Latitud"),
                                Longitud = reader.GetDouble("Longitud"),
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Uso = reader.GetString("Uso"),
                                Precio = reader.GetDouble("Precio"),
                                Tipo = new Tipo
                                {
                                    Id = reader.GetInt32("IdTipoInmueble"),
                                    TipoInmueble = reader.GetString("TipoInmueble")
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("IdPropietario"),
                                    Nombre = reader.GetString("NombrePropietario"),
                                    Apellido = reader.GetString("ApellidoPropietario"),
                                }
                            };
                            listaInmuebles.Add(inmueble);
                        }
                        connection.Close();
                    }
                }
                return listaInmuebles;
            }
        }

        public IList<Inmueble> GetInmueblesPorPropietario(
            int propietarioId,
            bool? disponible = null
        )
        {
            IList<Inmueble> listaInmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT i.Id, i.Direccion, i.Ambientes, i.Superficie, i.Latitud, i.Longitud, i.IdPropietario,
                    i.Disponible, i.IdTipo, i.Uso, i.Precio, p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario,
                    t.id AS IdTipoInmueble, t.tipoInmueble AS TipoInmueble
                    FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id 
                    INNER JOIN tipo t ON i.IdTipo = t.id 
                    WHERE i.IdPropietario = @IdPropietario";

                if (disponible.HasValue)
                {
                    sql += " AND i.Disponible = @Disponible";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdPropietario", propietarioId);
                    if (disponible.HasValue)
                    {
                        command.Parameters.AddWithValue("@Disponible", disponible.Value);
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Inmueble inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Superficie = reader.GetInt32("Superficie"),
                                Latitud = reader.GetDouble("Latitud"),
                                Longitud = reader.GetDouble("Longitud"),
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Uso = reader.GetString("Uso"),
                                Precio = reader.GetDouble("Precio"),
                                Tipo = new Tipo
                                {
                                    Id = reader.GetInt32("IdTipoInmueble"),
                                    TipoInmueble = reader.GetString("TipoInmueble")
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("IdPropietario"),
                                    Nombre = reader.GetString("NombrePropietario"),
                                    Apellido = reader.GetString("ApellidoPropietario"),
                                }
                            };
                            listaInmuebles.Add(inmueble);
                        }
                        connection.Close();
                    }
                }
                return listaInmuebles;
            }
        }

        public List<Inmueble> GetInmueblesDisponiblesPorFecha(
            DateTime fechaInicio,
            DateTime fechaFin
        )
        {
            var inmueblesDisponibles = new List<Inmueble>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (
                    var command = new MySqlCommand(
                        @"
                        SELECT i.*, t.tipoInmueble, p.Nombre AS NombrePropietario, p.Apellido AS ApellidoPropietario
                        FROM Inmueble i
                        JOIN Tipo t ON i.idTipo = t.id
                        JOIN Propietario p ON i.idPropietario = p.id
                        AND NOT EXISTS (
                            SELECT 1
                            FROM Contrato c
                            WHERE c.idInmueble = i.id
                            AND c.fechaInicio <= @fechaFin
                            AND c.fechaFin >= @fechaInicio
                        )",
                        connection
                    )
                )
                {
                    command.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", fechaFin);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("id"),
                                Direccion = reader.GetString("direccion"),
                                Ambientes = reader.GetInt32("ambientes"),
                                Superficie = reader.GetInt32("superficie"),
                                Latitud = reader.GetDouble("latitud"),
                                Longitud = reader.GetDouble("longitud"),
                                IdPropietario = reader.GetInt32("idPropietario"),
                                Disponible = reader.GetBoolean("disponible"),
                                Uso = reader.GetString("uso"),
                                Precio = reader.GetDouble("precio"),
                                IdTipo = reader.GetInt32("idTipo"),
                                Tipo = new Tipo
                                {
                                    Id = reader.GetInt32("idTipo"),
                                    TipoInmueble = reader.GetString("tipoInmueble")
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("idPropietario"),
                                    Nombre = reader.GetString("NombrePropietario"),
                                    Apellido = reader.GetString("ApellidoPropietario"),
                                }
                            };
                            inmueblesDisponibles.Add(inmueble);
                        }
                    }
                }
            }

            return inmueblesDisponibles;
        }
    }
}
