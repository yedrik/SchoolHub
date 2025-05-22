using System;
using System.Collections.Generic;
using SchoolHub.dtos;
using SchoolHub.repositories.Models;

namespace SchoolHub.Services
{
    /// <summary>
    /// Servicio que gestiona las operaciones relacionadas con las tareas de los estudiantes.
    /// </summary>
    public class TareaService
    {
        private readonly TareaRepository TareaRepository;

        /// <summary>
        /// Constructor que utiliza inyección de dependencias
        /// </summary>
        /// <param name="tareaRepository">Repositorio de tareas</param>
        public TareaService(TareaRepository tareaRepository)
        {
            TareaRepository = tareaRepository ?? throw new ArgumentNullException(nameof(tareaRepository));
        }

        /// <summary>
        /// Registra una nueva tarea en el sistema
        /// </summary>
        /// <param name="tarea">Datos de la tarea</param>
        /// <returns>Resultado de la operación</returns>
        public ResultadoOperacion RegistrarTarea(TareaDto tarea)
        {
            if (tarea == null)
                return new ResultadoOperacion(false, "El objeto tarea no puede ser nulo");

            if (string.IsNullOrWhiteSpace(tarea.Titulo))
                return new ResultadoOperacion(false, "El título de la tarea es requerido");

            if (tarea.IdEstudiante <= 0)
                return new ResultadoOperacion(false, "ID de estudiante inválido");

            if (tarea.FechaEntrega < DateTime.Today)
                return new ResultadoOperacion(false, "La fecha de entrega no puede ser en el pasado");

            try
            {
                int resultado = TareaRepository.RegistrarTarea(tarea);
                return resultado > 0
                    ? new ResultadoOperacion(true, "Tarea registrada correctamente")
                    : new ResultadoOperacion(false, "No se pudo registrar la tarea");
            }
            catch (Exception ex)
            {
                // En producción usar ILogger
                Console.WriteLine($"Error al registrar tarea: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al registrar la tarea");
            }
        }

        /// <summary>
        /// Obtiene todas las tareas del sistema
        /// </summary>
        public List<TareaDto> ObtenerTodasLasTareas()
        {
            try
            {
                return TareaRepository.ObtenerTareas() ?? new List<TareaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tareas: {ex.Message}");
                return new List<TareaDto>();
            }
        }

        /// <summary>
        /// Obtiene las tareas de un estudiante específico
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        public List<TareaDto> ObtenerTareasPorEstudiante(int idEstudiante)
        {
            if (idEstudiante <= 0)
                return new List<TareaDto>();

            try
            {
                return TareaRepository.ObtenerTareasPorEstudiante(idEstudiante) ?? new List<TareaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tareas para estudiante {idEstudiante}: {ex.Message}");
                return new List<TareaDto>();
            }
        }

        public TareaRepository GetTareaRepository()
        {
            return TareaRepository;
        }

        /// <summary>
        /// Elimina una tarea del sistema
        /// </summary>
        /// <param name="idTarea">ID de la tarea a eliminar</param>
        public ResultadoOperacion EliminarTarea(int idTarea, TareaRepository tareaRepository)
        {
            if (idTarea <= 0)
                return new ResultadoOperacion(false, "ID de tarea inválido");

            try
            {
                bool resultado = TareaRepository.EliminarTarea(idTarea);
                return resultado
                    ? new ResultadoOperacion(true, "Tarea eliminada correctamente")
                    : new ResultadoOperacion(false, "No se encontró la tarea especificada");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar tarea {idTarea}: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al eliminar la tarea");
            }
        }
    }

    /// <summary>
    /// Clase para representar el resultado de las operaciones
    /// </summary>
    public class ResultadoOperacion
    {
        private bool v1;
        private string v2;

        public bool Exito { get; }
        public string Mensaje { get; }

        public ResultadoOperacion(bool exito, string mensaje, int idCurso)
        {
            Exito = exito;
            Mensaje = mensaje;
        }

        public ResultadoOperacion(bool v1, string v2, UserDto usuarioRegistrado)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public ResultadoOperacion(bool v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}