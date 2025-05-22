namespace SchoolHub.dtos
{
    public class HorarioDto
    {
        public int IdHorario { get; set; }
        public string Dia { get; set; } = string.Empty;
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        // Relaciones (versión actual)
        public int IdMateria { get; set; }
        public int IdProfesor { get; set; }
        public int IdAula { get; set; }

        // Datos directos
        public string Aula { get; set; } = string.Empty;
        public int Grado { get; set; }

        // Nuevas propiedades para mostrar datos relacionados
        public string NombreMateria { get; set; } = string.Empty;
        public string NombreProfesor { get; set; } = string.Empty;
        public string ApellidoProfesor { get; set; } = string.Empty;
    }
}