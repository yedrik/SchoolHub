using Microsoft.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System;
using System.Collections.Generic;
using System.Data;


namespace SchoolHub.repositories.Models
{
    public class EventoRepository
    {
        private readonly DBContextUtility _conexion;

        public EventoRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public List<EventoDto> ObtenerEventos()
        {
            List<EventoDto> eventos = new List<EventoDto>();

            try
            {
                _conexion.Connect();
                const string SQL = "SELECT id_evento, nombre, descripcion, fecha, lugar FROM Evento";

                using (SqlCommand command = new SqlCommand(SQL, _conexion.Conexion()))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EventoDto evento = new EventoDto
                        {
                            IdEvento = reader.GetInt32(reader.GetOrdinal("id_evento")),
                            Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ?
                                     null : reader.GetString(reader.GetOrdinal("nombre")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ?
                                         null : reader.GetString(reader.GetOrdinal("descripcion")),
                            Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")).ToString("yyyy-MM-dd"),
                            Lugar = reader.IsDBNull(reader.GetOrdinal("lugar")) ?
                                   null : reader.GetString(reader.GetOrdinal("lugar"))
                        };
                        eventos.Add(evento);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log database-specific exceptions
                Console.WriteLine($"Database error al obtener eventos: {sqlEx.Message}");
                throw; // Re-throw to allow handling at a higher level
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener eventos: {ex.Message}");
                throw; // Re-throw to allow handling at a higher level
            }
            finally
            {
                _conexion.Disconnect();
            }

            return eventos;
        }

        internal int RegistrarEvento(EventoDto evento)
        {
            throw new NotImplementedException();
        }
    }
}