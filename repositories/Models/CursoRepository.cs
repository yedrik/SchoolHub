using Microsoft.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System;
using System.Collections.Generic;
using System.Data;


namespace SchoolHub.repositories.Models
{
    public class CursoRepository
    {
        private readonly DBContextUtility _conexion;

        public CursoRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public List<CursoDto> ObtenerCursos()
        {
            var cursos = new List<CursoDto>();

            try
            {
                _conexion.Connect();
                const string SQL = "SELECT id_curso, nombre, descripcion FROM Curso";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cursos.Add(new CursoDto
                        {
                            IdCurso = reader.GetInt32("id_curso"),
                            Nombre = reader.GetString("nombre"),
                            Descripcion = reader.IsDBNull("descripcion") ? string.Empty : reader.GetString("descripcion")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Consider using proper logging here
                throw new ApplicationException("Error al obtener cursos", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }

            return cursos;
        }

        public int RegistrarCurso(CursoDto curso)
        {
            if (curso == null) throw new ArgumentNullException(nameof(curso));

            try
            {
                _conexion.Connect();
                const string SQL = @"INSERT INTO Curso (nombre, descripcion) 
                                   VALUES (@nombre, @descripcion);
                                   SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = curso.Nombre;
                    command.Parameters.Add("@descripcion", SqlDbType.VarChar, 500).Value =
                        string.IsNullOrEmpty(curso.Descripcion) ? DBNull.Value : (object)curso.Descripcion;

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al registrar curso", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public CursoDto ObtenerCursoPorId(int id)
        {
            try
            {
                _conexion.Connect();
                const string SQL = "SELECT id_curso, nombre, descripcion FROM Curso WHERE id_curso = @id_curso";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@id_curso", SqlDbType.Int).Value = id;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CursoDto
                            {
                                IdCurso = reader.GetInt32("id_curso"),
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.IsDBNull("descripcion") ? string.Empty : reader.GetString("descripcion")
                            };
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener curso con ID {id}", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public int ActualizarCurso(CursoDto curso)
        {
            if (curso == null) throw new ArgumentNullException(nameof(curso));

            try
            {
                _conexion.Connect();
                const string SQL = @"UPDATE Curso 
                                   SET nombre = @nombre, 
                                       descripcion = @descripcion 
                                   WHERE id_curso = @id_curso";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@id_curso", SqlDbType.Int).Value = curso.IdCurso;
                    command.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = curso.Nombre;
                    command.Parameters.Add("@descripcion", SqlDbType.VarChar, 500).Value =
                        string.IsNullOrEmpty(curso.Descripcion) ? DBNull.Value : (object)curso.Descripcion;

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al actualizar curso con ID {curso.IdCurso}", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public int EliminarCurso(int id)
        {
            try
            {
                _conexion.Connect();
                const string SQL = "DELETE FROM Curso WHERE id_curso = @id_curso";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@id_curso", SqlDbType.Int).Value = id;
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al eliminar curso con ID {id}", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        internal List<CursoDto>? ObtenerCursosPorEstudiante(int idEstudiante)
        {
            throw new NotImplementedException();
        }
    }
}