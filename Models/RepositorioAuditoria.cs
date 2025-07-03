using MySql.Data.MySqlClient;

namespace ABM_inmobiliaria.Models
{
    public class RepositorioAuditoria
    {
        public RepositorioAuditoria() { }

        readonly string ConnectionString =
            "Server=localhost;Database=inmobiliaria_db;User=root;Password=;";

        public IList<Auditoria> GetAuditorias()
        {
            IList<Auditoria> auditorias = new List<Auditoria>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT a.Id, a.idUsuario, a.idEntidad , a.entidad, a.fechaAccion, a.accion, a.descripcion,
                      u.nombre AS nombreUsuario, u.apellido AS apellidoUsuario
              FROM auditoria a
              INNER JOIN usuario u ON a.idUsuario = u.id
              ORDER BY a.fechaAccion ASC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Auditoria auditoria = new Auditoria
                            {
                                Id = reader.GetInt32("Id"),
                                IdUsuario = reader.GetInt32("idUsuario"),
                                IdEntidad = reader.GetInt32("idEntidad"),
                                Entidad = reader.GetString("entidad"),
                                FechaAccion = reader.GetDateTime("fechaAccion"),
                                Accion = reader.GetBoolean("accion"),
                                Descripcion = reader.GetString("descripcion"),
                                Usuario = new Usuario
                                {
                                    Id = reader.GetInt32("idUsuario"),
                                    Nombre = reader.GetString("nombreUsuario"),
                                    Apellido = reader.GetString("apellidoUsuario")
                                }
                            };
                            auditorias.Add(auditoria);
                        }
                    }
                    connection.Close();
                }
            }
            return auditorias;
        }

        public void InsertarAuditoria(Auditoria auditoria)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                string sql =
                    @"INSERT INTO auditoria (idUsuario, idEntidad, entidad, fechaAccion, accion, descripcion)
               VALUES (@idUsuario, @idEntidad, @entidad, @fechaAccion, @accion, @descripcion)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idUsuario", auditoria.IdUsuario);
                    command.Parameters.AddWithValue("@idEntidad", auditoria.IdEntidad);
                    command.Parameters.AddWithValue("@entidad", auditoria.Entidad);
                    command.Parameters.AddWithValue("@fechaAccion", auditoria.FechaAccion);
                    command.Parameters.AddWithValue("@accion", auditoria.Accion);
                    command.Parameters.AddWithValue("@descripcion", auditoria.Descripcion);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public IList<Auditoria> GetAuditoriasPorEntidad(int idEntidad, string nombreEntidad)
        {
            IList<Auditoria> auditorias = new List<Auditoria>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql =
                    @"SELECT a.Id, a.idUsuario, a.idEntidad, a.entidad, a.fechaAccion, a.accion, a.descripcion,
                      u.nombre AS nombreUsuario, u.apellido AS apellidoUsuario
              FROM auditoria a
              INNER JOIN usuario u ON a.idUsuario = u.id
              WHERE a.idEntidad = @idEntidad AND a.entidad = @nombreEntidad
              ORDER BY a.fechaAccion ASC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idEntidad", idEntidad);
                    command.Parameters.AddWithValue("@nombreEntidad", nombreEntidad);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Auditoria auditoria = new Auditoria
                            {
                                Id = reader.GetInt32("Id"),
                                IdUsuario = reader.GetInt32("idUsuario"),
                                IdEntidad = reader.GetInt32("idEntidad"),
                                Entidad = reader.GetString("entidad"),
                                FechaAccion = reader.GetDateTime("fechaAccion"),
                                Accion = reader.GetBoolean("accion"),
                                Descripcion = reader.GetString("descripcion"),
                                Usuario = new Usuario
                                {
                                    Id = reader.GetInt32("idUsuario"),
                                    Nombre = reader.GetString("nombreUsuario"),
                                    Apellido = reader.GetString("apellidoUsuario")
                                }
                            };
                            auditorias.Add(auditoria);
                        }
                    }
                }
            }

            return auditorias;
        }
    }
}
