using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolHub.dtos;

using SchoolHub.Utilities;

namespace SchoolHub.repositories.Models
{
    public class MensajeRepository
    {
        private readonly DBContextUtility _conexion;

        public MensajeRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

       // public MensajeRepository()
     //   {
       // }

        public int RegistrarMensaje(MensajeDto mensaje)
        {
            if (mensaje == null)
                throw new ArgumentNullException(nameof(mensaje));

            int resultado = 0;
            try
            {
                _conexion.Connect();
                const string SQL = @"INSERT INTO Mensaje 
                                    (id_emisor, id_receptor, asunto, contenido, fecha_envio)
                                    VALUES 
                                    (@id_emisor, @id_receptor, @asunto, @contenido, @fecha_envio);
                                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    cmd.Parameters.Add("@id_emisor", SqlDbType.Int).Value = mensaje.IdEmisor;
                    cmd.Parameters.Add("@id_receptor", SqlDbType.Int).Value = mensaje.IdDestinatario;
                    cmd.Parameters.Add("@asunto", SqlDbType.NVarChar, 100).Value = mensaje.Asunto ?? string.Empty;
                    cmd.Parameters.Add("@contenido", SqlDbType.NVarChar, -1).Value = mensaje.Contenido ?? string.Empty;
                    cmd.Parameters.Add("@fecha_envio", SqlDbType.DateTime2).Value = DateTime.Now;

                    // Get the inserted ID
                    var id = cmd.ExecuteScalar();
                    if (id != null && id != DBNull.Value)
                    {
                        resultado = Convert.ToInt32(id);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error al registrar mensaje: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar mensaje: {ex.Message}");
                throw;
            }
            finally
            {
                _conexion.Disconnect();
            }

            return resultado;
        }

        public List<MensajeDto> ObtenerMensajesRecibidos(int idUsuario)
        {
            return ObtenerMensajes("id_receptor = @id_usuario", idUsuario);
        }

        public List<MensajeDto> ObtenerMensajesEnviados(int idUsuario)
        {
            return ObtenerMensajes("id_emisor = @id_usuario", idUsuario);
        }

        public List<MensajeDto> ObtenerMensajesProfesores(int idEstudiante)
        {
            return ObtenerMensajesPorRol(2); // Rol 2 = Profesor
        }

        public List<MensajeDto> ObtenerMensajesAdministracion(int idUsuario)
        {
            return ObtenerMensajesPorRol(1); // Rol 1 = Administración
        }

        private List<MensajeDto> ObtenerMensajes(string filtro, int idUsuario)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                throw new ArgumentException("El filtro no puede estar vacío", nameof(filtro));

            List<MensajeDto> mensajes = new List<MensajeDto>();

            try
            {
                _conexion.Connect();
                string SQL = $@"SELECT id_mensaje, id_emisor, id_receptor, asunto, 
                                       contenido, fecha_envio 
                                FROM Mensaje 
                                WHERE {filtro} 
                                ORDER BY fecha_envio DESC";

                using (SqlCommand cmd = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    cmd.Parameters.Add("@id_usuario", SqlDbType.Int).Value = idUsuario;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            mensajes.Add(new MensajeDto
                            {
                                IdMensaje = reader.GetInt32(reader.GetOrdinal("id_mensaje")),
                                IdEmisor = reader.GetInt32(reader.GetOrdinal("id_emisor")),
                                IdDestinatario = reader.GetInt32(reader.GetOrdinal("id_receptor")),
                                Asunto = reader.IsDBNull(reader.GetOrdinal("asunto")) ?
                                         null : reader.GetString(reader.GetOrdinal("asunto")),
                                Contenido = reader.IsDBNull(reader.GetOrdinal("contenido")) ?
                                           null : reader.GetString(reader.GetOrdinal("contenido")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha_envio")).ToString("yyyy-MM-dd HH:mm:ss")
                            });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error al obtener mensajes: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mensajes: {ex.Message}");
                throw;
            }
            finally
            {
                _conexion.Disconnect();
            }

            return mensajes;
        }

        private List<MensajeDto> ObtenerMensajesPorRol(int idRolEmisor)
        {
            List<MensajeDto> mensajes = new List<MensajeDto>();

            try
            {
                _conexion.Connect();
                const string SQL = @"SELECT m.id_mensaje, m.id_emisor, m.id_receptor, 
                                             m.asunto, m.contenido, m.fecha_envio 
                                      FROM Mensaje m
                                      INNER JOIN Usuario u ON m.id_emisor = u.id_usuario
                                      WHERE u.id_rol = @id_rol
                                      ORDER BY m.fecha_envio DESC";

                using (SqlCommand cmd = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    cmd.Parameters.Add("@id_rol", SqlDbType.Int).Value = idRolEmisor;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            mensajes.Add(new MensajeDto
                            {
                                IdMensaje = reader.GetInt32(reader.GetOrdinal("id_mensaje")),
                                IdEmisor = reader.GetInt32(reader.GetOrdinal("id_emisor")),
                                IdDestinatario = reader.GetInt32(reader.GetOrdinal("id_receptor")),
                                Asunto = reader.IsDBNull(reader.GetOrdinal("asunto")) ?
                                         null : reader.GetString(reader.GetOrdinal("asunto")),
                                Contenido = reader.IsDBNull(reader.GetOrdinal("contenido")) ?
                                           null : reader.GetString(reader.GetOrdinal("contenido")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha_envio")).ToString("yyyy-MM-dd HH:mm:ss")
                            });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error al obtener mensajes por rol: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mensajes por rol: {ex.Message}");
                throw;
            }
            finally
            {
                _conexion.Disconnect();
            }

            return mensajes;
        }

        internal List<MensajeDto> ObtenerMensajesEnviados()
        {
            throw new NotImplementedException();
        }

        internal List<MensajeDto> ObtenerMensajesRecibidos()
        {
            throw new NotImplementedException();
        }
    }
}