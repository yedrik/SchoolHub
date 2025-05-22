using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class RecuperacionContrasenaDto
    {
        public int IdRecuperacion { get; set; }     // ID del intento de recuperación
        public int IdUsuario { get; set; }          // ID del usuario asociado
        public string Codigo { get; set; }          // Código de verificación
        public string Fecha { get; set; }           // Fecha de solicitud
        public string Mensaje { get; set; }         // Mensaje de respuesta (opcional)
    }
}