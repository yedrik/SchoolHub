using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class CursoDto
    {
        public int IdCurso { get; set; }           // ID único del curso
        public string Nombre { get; set; }         // Nombre del curso
        public string Descripcion { get; set; }    // Descripción del curso
    }
}
