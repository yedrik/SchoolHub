using SchoolHub.dtos;
using SchoolHub.repositories.Models;


namespace SchoolHub.Services
{
    public class CalificacionService
    {
        private readonly CalificacionesRepository _calificacionRepository;

        public CalificacionService(CalificacionesRepository calificacionRepository)
        {
            _calificacionRepository = calificacionRepository ?? throw new ArgumentNullException(nameof(calificacionRepository));
        }

        public ResultadoOperacion RegistrarCalificacion(CalificacionDto calificacion)
        {
            if (calificacion == null)
                return new ResultadoOperacion(false, "Los datos de calificación no pueden ser nulos");

            if (!calificacion.EsValido())
                return new ResultadoOperacion(false, "Los datos de calificación no son válidos");

            try
            {
                int resultado = _calificacionRepository.RegistrarCalificacion(calificacion);
                return resultado > 0
                    ? new ResultadoOperacion(true, "Calificación registrada correctamente")
                    : new ResultadoOperacion(false, "No se pudo registrar la calificación");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar calificación: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al registrar la calificación");
            }
        }

        public List<CalificacionDto> ObtenerCalificacionesPorEstudiante(int idEstudiante)
        {
            if (idEstudiante <= 0)
                return new List<CalificacionDto>();

            try
            {
                return _calificacionRepository.ObtenerCalificacionesPorEstudiante(idEstudiante)
                    ?? new List<CalificacionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener calificaciones para estudiante {idEstudiante}: {ex.Message}");
                return new List<CalificacionDto>();
            }
        }

        public double? ObtenerPromedioPorMateria(int idEstudiante, string nombreMateria)
        {
            if (idEstudiante <= 0 || string.IsNullOrWhiteSpace(nombreMateria))
                return null;

            try
            {
                
                var calificaciones = ObtenerCalificacionesPorEstudiante(idEstudiante);

                var notasDeMateria = calificaciones
                    .Where(c => c.NombreMateria != null && c.NombreMateria.Equals(nombreMateria, StringComparison.OrdinalIgnoreCase))
                    .Select(c => c.Nota);

                if (!notasDeMateria.Any()) // Verificar si hay alguna calificación antes de calcular el promedio
                {
                    return null; // O 0, o lanzar una excepción, según tu lógica de negocio
                }
                return notasDeMateria.Average();
            }
            catch (InvalidOperationException ex) // Average puede lanzar esto si la secuencia está vacía
            {
                Console.WriteLine($"No se encontraron calificaciones para {nombreMateria} para el estudiante {idEstudiante}: {ex.Message}");
                return null; // O manejar como prefieras
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al calcular promedio para {nombreMateria}: {ex.Message}");
                return null;
            }
        }
    }
}