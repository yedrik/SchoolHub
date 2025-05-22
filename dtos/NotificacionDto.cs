using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class NotificacionDto
    {
        public int IdNotificacion { get; set; }   // Identificador único de la notificación
        public int IdUsuario { get; set; }        // Usuario que recibe la notificación
        public string Mensaje { get; set; }       // Contenido de la notificación
        public string Fecha { get; set; }         // Fecha de emisión de la notificación
        public bool Leido { get; set; }           // Indica si fue leída o no
    }
}
