using MySql.Data.MySqlClient;

namespace ABM_inmobiliaria.Models
{
    public class RepositorioPago
    {
        public RepositorioPago() { }

        readonly string ConnectionString =
            "Server=localhost;Database=inmobiliaria_db;User=root;Password=;";

        public IList<Pago> GetPagos()
        {
            IList<Pago> listaPagos = new List<Pago>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT p.Id, p.numeroPago, p.idContrato, p.idUsuario, p.fecha, p.importe, p.estado, p.detalle,
                u.nombre AS nombreUsuario, u.apellido AS apellidoUsuario,
                i.nombre AS nombreInquilino, i.apellido AS apellidoInquilino, i.dni AS dniInquilino, i.telefono AS telefonoInquilino, i.email AS emailInquilino,
                pr.nombre AS nombrePropietario, pr.apellido AS apellidoPropietario, pr.dni AS dniPropietario, pr.telefono AS telefonoPropietario, pr.email AS emailPropietario,
                inm.direccion AS direccionInmueble, inm.ambientes AS ambientesInmueble
                FROM pago p  
                INNER JOIN usuario u ON p.idUsuario = u.id
                INNER JOIN contrato c ON p.idContrato = c.id
                INNER JOIN inquilino i ON c.idInquilino = i.id
                INNER JOIN propietario pr ON c.idPropietario = pr.id
                INNER JOIN inmueble inm ON c.idInmueble = inm.id";
                //WHERE p.estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pago pago = new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                NumeroPago = reader.GetInt32("numeroPago"),
                                IdContrato = reader.GetInt32("idContrato"),
                                IdUsuario = reader.GetInt32("idUsuario"),
                                Usuario = new Usuario
                                {
                                    Nombre = reader.GetString("nombreUsuario"),
                                    Apellido = reader.GetString("apellidoUsuario")
                                },
                                Contrato = new Contrato
                                {
                                    Inquilino = new Inquilino
                                    {
                                        Nombre = reader.GetString("nombreInquilino"),
                                        Apellido = reader.GetString("apellidoInquilino"),
                                        Dni = reader.GetString("dniInquilino"),
                                        Telefono = reader.GetString("telefonoInquilino"),
                                        Email = reader.GetString("emailInquilino")
                                    },
                                    Propietario = new Propietario
                                    {
                                        Nombre = reader.GetString("nombrePropietario"),
                                        Apellido = reader.GetString("apellidoPropietario"),
                                        Dni = reader.GetString("dniPropietario"),
                                        Telefono = reader.GetString("telefonoPropietario"),
                                        Email = reader.GetString("emailPropietario")
                                    },
                                    Inmueble = new Inmueble
                                    {
                                        Direccion = reader.GetString("direccionInmueble"),
                                        Ambientes = reader.GetInt32("ambientesInmueble")
                                    }
                                },
                                Fecha = reader.GetDateTime("fecha"),
                                Importe = reader.GetDouble("importe"),
                                Estado = reader.GetBoolean("estado"),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("detalle"))
                                    ? null
                                    : reader.GetString("detalle")
                            };
                            listaPagos.Add(pago);
                        }
                        connection.Close();
                    }
                }
            }
            return listaPagos;
        }

        public Pago GetPago(int id)
        {
            Pago pago = null; 

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT p.Id, p.numeroPago, p.idContrato, p.idUsuario, p.fecha, p.importe, p.estado, p.detalle,
                           u.nombre AS nombreUsuario, u.apellido AS apellidoUsuario,
                           i.nombre AS nombreInquilino, i.apellido AS apellidoInquilino, i.dni AS dniInquilino, i.telefono AS telefonoInquilino, i.email AS emailInquilino,
                           pr.nombre AS nombrePropietario, pr.apellido AS apellidoPropietario, pr.dni AS dniPropietario, pr.telefono AS telefonoPropietario, pr.email AS emailPropietario
                    FROM pago p  
                    INNER JOIN usuario u ON p.idUsuario = u.id
                    INNER JOIN contrato c ON p.idContrato = c.id
                    INNER JOIN inquilino i ON c.idInquilino = i.id
                    INNER JOIN propietario pr ON c.idPropietario = pr.id
                    WHERE p.Id = @PagoId";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PagoId", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                NumeroPago = reader.GetInt32("numeroPago"),
                                IdContrato = reader.GetInt32("idContrato"),
                                IdUsuario = reader.GetInt32("idUsuario"),
                                Usuario = new Usuario
                                {
                                    Nombre = reader.GetString("nombreUsuario"),
                                    Apellido = reader.GetString("apellidoUsuario")
                                },
                                Contrato = new Contrato
                                {
                                    Inquilino = new Inquilino
                                    {
                                        Nombre = reader.GetString("nombreInquilino"),
                                        Apellido = reader.GetString("apellidoInquilino"),
                                        Dni = reader.GetString("dniInquilino"),
                                        Telefono = reader.GetString("telefonoInquilino"),
                                        Email = reader.GetString("emailInquilino")
                                    },
                                    Propietario = new Propietario
                                    {
                                        Nombre = reader.GetString("nombrePropietario"),
                                        Apellido = reader.GetString("apellidoPropietario"),
                                        Dni = reader.GetString("dniPropietario"),
                                        Telefono = reader.GetString("telefonoPropietario"),
                                        Email = reader.GetString("emailPropietario")
                                    }
                                },
                                Fecha = reader.GetDateTime("fecha"),
                                Importe = reader.GetDouble("importe"),
                                Estado = reader.GetBoolean("estado"),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("detalle"))
                                    ? null
                                    : reader.GetString("detalle")
                            };
                        }
                        connection.Close();
                    }
                }
                return pago;
            }
        }

        public void EliminarPago(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @$"UPDATE  pago SET estado = @Estado
                WHERE id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Estado", false);
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public int InsertarPago(Pago pago)
        {
            int idPago = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"INSERT INTO pago (numeroPago, idContrato, idUsuario, fecha, importe, estado, detalle)
              VALUES (@NumeroPago, @IdContrato, @IdUsuario, @Fecha, @Importe, @Estado, @detalle);
              SELECT LAST_INSERT_ID();"; // Consulta para obtener el último ID insertado

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NumeroPago", pago.NumeroPago);
                    command.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
                    command.Parameters.AddWithValue("@IdUsuario", pago.IdUsuario);
                    command.Parameters.AddWithValue("@Fecha", pago.Fecha);
                    command.Parameters.AddWithValue("@Importe", pago.Importe);
                    command.Parameters.AddWithValue("@Estado", true);
                    command.Parameters.AddWithValue("@Detalle", pago.Detalle);

                    connection.Open();
                    idPago = Convert.ToInt32(command.ExecuteScalar()); // Ejecutar la consulta y obtener el ID
                    connection.Close();
                }
            }
            return idPago; // Devolver el ID del pago insertado
        }

        public int ObtenerNumeroPago(int idContrato)
        {
            int numeroPago = 1;

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @"SELECT MAX(NumeroPago) FROM pago WHERE IdContrato = @IdContrato";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", idContrato);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read() && reader[0] != DBNull.Value)
                        {
                            numeroPago = Convert.ToInt32(reader[0]) + 1;
                        }
                    }
                }
            }
            return numeroPago;
        }

        public void ActualizarPago(Pago pago)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql = @"UPDATE pago SET detalle = @Detalle WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Detalle", pago.Detalle);
                    command.Parameters.AddWithValue("@Id", pago.Id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        //Adicional
        public IList<Pago> GetPagosPorContrato(int idContrato)
        {
            IList<Pago> listaPagos = new List<Pago>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT p.Id, p.numeroPago, p.idContrato, p.idUsuario, p.fecha, p.importe, p.estado, p.detalle,
                    u.nombre AS nombreUsuario, u.apellido AS apellidoUsuario,
                    i.nombre AS nombreInquilino, i.apellido AS apellidoInquilino, i.dni AS dniInquilino, i.telefono AS telefonoInquilino, i.email AS emailInquilino,
                    pr.nombre AS nombrePropietario, pr.apellido AS apellidoPropietario, pr.dni AS dniPropietario, pr.telefono AS telefonoPropietario, pr.email AS emailPropietario,
                    inm.direccion AS direccionInmueble, inm.ambientes AS ambientesInmueble
                    FROM pago p  
                    INNER JOIN usuario u ON p.idUsuario = u.id
                    INNER JOIN contrato c ON p.idContrato = c.id
                    INNER JOIN inquilino i ON c.idInquilino = i.id
                    INNER JOIN propietario pr ON c.idPropietario = pr.id
                    INNER JOIN inmueble inm ON c.idInmueble = inm.id
                    WHERE p.idContrato = @IdContrato AND p.estado = 1";

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
                                Id = reader.GetInt32("Id"),
                                NumeroPago = reader.GetInt32("numeroPago"),
                                IdContrato = reader.GetInt32("idContrato"),
                                IdUsuario = reader.GetInt32("idUsuario"),
                                Usuario = new Usuario
                                {
                                    Nombre = reader.GetString("nombreUsuario"),
                                    Apellido = reader.GetString("apellidoUsuario")
                                },
                                Contrato = new Contrato
                                {
                                    Inquilino = new Inquilino
                                    {
                                        Nombre = reader.GetString("nombreInquilino"),
                                        Apellido = reader.GetString("apellidoInquilino"),
                                        Dni = reader.GetString("dniInquilino"),
                                        Telefono = reader.GetString("telefonoInquilino"),
                                        Email = reader.GetString("emailInquilino")
                                    },
                                    Propietario = new Propietario
                                    {
                                        Nombre = reader.GetString("nombrePropietario"),
                                        Apellido = reader.GetString("apellidoPropietario"),
                                        Dni = reader.GetString("dniPropietario"),
                                        Telefono = reader.GetString("telefonoPropietario"),
                                        Email = reader.GetString("emailPropietario")
                                    },
                                    Inmueble = new Inmueble
                                    {
                                        Direccion = reader.GetString("direccionInmueble"),
                                        Ambientes = reader.GetInt32("ambientesInmueble")
                                    }
                                },
                                Fecha = reader.GetDateTime("fecha"),
                                Importe = reader.GetDouble("importe"),
                                Estado = reader.GetBoolean("estado"),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("detalle"))
                                    ? null
                                    : reader.GetString("detalle")
                            };
                            listaPagos.Add(pago);
                        }
                        connection.Close();
                    }
                }
            }
            return listaPagos;
        }
    }
}
