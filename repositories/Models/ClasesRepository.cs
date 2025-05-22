using Microsoft.Data.SqlClient; // Usando Microsoft.Data.SqlClient para .NET Core/.NET 5+
using SchoolHub.dtos;         // Namespace de tus DTOs
using SchoolHub.Utilities;    // Namespace de tu DBContextUtility
using System;
using System.Collections.Generic;
using System.Data;

namespace SchoolHub.repositories.Models // O el namespace correcto para tus repositorios
{
    public class ClaseRepository
    {
        private readonly DBContextUtility _conexion;

        public ClaseRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public List<ClaseDto> ObtenerClases()
        {
            var clases = new List<ClaseDto>();

            try
            {
                _conexion.Connect(); // Gestiona la conexión como lo tienes implementado
                const string SQL = @"
                    SELECT 
                        c.IdClase, c.Codigo, c.Activo, c.IdGrado,
                        m.IdMateria, m.Nombre AS NombreMateria,
                        p.IdProfesor, p.Nombres + ' ' + p.Apellidos AS NombreProfesor,
                        s.IdSalon, s.Nombre AS NombreSalon,
                        h.IdHorario, h.DiaSemana, h.HoraInicio, h.HoraFin,
                        g.Nombre AS Grado
                    FROM Clases c
                    INNER JOIN Materias m ON c.IdMateria = m.IdMateria
                    INNER JOIN Profesores p ON c.IdProfesor = p.IdProfesor
                    INNER JOIN Salones s ON c.IdSalon = s.IdSalon
                    INNER JOIN Horarios h ON c.IdHorario = h.IdHorario
                    INNER JOIN Grados g ON c.IdGrado = g.IdGrado
                    ORDER BY c.Activo DESC, h.DiaSemana, h.HoraInicio";

                // Usar using para SqlCommand y SqlDataReader asegura que se desechen correctamente
                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clases.Add(new ClaseDto
                        {
                            IdClase = reader.GetInt32(reader.GetOrdinal("IdClase")),
                            Codigo = reader.IsDBNull(reader.GetOrdinal("Codigo")) ? 0 : reader.GetInt32(reader.GetOrdinal("Codigo")),
                            Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                            IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                            NombreMateria = reader.GetString(reader.GetOrdinal("NombreMateria")),
                            IdProfesor = reader.GetInt32(reader.GetOrdinal("IdProfesor")),
                            NombreProfesor = reader.GetString(reader.GetOrdinal("NombreProfesor")),
                            IdSalon = reader.GetInt32(reader.GetOrdinal("IdSalon")),
                            NombreSalon = reader.GetString(reader.GetOrdinal("NombreSalon")),
                            IdHorario = reader.GetInt32(reader.GetOrdinal("IdHorario")),
                            DiaSemana = reader.GetString(reader.GetOrdinal("DiaSemana")),
                            HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                            HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                            IdGrado = reader.IsDBNull(reader.GetOrdinal("IdGrado")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdGrado")),
                            Grado = reader.GetString(reader.GetOrdinal("Grado"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Considera un logging más robusto y específico
                Console.WriteLine($"Error en ClaseRepository.ObtenerClases: {ex.ToString()}");
                throw new ApplicationException("Error al obtener clases.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }

            return clases;
        }

        public int RegistrarClase(ClaseDto clase)
        {
            if (clase == null) throw new ArgumentNullException(nameof(clase));
            // if (!clase.EsValido()) throw new ArgumentException("Datos de la clase no válidos. Verifique la lógica de EsValido.");

            int idClaseGenerado = 0;
            try
            {
                _conexion.Connect();
                const string SQL = @"
                    INSERT INTO Clases (
                        Codigo, IdMateria, IdProfesor, IdSalon, IdHorario, IdGrado, Activo
                    ) VALUES (
                        @Codigo, @IdMateria, @IdProfesor, @IdSalon, @IdHorario, @IdGrado, @Activo
                    );
                    SELECT SCOPE_IDENTITY();"; // Para obtener el ID de la clase recién insertada

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@Codigo", SqlDbType.Int).Value = (object)clase.Codigo ?? DBNull.Value;
                    command.Parameters.Add("@IdMateria", SqlDbType.Int).Value = clase.IdMateria;
                    command.Parameters.Add("@IdProfesor", SqlDbType.Int).Value = clase.IdProfesor;
                    command.Parameters.Add("@IdSalon", SqlDbType.Int).Value = clase.IdSalon;
                    command.Parameters.Add("@IdHorario", SqlDbType.Int).Value = clase.IdHorario;
                    command.Parameters.Add("@IdGrado", SqlDbType.Int).Value = clase.IdGrado; // Se usa la propiedad IdGrado (int)
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = clase.Activo;

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        idClaseGenerado = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ClaseRepository.RegistrarClase: {ex.ToString()}");
                throw new ApplicationException("Error al registrar la clase.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return idClaseGenerado; // Devuelve el ID de la clase creada
        }

        public ClaseDto ObtenerClasePorId(int id)
        {
            ClaseDto clase = null; // Inicializar a null
            try
            {
                _conexion.Connect();
                const string SQL = @"
                    SELECT 
                        c.IdClase, c.Codigo, c.Activo, c.IdGrado,
                        m.IdMateria, m.Nombre AS NombreMateria,
                        p.IdProfesor, p.Nombres + ' ' + p.Apellidos AS NombreProfesor,
                        s.IdSalon, s.Nombre AS NombreSalon,
                        h.IdHorario, h.DiaSemana, h.HoraInicio, h.HoraFin,
                        g.Nombre AS Grado
                    FROM Clases c
                    INNER JOIN Materias m ON c.IdMateria = m.IdMateria
                    INNER JOIN Profesores p ON c.IdProfesor = p.IdProfesor
                    INNER JOIN Salones s ON c.IdSalon = s.IdSalon
                    INNER JOIN Horarios h ON c.IdHorario = h.IdHorario
                    INNER JOIN Grados g ON c.IdGrado = g.IdGrado
                    WHERE c.IdClase = @IdClase";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdClase", SqlDbType.Int).Value = id;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            clase = new ClaseDto
                            {
                                IdClase = reader.GetInt32(reader.GetOrdinal("IdClase")),
                                Codigo = reader.IsDBNull(reader.GetOrdinal("Codigo")) ? 0 : reader.GetInt32(reader.GetOrdinal("Codigo")),
                                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                                IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                                NombreMateria = reader.GetString(reader.GetOrdinal("NombreMateria")),
                                IdProfesor = reader.GetInt32(reader.GetOrdinal("IdProfesor")),
                                NombreProfesor = reader.GetString(reader.GetOrdinal("NombreProfesor")),
                                IdSalon = reader.GetInt32(reader.GetOrdinal("IdSalon")),
                                NombreSalon = reader.GetString(reader.GetOrdinal("NombreSalon")),
                                IdHorario = reader.GetInt32(reader.GetOrdinal("IdHorario")),
                                DiaSemana = reader.GetString(reader.GetOrdinal("DiaSemana")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                IdGrado = reader.IsDBNull(reader.GetOrdinal("IdGrado")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdGrado")),
                                Grado = reader.GetString(reader.GetOrdinal("Grado"))
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ClaseRepository.ObtenerClasePorId({id}): {ex.ToString()}");
                throw new ApplicationException($"Error al obtener la clase con ID {id}.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return clase; // Devuelve null si no se encontró la clase
        }

        public int ActualizarClase(ClaseDto clase)
        {
            if (clase == null) throw new ArgumentNullException(nameof(clase));
            // if (!clase.EsValido()) throw new ArgumentException("Datos de la clase no válidos. Verifique la lógica de EsValido.");

            int filasAfectadas = 0;
            try
            {
                _conexion.Connect();
                const string SQL = @"
                    UPDATE Clases SET
                        Codigo = @Codigo,
                        IdMateria = @IdMateria,
                        IdProfesor = @IdProfesor,
                        IdSalon = @IdSalon,
                        IdHorario = @IdHorario,
                        IdGrado = @IdGrado,
                        Activo = @Activo
                    WHERE IdClase = @IdClase";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdClase", SqlDbType.Int).Value = clase.IdClase;
                    command.Parameters.Add("@Codigo", SqlDbType.Int).Value = (object)clase.Codigo ?? DBNull.Value;
                    command.Parameters.Add("@IdMateria", SqlDbType.Int).Value = clase.IdMateria;
                    command.Parameters.Add("@IdProfesor", SqlDbType.Int).Value = clase.IdProfesor;
                    command.Parameters.Add("@IdSalon", SqlDbType.Int).Value = clase.IdSalon;
                    command.Parameters.Add("@IdHorario", SqlDbType.Int).Value = clase.IdHorario;
                    command.Parameters.Add("@IdGrado", SqlDbType.Int).Value = clase.IdGrado; // Se usa la propiedad IdGrado (int)
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = clase.Activo;

                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ClaseRepository.ActualizarClase({clase.IdClase}): {ex.ToString()}");
                throw new ApplicationException($"Error al actualizar la clase con ID {clase.IdClase}.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return filasAfectadas;
        }

        public int EliminarClase(int id)
        {
            int filasAfectadas = 0;
            try
            {
                _conexion.Connect();
                const string SQL = "DELETE FROM Clases WHERE IdClase = @IdClase";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdClase", SqlDbType.Int).Value = id;
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ClaseRepository.EliminarClase({id}): {ex.ToString()}");
                throw new ApplicationException($"Error al eliminar la clase con ID {id}.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return filasAfectadas;
        }

        public List<ClaseDto> ObtenerClasesPorProfesor(int idProfesor)
        {
            var clases = new List<ClaseDto>();
            try
            {
                _conexion.Connect();
                const string SQL = @"
                    SELECT 
                        c.IdClase, c.Codigo, c.Activo, c.IdGrado,
                        m.IdMateria, m.Nombre AS NombreMateria,
                        p.IdProfesor, p.Nombres + ' ' + p.Apellidos AS NombreProfesor,
                        s.IdSalon, s.Nombre AS NombreSalon,
                        h.IdHorario, h.DiaSemana, h.HoraInicio, h.HoraFin,
                        g.Nombre AS Grado
                    FROM Clases c
                    INNER JOIN Materias m ON c.IdMateria = m.IdMateria
                    INNER JOIN Profesores p ON c.IdProfesor = p.IdProfesor
                    INNER JOIN Salones s ON c.IdSalon = s.IdSalon
                    INNER JOIN Horarios h ON c.IdHorario = h.IdHorario
                    INNER JOIN Grados g ON c.IdGrado = g.IdGrado
                    WHERE c.IdProfesor = @IdProfesor AND c.Activo = 1
                    ORDER BY h.DiaSemana, h.HoraInicio";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdProfesor", SqlDbType.Int).Value = idProfesor;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clases.Add(new ClaseDto
                            {
                                IdClase = reader.GetInt32(reader.GetOrdinal("IdClase")),
                                Codigo = reader.IsDBNull(reader.GetOrdinal("Codigo")) ? 0 : reader.GetInt32(reader.GetOrdinal("Codigo")),
                                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                                IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                                NombreMateria = reader.GetString(reader.GetOrdinal("NombreMateria")),
                                IdProfesor = reader.GetInt32(reader.GetOrdinal("IdProfesor")),
                                NombreProfesor = reader.GetString(reader.GetOrdinal("NombreProfesor")),
                                IdSalon = reader.GetInt32(reader.GetOrdinal("IdSalon")),
                                NombreSalon = reader.GetString(reader.GetOrdinal("NombreSalon")),
                                IdHorario = reader.GetInt32(reader.GetOrdinal("IdHorario")),
                                DiaSemana = reader.GetString(reader.GetOrdinal("DiaSemana")),
                                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                                HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                                IdGrado = reader.IsDBNull(reader.GetOrdinal("IdGrado")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdGrado")),
                                Grado = reader.GetString(reader.GetOrdinal("Grado"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ClaseRepository.ObtenerClasesPorProfesor({idProfesor}): {ex.ToString()}");
                throw new ApplicationException($"Error al obtener clases del profesor con ID {idProfesor}.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return clases;
        }

        public bool CambiarEstadoClase(int id, bool activo)
        {
            int filasAfectadas = 0;
            try
            {
                _conexion.Connect();
                const string SQL = "UPDATE Clases SET Activo = @Activo WHERE IdClase = @IdClase";

                using (var command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdClase", SqlDbType.Int).Value = id;
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;

                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ClaseRepository.CambiarEstadoClase({id}): {ex.ToString()}");
                throw new ApplicationException($"Error al cambiar estado de la clase con ID {id}.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return filasAfectadas > 0;
        }
    }
}