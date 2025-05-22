using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class AsistenciaDto
    {
        public int IdAsistencia { get; set; }
        public int IdEstudiante { get; set; }
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}