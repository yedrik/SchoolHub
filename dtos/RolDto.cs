using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class RolDto
    {
        public int IdRol { get; set; }         // ID del rol (por ejemplo: 1 = Admin, 2 = Profesor, etc.)
        public string Nombre { get; set; }  // Nombre descriptivo del rol
    }
}