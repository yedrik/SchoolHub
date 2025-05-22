using Microsoft.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System;
using System.Collections.Generic;


namespace SchoolHub.repositories.Models
{
    public class RecursoRepository
    {
        private readonly DBContextUtility _conexion;

        public RecursoRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public List<RecursoDto> ObtenerRecursos()
        {
            List<RecursoDto> recursos = new List<RecursoDto>();

            try
            {
                _conexion.Connect();
                const string SQL = "SELECT id_recurso, nombre, tipo, archivo_url, descripcion FROM Recurso";

                using (SqlCommand command = new SqlCommand(SQL, _conexion.Conexion()))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecursoDto recurso = new RecursoDto
                        {
                            IdRecurso = reader.GetInt32(reader.GetOrdinal("id_recurso")),
                            Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                            Tipo = reader.IsDBNull(reader.GetOrdinal("tipo")) ? null : reader.GetString(reader.GetOrdinal("tipo")),
                            ArchivoUrl = reader.IsDBNull(reader.GetOrdinal("archivo_url")) ? null : reader.GetString(reader.GetOrdinal("archivo_url")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion"))
                        };
                        recursos.Add(recurso);
                    }
                }
            }
            catch (Exception ex)
            {
                // Consider logging the error properly instead of Console.WriteLine
                Console.WriteLine($"Error al obtener recursos: {ex.Message}");
                throw; // Re-throw the exception to let the caller handle it
            }
            finally
            {
                _conexion.Disconnect();
            }

            return recursos;
        }

        internal int RegistrarRecurso(RecursoDto recurso)
        {
            throw new NotImplementedException();
        }
    }
}