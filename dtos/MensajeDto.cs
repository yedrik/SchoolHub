using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class MensajeDto
    {
        public int IdMensaje { get; set; }         // ID del mensaje
        public int IdRemitente { get; set; }       // ID del usuario que envía
        public int IdEmisor { get; set; }
        public int IdDestinatario { get; set; }    // ID del usuario que recibe
        public string Asunto { get; set; }         // Asunto del mensaje
        public string Contenido { get; set; }      // Cuerpo del mensaje
        public string Fecha { get; set; }          // Fecha de envío
        public string en_linea { get; set; }
    }
}
