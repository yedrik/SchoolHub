namespace SchoolHub.dtos
{
    public class ClaseDto
    {
        public int IdClase { get; set; }
        public int Codigo { get; set; }
        public string NombreMateria { get; set; } = string.Empty;
        public int IdMateria { get; set; }
        public int IdProfesor { get; set; }
        public string NombreProfesor { get; set; } = string.Empty;
        public int IdSalon { get; set; }
        public string NombreSalon { get; set; } = string.Empty;
        public string DiaSemana { get; set; } = string.Empty;
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int IdHorario { get; set; }
        public int IdGrado { get; set; } 
        public string Grado { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        public string HorarioFormateado => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";

        public bool EsValido()
        {
            return Codigo > 0 && // Ejemplo de validación para Codigo
                   IdMateria > 0 &&
                   IdProfesor > 0 &&
                   IdSalon > 0 &&
                   IdHorario > 0 &&
                   IdGrado > 0 && // Validar el ID del grado
                   !string.IsNullOrEmpty(DiaSemana) &&
                   HoraInicio < HoraFin;
        }
    }
}