using SchoolHub.dtos;
using SchoolHub.repositories.Models;
using System.Linq;

namespace SchoolHub.Services
{
    /// <summary>
    /// Servicio encargado de generar reportes relacionados con calificaciones y tareas de los estudiantes.
    /// </summary>
    public class ReporteService
    {
        private readonly CalificacionesRepository _calificacionRepository;
        private readonly TareaRepository _tareaRepository;

        // Se recomienda recibir las dependencias a través del constructor para facilitar las pruebas
        public ReporteService(CalificacionesRepository calificacionRepository, TareaRepository tareaRepository)
        {
            _calificacionRepository = calificacionRepository ?? throw new ArgumentNullException(nameof(calificacionRepository));
            _tareaRepository = tareaRepository ?? throw new ArgumentNullException(nameof(tareaRepository));
        }

        /// <summary>
        /// Obtiene el reporte de calificaciones para un estudiante específico.
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        /// <returns>Lista de calificaciones del estudiante, o una lista vacía en caso de error.</returns>
        public List<CalificacionDto> ObtenerReporteCalificaciones(int idEstudiante)
        {
            try
            {
                {
                    // Llama al método correcto del repositorio para obtener las calificaciones por estudiante.
                    // Los parámetros adicionales que estabas pasando no son estándar y probablemente causaban errores.
                    return _calificacionRepository.ObtenerCalificacionesPorEstudiante(idEstudiante);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener reporte de calificaciones para el estudiante {idEstudiante}: {ex.Message}");
                // Considera loggear el error detalladamente en un sistema de logging.
                return new List<CalificacionDto>();
            }
        }

        /// <summary>
        /// Obtiene el reporte de tareas para un estudiante específico.
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        /// <returns>Lista de tareas asignadas al estudiante, o una lista vacía en caso de error.</returns>
        public List<TareaDto> ObtenerReporteTareas(int idEstudiante)
        {
            try
            {
                return _tareaRepository.ObtenerTareasPorEstudiante(idEstudiante);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener reporte de tareas para el estudiante {idEstudiante}: {ex.Message}");
                // Considera loggear el error detalladamente en un sistema de logging.
                return new List<TareaDto>();
            }
        }

        // Se eliminó este método interno ya que duplica la funcionalidad de ObtenerReporteCalificaciones
        // internal dynamic ObtenerReporteCalificaciones(object id_estudiante)
        // {
        //     throw new NotImplementedException();
        // }
    }

    // Se corrigió la declaración de la clase CalificacionRepository.
    // No es necesario redeclarar una clase interna si ya existe CalificacionesRepository.
    // Si CalificacionesRepository no existe en otro archivo, entonces esta sería la declaración correcta.
    // Asumiendo que CalificacionesRepository ya existe, esta clase interna se puede eliminar.
    // Si no existe, descomenta y corrige el constructor para recibir la conexión.
    // internal class CalificacionRepository : CalificacionesRepository
    // {
    //     public CalificacionRepository()
    //     {
    //     }
    //
    //     public CalificacionRepository(DBContextUtility conexion) : base(conexion)
    //     {
    //     }
    // }
}