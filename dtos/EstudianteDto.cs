using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class EstudianteDto
    {
        public int IdEstudiante { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public decimal Promedio { get; set; }
        public string Grupo { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Grado { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}