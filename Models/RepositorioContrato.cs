using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ABM_inmobiliaria.Models
{
    public class RepositorioContrato
    {
        public RepositorioContrato() { }

        readonly string ConnectionString =
            "Server=localhost;Database=inmobiliaria_db;User=root;Password=;";

        public IList<Contrato> GetContratos(
    DateTime? fechaInicio = null,
    DateTime? fechaFin = null,
    int? idInmueble = null
)
        {
            IList<Contrato> listaContratos = new List<Contrato>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT c.Id, idInmueble, idInquilino, c.idPropietario, c.idUsuario, vigente, montoMensual, fechaInicio, fechaFin,
                        i.nombre AS nombreInquilino, i.apellido AS apellidoInquilino,
                        p.nombre AS nombrePropietario, p.apellido AS apellidoPropietario,
                        u.nombre AS nombreUsuario, u.apellido AS apellidoUsuario,
                        inm.direccion AS direccionInmueble, inm.ambientes AS ambientes
                FROM contrato c
                INNER JOIN inquilino i ON c.idInquilino = i.id
                INNER JOIN propietario p ON c.idPropietario = p.id
                INNER JOIN usuario u ON c.idUsuario = u.id
                INNER JOIN inmueble inm ON c.idInmueble = inm.id
                WHERE (@FechaInicio IS NULL OR c.fechaInicio <= @FechaFin)
                  AND (@FechaFin IS NULL OR c.fechaFin >= @FechaInicio)
                  AND (@IdInmueble IS NULL OR c.idInmueble = @IdInmueble)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@FechaInicio",
                        fechaInicio.HasValue ? (object)fechaInicio.Value : DBNull.Value
                    );
                    command.Parameters.AddWithValue(
                        "@FechaFin",
                        fechaFin.HasValue ? (object)fechaFin.Value : DBNull.Value
                    );
                    command.Parameters.AddWithValue(
                        "@IdInmueble",
                        idInmueble.HasValue ? (object)idInmueble.Value : DBNull.Value
                    );

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contrato contrato = new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaFin = reader.GetDateTime("fechaFin"),
                                FechaInicio = reader.GetDateTime("fechaInicio"),
                                IdUsuario = reader.GetInt32("idUsuario"),
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("idInquilino"),
                                    Nombre = reader.GetString("nombreInquilino"),
                                    Apellido = reader.GetString("apellidoInquilino"),
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("idPropietario"),
                                    Nombre = reader.GetString("nombrePropietario"),
                                    Apellido = reader.GetString("apellidoPropietario")
                                },
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("idInmueble"),
                                    Direccion = reader.GetString("direccionInmueble"),
                                    Ambientes = reader.GetInt32("ambientes")
                                },
                                Usuario = new Usuario
                                {
                                    Id = reader.GetInt32("idUsuario"),
                                    Nombre = reader.GetString("nombreUsuario"),
                                    Apellido = reader.GetString("apellidoUsuario")
                                },
                                Vigente = reader.GetBoolean("vigente"),
                                MontoMensual = reader.GetDouble("montoMensual")
                            };
                            listaContratos.Add(contrato);
                        }
                    }
                    connection.Close();
                }
            }
            return listaContratos;
        }

        public int InsertarContrato(Contrato contrato)
        {
            int idContrato = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql =
                    @"INSERT INTO contrato (idInquilino, idInmueble, idPropietario, fechaInicio, fechaFin, montoMensual, idUsuario, vigente)
              VALUES (@IdInquilino, @IdInmueble, @IdPropietario, @FechaInicio, @FechaFin, @MontoMensual, @IdUsuario, @Vigente);
              SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdPropietario", contrato.IdPropietario);
                    command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
                    command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
                    command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                    command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@IdUsuario", contrato.IdUsuario);
                    command.Parameters.AddWithValue("@Vigente", 1);
                    connection.Open();
                    idContrato = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return idContrato;
        }

        public Contrato? GetContrato(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT c.Id, idInmueble, idInquilino, c.idPropietario, vigente, montoMensual, fechaInicio, fechaFin,
                                i.nombre AS nombreInquilino,i.apellido AS apellidoInquilino,
                                p.nombre AS nombrePropietario, p.apellido AS apellidoPropietario,
                                inm.direccion AS direccionInmueble, inm.ambientes AS ambientes
                            FROM contrato c INNER JOIN inquilino i ON c.idInquilino = i.id
                                            INNER JOIN propietario p ON c.idPropietario = p.id
                                            INNER JOIN inmueble inm ON c.idInmueble = inm.id
                            WHERE c.Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaFin = reader.GetDateTime("fechaFin"),
                                FechaInicio = reader.GetDateTime("fechaInicio"),
                                IdInquilino = reader.GetInt32("idInquilino"),
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("idInquilino"),
                                    Nombre = reader.GetString("nombreInquilino"),
                                    Apellido = reader.GetString("apellidoInquilino"),
                                },
                                IdPropietario = reader.GetInt32("idPropietario"),
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("idPropietario"),
                                    Nombre = reader.GetString("nombrePropietario"),
                                    Apellido = reader.GetString("apellidoPropietario")
                                },
                                IdInmueble = reader.GetInt32("idInmueble"),
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("idInmueble"),
                                    Direccion = reader.GetString("direccionInmueble"),
                                    Ambientes = reader.GetInt32("ambientes")
                                },
                                Vigente = reader.GetBoolean("vigente"),
                                MontoMensual = reader.GetDouble("montoMensual")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void ActualizarContrato(Contrato contrato)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql =
                    @"UPDATE contrato
                    SET idInmueble = @IdInmueble, 
                        idInquilino = @IdInquilino, 
                        idPropietario = @IdPropietario, 
                        vigente = @Vigente, 
                        montoMensual = @MontoMensual, 
                        fechaInicio = @FechaInicio, 
                        fechaFin = @FechaFin
                    WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", contrato.Id);
                    command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
                    command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
                    command.Parameters.AddWithValue("@IdPropietario", contrato.IdPropietario);
                    command.Parameters.AddWithValue("@Vigente", contrato.Vigente);
                    command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarContrato(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"DELETE FROM contrato WHERE {nameof(Contrato.Id)} = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        //metodo para controlar el inmueble no este ocupado por otro contrato en las mismas fechas
        public bool InmuebleOcupadoEnOtroContrato(
            int idInmueble,
            DateTime fechaInicio,
            int idContrato
        )
        {
            // Consulta para verificar si hay algún contrato que coincida con las fechas del contrato nuevo
            string sql =
                @"SELECT COUNT(*) FROM contrato 
                   WHERE idInmueble = @IdInmueble 
                   AND fechaFin >= @FechaInicio
                   AND id != @IdContrato
                   AND vigente = 1";

            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdInmueble", idInmueble);
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@IdContrato", idContrato);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // Si count es mayor que 0, significa que el inmueble está ocupado en otro contrato con fecha de fin después de la fecha de inicio del contrato nuevo
                    return count > 0;
                }
            }
        }

        //Adicional

        // Método para obtener los pagos de un contrato
        public IList<Pago> ObtenerPagos(int idContrato)
        {
            IList<Pago> listaPagos = new List<Pago>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT id, idContrato, fecha, importe
                    FROM pago
                    WHERE idContrato = @IdContrato";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", idContrato);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pago pago = new Pago
                            {
                                Id = reader.GetInt32("id"),
                                IdContrato = reader.GetInt32("idContrato"),
                                Fecha = reader.GetDateTime("fecha"),
                                Importe = reader.GetDouble("importe")
                            };
                            listaPagos.Add(pago);
                        }
                    }
                    connection.Close();
                }
            }
            return listaPagos;
        }
    }
}
