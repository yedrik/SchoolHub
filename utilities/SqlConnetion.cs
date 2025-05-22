using System;
using System.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace SchoolHub.Utilities
{
    /// <summary>
    /// Utilidad para gestionar la conexión a la base de datos SQL Server.
    /// </summary>
    public class SqlConnectionUtility
    {
        private readonly string connectionString =
            "server=DESKTOP-9G8RQO4\\SQLEXPRESS;database=SchoolHub;user id=SchoolHub;password=12345678;MultipleActiveResultSets=true";

        /// <summary>
        /// Abre y devuelve una conexión activa a la base de datos.
        /// </summary>
        /// <returns>Objeto SqlConnection abierto</returns>
        /// <exception cref="Exception">Si ocurre un error al abrir la conexión</exception>
        public SqlConnection GetConnection()
        {
            try
            {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error al conectar a la base de datos: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error general: " + ex.Message);
            }
        }

        /// <summary>
        /// Cierra y libera la conexión SQL proporcionada.
        /// </summary>
        /// <param name="connection">Instancia de SqlConnection a cerrar</param>
        public void CloseConnection(SqlConnection connection)
        {
            if (connection != null && connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }
    }
}
