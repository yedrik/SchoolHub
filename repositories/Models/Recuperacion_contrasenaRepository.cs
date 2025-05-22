using Microsoft.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System;
using System.Data;


namespace SchoolHub.repositories.Models
{
    public class RecuperacionContrasenaRepository
    {
        private readonly DBContextUtility _conexion;

        public RecuperacionContrasenaRepository(DBContextUtility conexion)
        {
            _conexion = conexion;
        }

        public string RecuperarContrasena(string correo)
        {
            string respuesta = "No se encontró el correo.";

            try
            {
                _conexion.Connect();
                string SQL = "SELECT correo FROM Usuario WHERE correo = @correo";

                using (SqlCommand command = new SqlCommand(SQL, _conexion.Conexion()))
                {
                    command.Parameters.Add("@correo", SqlDbType.VarChar).Value = correo;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            respuesta = "Correo encontrado. Proceda con la recuperación.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al recuperar contraseña: " + ex.Message);
                respuesta = "Error al procesar la solicitud.";
            }
            finally
            {
                _conexion.Disconnect();
            }

            return respuesta;
        }
    }
}
