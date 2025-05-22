using SchoolHub.dtos;
using SchoolHub.repositories.Models; // Asegúrate que este es el namespace de tus repositorios
using System; // Para ArgumentNullException
using System.Collections.Generic; // Para List<>

namespace SchoolHub.Services
{
    public class ProfesorService
    {
        private readonly CursoRepository _cursoRepository;
        private readonly UsuarioRepository _usuarioRepository; // Añade UsuarioRepository como dependencia

        // Constructor para Inyección de Dependencias
        public ProfesorService(CursoRepository cursoRepository, UsuarioRepository usuarioRepository)
        {
            _cursoRepository = cursoRepository ?? throw new ArgumentNullException(nameof(cursoRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        // ELIMINA O COMENTA EL CONSTRUCTOR SIN PARÁMETROS:
        // public ProfesorService()
        // {
        //     // Esta forma de instanciación manual es la que causa problemas con DI
        //     // _cursoRepository = new CursoRepository(new Utilities.DBContextUtility());
        // }

        /// <summary>
        /// Obtiene los cursos asignados a un profesor.
        /// </summary>
        public List<CursoDto> ObtenerCursosAsignados(int idUsuarioProfesor)
        {
            try
            {
                // TODO: Asegúrate que _cursoRepository tenga un método adecuado.
                // Por ejemplo: return _cursoRepository.ObtenerCursosPorProfesor(idUsuarioProfesor);
                // Devolviendo una lista vacía temporalmente:
                Console.WriteLine($"ProfesorService: Buscando cursos para profesor ID {idUsuarioProfesor} usando instancia de _cursoRepository: {_cursoRepository.GetHashCode()}");
                return new List<CursoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProfesorService al obtener cursos asignados: {ex.Message}");
                return new List<CursoDto>();
            }
        }

        /// <summary>
        /// Obtiene la información básica del profesor.
        /// </summary>
        public UserDto ObtenerInformacionProfesor(int idUsuarioProfesor)
        {
            try
            {
                // Ahora usa la instancia _usuarioRepository inyectada
                Console.WriteLine($"ProfesorService: Buscando información para profesor ID {idUsuarioProfesor} usando instancia de _usuarioRepository: {_usuarioRepository.GetHashCode()}");
                return _usuarioRepository.ObtenerUsuarioPorId(idUsuarioProfesor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProfesorService al obtener información del profesor: {ex.Message}");
                return null;
            }
        }
    }
}