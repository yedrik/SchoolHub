using SchoolHub.dtos; // Asegúrate que el namespace es correcto
using SchoolHub.Utilities; // Si usas alguna utilidad de aquí
using System.Data;
using Microsoft.Data.SqlClient;

using System;
using System.Collections.Generic;

namespace SchoolHub.repositories.Models
{
    public class CalificacionesRepository
    {
        private readonly DBContextUtility _conexion;

        public CalificacionesRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public List<CalificacionDto> ObtenerCalificaciones(int idEstudiante)
        {
            List<CalificacionDto> calificaciones = new List<CalificacionDto>();

            try
            {
                _conexion.Connect(); // Considera el patrón using para SqlConnection
                string SQL = @"SELECT id_calificacion, id_estudiante, id_materia, nota, fecha, periodo 
                               FROM Calificacion 
                               WHERE id_estudiante = @id_estudiante";

                using (SqlCommand command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@id_estudiante", SqlDbType.Int).Value = idEstudiante;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CalificacionDto calificacion = new CalificacionDto
                            {
                                IdCalificacion = Convert.ToInt32(reader["id_calificacion"]),
                                IdEstudiante = Convert.ToInt32(reader["id_estudiante"]),
                                IdMateria = Convert.ToInt32(reader["id_materia"]),
                                Nota = Convert.ToDouble(reader["nota"]),
                                Fecha = Convert.ToDateTime(reader["fecha"]),
                                Periodo = reader["periodo"]?.ToString() ?? string.Empty
                            };
                            calificaciones.Add(calificacion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener calificaciones: {ex.ToString()}"); // Usa ex.ToString() para más detalles
                // Considera usar un sistema de logging más robusto
            }
            finally
            {
                _conexion.Disconnect(); // Considera el patrón using para SqlConnection
            }

            return calificaciones;
        }

        public List<CalificacionDto> ObtenerCalificacionesPorEstudiante(int idEstudiante) // Cambia el nombre si es diferente
        {
            List<CalificacionDto> calificaciones = new List<CalificacionDto>();
            try
            {
                _conexion.Connect();
                // SQL con JOIN para obtener el nombre de la materia
                string SQL = @"SELECT c.id_calificacion, c.id_estudiante, c.id_materia, m.nombre AS nombre_materia, c.nota, c.fecha, c.periodo 
                       FROM Calificacion c
                       INNER JOIN Materia m ON c.id_materia = m.id_materia 
                       WHERE c.id_estudiante = @id_estudiante";

                using (SqlCommand command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@id_estudiante", SqlDbType.Int).Value = idEstudiante;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CalificacionDto calificacion = new CalificacionDto
                            {
                                IdCalificacion = Convert.ToInt32(reader["id_calificacion"]),
                                IdEstudiante = Convert.ToInt32(reader["id_estudiante"]),
                                IdMateria = Convert.ToInt32(reader["id_materia"]),
                                NombreMateria = reader["nombre_materia"]?.ToString() ?? string.Empty, // Poblar nueva propiedad
                                Nota = Convert.ToDouble(reader["nota"]),
                                Fecha = Convert.ToDateTime(reader["fecha"]),
                                Periodo = reader["periodo"]?.ToString() ?? string.Empty
                            };
                            calificaciones.Add(calificacion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener calificaciones por estudiante: {ex.ToString()}");
            }
            finally
            {
                _conexion.Disconnect();
            }
            return calificaciones;
        }

        // No implementado
        internal int RegistrarCalificacion(CalificacionDto calificacion)
        {
            throw new NotImplementedException();
        }

        // ... otros métodos de tu repositorio ...
    }
}