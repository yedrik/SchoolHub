using Microsoft.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System;
using System.Collections.Generic;
using System.Data;


namespace SchoolHub.repositories.Models
{
    public class EstudianteRepository
    {
        private readonly DBContextUtility _conexion;

        public EstudianteRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public List<EstudianteDto> ObtenerEstudiantes()
        {
            var estudiantes = new List<EstudianteDto>();

            try
            {
                _conexion.Connect();
                const string SQL = @"SELECT id_estudiante, nombre, apellido, correo, fecha_nacimiento, 
                                   foto, promedio, grupo, codigo, grado, activo 
                                   FROM Estudiante";

                using var command = new SqlCommand(SQL, _conexion.Conexion());
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    estudiantes.Add(new EstudianteDto
                    {
                        IdEstudiante = reader.GetInt32("id_estudiante"),
                        Nombres = reader.GetString("nombre"),
                        Apellidos = reader.GetString("apellido"),
                        Correo = reader.GetString("correo"),
                        Foto = reader.IsDBNull(reader.GetOrdinal("foto")) ? string.Empty : reader.GetString("foto"),
                        Promedio = reader.IsDBNull(reader.GetOrdinal("promedio")) ? 0 : reader.GetDecimal("promedio"),
                        Grupo = reader.GetString("grupo"),
                        Codigo = reader.GetString("codigo"),
                        Grado = reader.GetString("grado"),
                        Activo = reader.GetBoolean("activo")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener estudiantes", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }

            return estudiantes;
        }

        public EstudianteDto ObtenerEstudiantePorId(int idEstudiante)
        {
            try
            {
                _conexion.Connect();
                const string SQL = @"SELECT id_estudiante, nombre, apellido, correo, fecha_nacimiento, 
                                   foto, promedio, grupo, codigo, grado, activo 
                                   FROM Estudiante 
                                   WHERE id_estudiante = @idEstudiante";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@idEstudiante", SqlDbType.Int).Value = idEstudiante;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new EstudianteDto
                            {
                                IdEstudiante = reader.GetInt32("id_estudiante"),
                                Nombres = reader.GetString("nombre"),
                                Apellidos = reader.GetString("apellido"),
                                Correo = reader.GetString("correo"),
                                Foto = reader.IsDBNull(reader.GetOrdinal("foto")) ? string.Empty : reader.GetString("foto"),
                                Promedio = reader.IsDBNull(reader.GetOrdinal("promedio")) ? 0 : reader.GetDecimal("promedio"),
                                Grupo = reader.GetString("grupo"),
                                Codigo = reader.GetString("codigo"),
                                Grado = reader.GetString("grado"),
                                Activo = reader.GetBoolean("activo")
                            };
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener estudiante con ID {idEstudiante}", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public int RegistrarEstudiante(EstudianteDto estudiante)
        {
            if (estudiante == null) throw new ArgumentNullException(nameof(estudiante));

            try
            {
                _conexion.Connect();
                const string SQL = @"INSERT INTO Estudiante 
                                   (nombre, apellido, correo, fecha_nacimiento, foto, grupo, codigo, grado, activo)
                                   VALUES (@nombre, @apellido, @correo, @fechaNacimiento, @foto, @grupo, @codigo, @grado, @activo);
                                   SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = estudiante.Nombres;
                    command.Parameters.Add("@apellido", SqlDbType.VarChar, 100).Value = estudiante.Apellidos;
                    command.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value = estudiante.Correo;
                    command.Parameters.Add("@foto", SqlDbType.VarChar, 255).Value = string.IsNullOrEmpty(estudiante.Foto) ? DBNull.Value : (object)estudiante.Foto;
                    command.Parameters.Add("@grupo", SqlDbType.VarChar, 10).Value = estudiante.Grupo;
                    command.Parameters.Add("@codigo", SqlDbType.VarChar, 20).Value = estudiante.Codigo;
                    command.Parameters.Add("@grado", SqlDbType.VarChar, 10).Value = estudiante.Grado;
                    command.Parameters.Add("@activo", SqlDbType.Bit).Value = estudiante.Activo;

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al registrar estudiante", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public int ActualizarEstudiante(EstudianteDto estudiante)
        {
            if (estudiante == null) throw new ArgumentNullException(nameof(estudiante));

            try
            {
                _conexion.Connect();
                const string SQL = @"UPDATE Estudiante 
                                   SET nombre = @nombre, 
                                       apellido = @apellido, 
                                       correo = @correo, 
                                       fecha_nacimiento = @fechaNacimiento,
                                       foto = @foto,
                                       grupo = @grupo,
                                       codigo = @codigo,
                                       grado = @grado,
                                       activo = @activo
                                   WHERE id_estudiante = @idEstudiante";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@idEstudiante", SqlDbType.Int).Value = estudiante.IdEstudiante;
                    command.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = estudiante.Nombres;
                    command.Parameters.Add("@apellido", SqlDbType.VarChar, 100).Value = estudiante.Apellidos;
                    command.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value = estudiante.Correo;
                    command.Parameters.Add("@foto", SqlDbType.VarChar, 255).Value = string.IsNullOrEmpty(estudiante.Foto) ? DBNull.Value : (object)estudiante.Foto;
                    command.Parameters.Add("@grupo", SqlDbType.VarChar, 10).Value = estudiante.Grupo;
                    command.Parameters.Add("@codigo", SqlDbType.VarChar, 20).Value = estudiante.Codigo;
                    command.Parameters.Add("@grado", SqlDbType.VarChar, 10).Value = estudiante.Grado;
                    command.Parameters.Add("@activo", SqlDbType.Bit).Value = estudiante.Activo;

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al actualizar estudiante con ID {estudiante.IdEstudiante}", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public int EliminarEstudiante(int idEstudiante)
        {
            try
            {
                _conexion.Connect();
                const string SQL = "DELETE FROM Estudiante WHERE id_estudiante = @idEstudiante";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@idEstudiante", SqlDbType.Int).Value = idEstudiante;
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al eliminar estudiante con ID {idEstudiante}", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public List<CursoDto> ObtenerCursosPorEstudiante(int idEstudiante)
        {
            var cursos = new List<CursoDto>();

            try
            {
                _conexion.Connect();
                const string SQL = @"SELECT c.id_curso, c.nombre, c.descripcion
                                   FROM Curso c
                                   INNER JOIN EstudianteCurso ec ON c.id_curso = ec.id_curso
                                   WHERE ec.id_estudiante = @idEstudiante";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@idEstudiante", SqlDbType.Int).Value = idEstudiante;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cursos.Add(new CursoDto
                            {
                                IdCurso = reader.GetInt32("id_curso"),
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? string.Empty : reader.GetString("descripcion")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener cursos para el estudiante con ID {idEstudiante}", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }

            return cursos;
        }
    }
}