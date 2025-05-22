using SchoolHub.dtos;
using SchoolHub.Utilities;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SchoolHub.repositories.Models
{
    public class AsistenciaRepository
    {
        private readonly DBContextUtility _conexion;

        public AsistenciaRepository(DBContextUtility conexion)
        {
            _conexion = conexion;
        }

        public List<AsistenciaDto> ObtenerAsistencias()
        {
            List<AsistenciaDto> asistencias = new List<AsistenciaDto>();

            try
            {
                _conexion.Connect();
                string SQL = "SELECT * FROM Asistencia";

                using (SqlCommand command = new SqlCommand(SQL, _conexion.Conexion()))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AsistenciaDto asistencia = new AsistenciaDto
                        {
                            IdAsistencia = Convert.ToInt32(reader["id_asistencia"]),
                            IdEstudiante = Convert.ToInt32(reader["id_estudiante"]),
                            Fecha = Convert.ToDateTime(reader["fecha"]), // Cambiado a DateTime
                            Presente = Convert.ToBoolean(reader["presente"]),
                            Motivo = reader["motivo"]?.ToString() ?? string.Empty,
                            Mensaje = string.Empty
                        };
                        asistencias.Add(asistencia);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener asistencias: {ex.Message}");
                // Considera lanzar la excepción o manejarla de otra forma
            }
            finally
            {
                _conexion.Disconnect();
            }

            return asistencias;
        }

        public int RegistrarAsistencia(AsistenciaDto asistencia)
        {
            int resultado = 0;

            try
            {
                _conexion.Connect();
                string SQL = @"INSERT INTO Asistencia 
                             (id_estudiante, fecha, presente, motivo)
                             VALUES (@id_estudiante, @fecha, @presente, @motivo)";

                using (SqlCommand command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@id_estudiante", SqlDbType.Int).Value = asistencia.IdEstudiante;
                    command.Parameters.Add("@fecha", SqlDbType.Date).Value = asistencia.Fecha; // Usando DateTime directamente
                    command.Parameters.Add("@presente", SqlDbType.Bit).Value = asistencia.Presente;
                    command.Parameters.Add("@motivo", SqlDbType.VarChar, 255).Value =
                        string.IsNullOrEmpty(asistencia.Motivo) ? DBNull.Value : (object)asistencia.Motivo;

                    resultado = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar asistencia: {ex.Message}");
                // Considera lanzar la excepción o manejarla de otra forma
                resultado = -1; // Valor que indica error
            }
            finally
            {
                _conexion.Disconnect();
            }

            return resultado;
        }

        internal List<AsistenciaDto>? ObtenerAsistenciasPorEstudiante(int idEstudiante)
        {
            throw new NotImplementedException();
        }
    }
}