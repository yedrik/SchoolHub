using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolHub.dtos
{
    public class TareaDto
    {
        public int IdTarea { get; set; }              // Identificador único de la tarea
        public string Titulo { get; set; }            // Título de la tarea
        public string Descripcion { get; set; }       // Descripción de la tarea
        public DateTime FechaEntrega { get; set; }    // Fecha de entrega en formato DateTime
        public string Estado { get; set; }            // Estado de la tarea (pendiente, entregada, etc.)
        public int IdEstudiante { get; set; }         // ID del estudiante asociado a la tarea
    }
}

