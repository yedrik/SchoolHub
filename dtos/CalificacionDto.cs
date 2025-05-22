using System;

namespace SchoolHub.dtos
{
    public class CalificacionDto
    {
        public int IdCalificacion { get; set; }
        public int IdEstudiante { get; set; }
        public int IdMateria { get; set; }
        public double Nota { get; set; }
        public string NombreMateria { get; set; }
        public DateTime Fecha { get; set; }
        public string Periodo { get; set; } = string.Empty;

        public bool EsValido()
        {
            return IdEstudiante > 0 &&
                   IdMateria > 0 &&
                   Nota >= 0 &&
                   !string.IsNullOrEmpty(Periodo); // Validación añadida para Periodo
        }
    }
}