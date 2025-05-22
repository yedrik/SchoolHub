using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class EventoDto
    {
        public int IdEvento { get; set; }         // ID único del evento
        public string Nombre { get; set; }        // Nombre del evento
        public string Descripcion { get; set; }   // Descripción del evento
        public string Fecha { get; set; }         // Fecha del evento (formato string)
        public string Lugar { get; set; }         // Lugar donde se llevará a cabo
    }
}
