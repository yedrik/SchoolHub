using System;
using System.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace SchoolHub.Utilities
{
    public class DBContextUtility
    {
        private readonly string _connectionString;
        private SqlConnection _sqlConnection;

        public DBContextUtility(IConfiguration configuration)
        {
            // Lee la cadena de conexión llamada "DefaultConnection" del appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encontró o está vacía en appsettings.json.");
            }
        }

        
        public SqlConnection Conexion() // Cambia SqlConnection a MySqlConnection si usas MySQL
        {
            // Si _sqlConnection no existe o está cerrada, crea una nueva.
            // Este patrón de mantener una conexión abierta puede ser problemático.
            // Es mejor crear y abrir conexiones por cada operación dentro de un bloque 'using'.
            if (_sqlConnection == null || _sqlConnection.State == System.Data.ConnectionState.Closed || _sqlConnection.State == System.Data.ConnectionState.Broken)
            {
                _sqlConnection = new SqlConnection(_connectionString); // O new MySqlConnection(_connectionString)
            }
            return _sqlConnection;
        }

        public void Connect()
        {
            try
            {
                if (Conexion().State != System.Data.ConnectionState.Open)
                {
                    Conexion().Open();
                    Console.WriteLine("Conexión a la base de datos abierta exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a la base de datos: {ex.ToString()}");
                throw;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_sqlConnection != null && _sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    _sqlConnection.Close();
                    Console.WriteLine("Conexión a la base de datos cerrada exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desconectar de la base de datos: {ex.ToString()}");
            }
        }
    }
}
