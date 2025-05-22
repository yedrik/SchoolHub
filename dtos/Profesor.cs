using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class ProfesorDto
    {
        public int IdProfesor { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool EnLinea { get; set; }
    }
}

