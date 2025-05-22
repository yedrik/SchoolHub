using Microsoft.Data.SqlClient;
using SchoolHub.dtos;      // Para ProfesorDto
using SchoolHub.Utilities; // Para DBContextUtility
using System;
using System.Collections.Generic;
using System.Data;

namespace SchoolHub.repositories.Models // O el namespace donde tengas tus repositorios
{
    public class ProfesorRepository
    {
        private readonly DBContextUtility _conexion;

        public ProfesorRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        /// <summary>
        /// Método auxiliar para mapear un SqlDataReader a un ProfesorDto.
        /// </summary>
        private ProfesorDto MapearProfesor(SqlDataReader reader)
        {
            return new ProfesorDto
            {
                IdProfesor = reader.GetInt32(reader.GetOrdinal("IdProfesor")),
                Nombres = reader.IsDBNull(reader.GetOrdinal("Nombres")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombres")),
                Apellidos = reader.IsDBNull(reader.GetOrdinal("Apellidos")) ? string.Empty : reader.GetString(reader.GetOrdinal("Apellidos")),
                Correo = reader.IsDBNull(reader.GetOrdinal("Correo")) ? string.Empty : reader.GetString(reader.GetOrdinal("Correo")),
                Especialidad = reader.IsDBNull(reader.GetOrdinal("Especialidad")) ? string.Empty : reader.GetString(reader.GetOrdinal("Especialidad")),
                Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Foto")),
                Codigo = reader.IsDBNull(reader.GetOrdinal("Codigo")) ? string.Empty : reader.GetString(reader.GetOrdinal("Codigo")),
                Departamento = reader.IsDBNull(reader.GetOrdinal("Departamento")) ? string.Empty : reader.GetString(reader.GetOrdinal("Departamento")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                EnLinea = reader.IsDBNull(reader.GetOrdinal("EnLinea")) ? false : reader.GetBoolean(reader.GetOrdinal("EnLinea"))
            };
        }

        /// <summary>
        /// Obtiene todos los profesores. Por defecto, solo los activos.
        /// </summary>
        /// <param name="incluirInactivos">Si es true, incluye también los profesores inactivos.</param>
        /// <returns>Lista de ProfesorDto.</returns>
        public List<ProfesorDto> ObtenerTodos(bool incluirInactivos = false)
        {
            var profesores = new List<ProfesorDto>();
            // **AJUSTA LOS NOMBRES DE COLUMNA AQUÍ SI SON DIFERENTES EN TU DB**
            string sql = @"SELECT IdProfesor, Nombres, Apellidos, Correo, Especialidad, Foto, Codigo, Departamento, Activo, EnLinea 
                           FROM Profesores"; // Asume que la tabla se llama Profesores

            if (!incluirInactivos)
            {
                sql += " WHERE Activo = 1"; // Asume columna 'Activo' de tipo BIT
            }
            sql += " ORDER BY Apellidos, Nombres";

            try
            {
                _conexion.Connect();
                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        profesores.Add(MapearProfesor(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProfesorRepository.ObtenerTodos: {ex.ToString()}");
                throw new ApplicationException("Error al obtener todos los profesores.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return profesores;
        }

        /// <summary>
        /// Obtiene un profesor específico por su ID.
        /// </summary>
        /// <param name="idProfesor">ID del profesor.</param>
        /// <returns>ProfesorDto si se encuentra, de lo contrario null.</returns>
        public ProfesorDto ObtenerPorId(int idProfesor)
        {
            ProfesorDto profesor = null;
            // **AJUSTA LOS NOMBRES DE COLUMNA AQUÍ SI SON DIFERENTES EN TU DB**
            const string sql = @"SELECT IdProfesor, Nombres, Apellidos, Correo, Especialidad, Foto, Codigo, Departamento, Activo, EnLinea 
                                 FROM Profesores 
                                 WHERE IdProfesor = @IdProfesor";

            try
            {
                _conexion.Connect();
                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdProfesor", SqlDbType.Int).Value = idProfesor;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            profesor = MapearProfesor(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProfesorRepository.ObtenerPorId({idProfesor}): {ex.ToString()}");
                throw new ApplicationException($"Error al obtener el profesor con ID {idProfesor}.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return profesor;
        }

        /// <summary>
        /// Agrega un nuevo profesor a la base de datos.
        /// </summary>
        /// <param name="profesor">DTO del profesor a agregar.</param>
        /// <returns>El ID del profesor recién creado, o 0 si falla.</returns>
        public int Agregar(ProfesorDto profesor)
        {
            if (profesor == null) throw new ArgumentNullException(nameof(profesor));

            int nuevoIdProfesor = 0;
            // **AJUSTA LOS NOMBRES DE COLUMNA AQUÍ SI SON DIFERENTES EN TU DB**
            const string sql = @"INSERT INTO Profesores 
                                 (Nombres, Apellidos, Correo, Especialidad, Foto, Codigo, Departamento, Activo, EnLinea) 
                                 OUTPUT INSERTED.IdProfesor 
                                 VALUES 
                                 (@Nombres, @Apellidos, @Correo, @Especialidad, @Foto, @Codigo, @Departamento, @Activo, @EnLinea);";
            // Usamos OUTPUT INSERTED.IdProfesor en lugar de SCOPE_IDENTITY() para obtener el ID directamente

            try
            {
                _conexion.Connect();
                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    // Asigna los parámetros basados en el DTO
                    command.Parameters.Add("@Nombres", SqlDbType.NVarChar, 100).Value = (object)profesor.Nombres ?? DBNull.Value;
                    command.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 100).Value = (object)profesor.Apellidos ?? DBNull.Value;
                    command.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = (object)profesor.Correo ?? DBNull.Value;
                    command.Parameters.Add("@Especialidad", SqlDbType.NVarChar, 100).Value = (object)profesor.Especialidad ?? DBNull.Value;
                    command.Parameters.Add("@Foto", SqlDbType.NVarChar, 255).Value = (object)profesor.Foto ?? DBNull.Value;
                    command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 50).Value = (object)profesor.Codigo ?? DBNull.Value;
                    command.Parameters.Add("@Departamento", SqlDbType.NVarChar, 100).Value = (object)profesor.Departamento ?? DBNull.Value;
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = profesor.Activo;
                    command.Parameters.Add("@EnLinea", SqlDbType.Bit).Value = profesor.EnLinea;

                    object result = command.ExecuteScalar(); // ExecuteScalar porque OUTPUT INSERTED devuelve un valor
                    if (result != null && result != DBNull.Value)
                    {
                        nuevoIdProfesor = Convert.ToInt32(result);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL en ProfesorRepository.Agregar: {sqlEx.ToString()}");
                throw new ApplicationException($"Error de base de datos al agregar profesor: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProfesorRepository.Agregar: {ex.ToString()}");
                throw new ApplicationException("Error general al agregar profesor.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return nuevoIdProfesor;
        }

        /// <summary>
        /// Actualiza la información de un profesor existente.
        /// </summary>
        /// <param name="profesor">DTO del profesor con la información actualizada.</param>
        /// <returns>True si la actualización fue exitosa, de lo contrario false.</returns>
        public bool Actualizar(ProfesorDto profesor)
        {
            if (profesor == null) throw new ArgumentNullException(nameof(profesor));

            int filasAfectadas = 0;
            // **AJUSTA LOS NOMBRES DE COLUMNA AQUÍ SI SON DIFERENTES EN TU DB**
            const string sql = @"UPDATE Profesores SET 
                                 Nombres = @Nombres, 
                                 Apellidos = @Apellidos, 
                                 Correo = @Correo, 
                                 Especialidad = @Especialidad, 
                                 Foto = @Foto, 
                                 Codigo = @Codigo, 
                                 Departamento = @Departamento, 
                                 Activo = @Activo, 
                                 EnLinea = @EnLinea
                                 WHERE IdProfesor = @IdProfesor";

            try
            {
                _conexion.Connect();
                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdProfesor", SqlDbType.Int).Value = profesor.IdProfesor;
                    command.Parameters.Add("@Nombres", SqlDbType.NVarChar, 100).Value = (object)profesor.Nombres ?? DBNull.Value;
                    command.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 100).Value = (object)profesor.Apellidos ?? DBNull.Value;
                    command.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = (object)profesor.Correo ?? DBNull.Value;
                    command.Parameters.Add("@Especialidad", SqlDbType.NVarChar, 100).Value = (object)profesor.Especialidad ?? DBNull.Value;
                    command.Parameters.Add("@Foto", SqlDbType.NVarChar, 255).Value = (object)profesor.Foto ?? DBNull.Value;
                    command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 50).Value = (object)profesor.Codigo ?? DBNull.Value;
                    command.Parameters.Add("@Departamento", SqlDbType.NVarChar, 100).Value = (object)profesor.Departamento ?? DBNull.Value;
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = profesor.Activo;
                    command.Parameters.Add("@EnLinea", SqlDbType.Bit).Value = profesor.EnLinea;

                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProfesorRepository.Actualizar({profesor.IdProfesor}): {ex.ToString()}");
                throw new ApplicationException($"Error al actualizar el profesor con ID {profesor.IdProfesor}.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return filasAfectadas > 0;
        }

        /// <summary>
        /// Cambia el estado Activo de un profesor (para eliminación lógica).
        /// </summary>
        /// <param name="idProfesor">ID del profesor.</param>
        /// <param name="activo">Nuevo estado (true para activo, false para inactivo).</param>
        /// <returns>True si el cambio de estado fue exitoso, de lo contrario false.</returns>
        public bool CambiarEstadoActivo(int idProfesor, bool activo)
        {
            int filasAfectadas = 0;
            // **AJUSTA EL NOMBRE DE COLUMNA AQUÍ SI ES DIFERENTE EN TU DB**
            const string sql = "UPDATE Profesores SET Activo = @Activo WHERE IdProfesor = @IdProfesor";

            try
            {
                _conexion.Connect();
                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdProfesor", SqlDbType.Int).Value = idProfesor;
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProfesorRepository.CambiarEstadoActivo({idProfesor}): {ex.ToString()}");
                throw new ApplicationException($"Error al cambiar el estado del profesor con ID {idProfesor}.", ex);
            }
            finally
            {
                _conexion.Disconnect();
            }
            return filasAfectadas > 0;
        }
    }
}