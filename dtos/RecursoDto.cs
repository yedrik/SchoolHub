using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class RecursoDto
    {
        public int IdRecurso { get; set; }         // ID del recurso
        public string Nombre { get; set; }         // Nombre del recurso
        public string Tipo { get; set; }           // Tipo de recurso (PDF, video, etc.)
        public string ArchivoUrl { get; set; }     // URL del archivo
        public string Descripcion { get; set; }    // Descripción del recurso
    }
}
