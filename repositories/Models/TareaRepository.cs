using Microsoft.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System;
using System.Collections.Generic;
using System.Data;


namespace SchoolHub.repositories.Models
{
    public class TareaRepository
    {
        private readonly DBContextUtility _conexion;

        public TareaRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public TareaRepository()
        {
        }

        public List<TareaDto> ObtenerTareas()
        {
            var tareas = new List<TareaDto>();

            try
            {
                _conexion.Connect();
                const string sql = "SELECT id_tarea, titulo, descripcion, fecha_entrega, estado, id_estudiante FROM Tarea";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tareas.Add(new TareaDto
                        {
                            IdTarea = reader.GetInt32(reader.GetOrdinal("id_tarea")),
                            Titulo = reader.IsDBNull(reader.GetOrdinal("titulo")) ? null : reader.GetString(reader.GetOrdinal("titulo")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                            FechaEntrega = reader.GetDateTime(reader.GetOrdinal("fecha_entrega")),
                            Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                            IdEstudiante = reader.GetInt32(reader.GetOrdinal("id_estudiante"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Consider using a proper logging framework
                throw new ApplicationException("Error al obtener tareas", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }

            return tareas;
        }

        public List<TareaDto> ObtenerTareasPorEstudiante(int idEstudiante)
        {
            var tareas = new List<TareaDto>();

            try
            {
                _conexion.Connect();
                const string sql = "SELECT id_tarea, titulo, descripcion, fecha_entrega, estado, id_estudiante " +
                                  "FROM Tarea WHERE id_estudiante = @id_estudiante";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@id_estudiante", SqlDbType.Int).Value = idEstudiante;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tareas.Add(new TareaDto
                            {
                                IdTarea = reader.GetInt32(reader.GetOrdinal("id_tarea")),
                                Titulo = reader.IsDBNull(reader.GetOrdinal("titulo")) ? null : reader.GetString(reader.GetOrdinal("titulo")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                                FechaEntrega = reader.GetDateTime(reader.GetOrdinal("fecha_entrega")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                                IdEstudiante = reader.GetInt32(reader.GetOrdinal("id_estudiante"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener tareas para el estudiante {idEstudiante}", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }

            return tareas;
        }

        public int RegistrarTarea(TareaDto tarea)
        {
            if (tarea == null)
                throw new ArgumentNullException(nameof(tarea));

            try
            {
                _conexion.Connect();
                const string sql = @"INSERT INTO Tarea 
                                    (titulo, descripcion, fecha_entrega, estado, id_estudiante)
                                    VALUES 
                                    (@titulo, @descripcion, @fecha_entrega, @estado, @id_estudiante);
                                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@titulo", SqlDbType.VarChar, 100).Value = tarea.Titulo ?? string.Empty;
                    command.Parameters.Add("@descripcion", SqlDbType.VarChar, 500).Value = tarea.Descripcion ?? string.Empty;
                    command.Parameters.Add("@fecha_entrega", SqlDbType.DateTime).Value = tarea.FechaEntrega;
                    command.Parameters.Add("@estado", SqlDbType.VarChar, 50).Value = tarea.Estado ?? "Pendiente";
                    command.Parameters.Add("@id_estudiante", SqlDbType.Int).Value = tarea.IdEstudiante;

                    // Execute and return the new ID
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al registrar tarea", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public bool EliminarTarea(int id)
        {
            try
            {
                _conexion.Connect();
                const string sql = "DELETE FROM Tarea WHERE id_tarea = @id_tarea";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@id_tarea", SqlDbType.Int).Value = id;
                    if (command.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al eliminar tarea con ID {id}", ex);
                return false;
            }
            finally
            {
                _conexion.Disconnect();
            }
        }
    }
}