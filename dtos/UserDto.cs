using System;
using System.Text.Json.Serialization; // Necesario si tienes constructores parametrizados y propiedades de solo inicialización

namespace SchoolHub.dtos
{
    public class UserDto
    {

        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Contacto { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        public bool EsValido()
        {
            return !string.IsNullOrEmpty(Correo) &&
                   !string.IsNullOrEmpty(Contrasena) &&
                   !string.IsNullOrEmpty(Nombres) &&
                   !string.IsNullOrEmpty(Apellidos);
        }

        public string ObtenerNombreCompleto()
        {
            return $"{Nombres} {Apellidos}";
        }

        public int ObtenerEdad()
        {
            var edad = DateTime.Now.Year - FechaNacimiento.Year;
            if (DateTime.Now.DayOfYear < FechaNacimiento.DayOfYear)
                edad--;
            return edad;
        }
    }
}