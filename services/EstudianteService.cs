using SchoolHub.dtos;
using SchoolHub.repositories.Models;
using System;
using System.Collections.Generic;

namespace SchoolHub.Services
{
    public class EstudianteService
    {
        private readonly EstudianteRepository _estudianteRepository;
        private readonly CursoRepository _cursoRepository;

        public EstudianteService(EstudianteRepository estudianteRepository, CursoRepository cursoRepository)
        {
            _estudianteRepository = estudianteRepository ?? throw new ArgumentNullException(nameof(estudianteRepository));
            _cursoRepository = cursoRepository ?? throw new ArgumentNullException(nameof(cursoRepository));
        }

        public ResultadoOperacion RegistrarEstudiante(EstudianteDto estudiante)
        {
            if (estudiante == null)
                return new ResultadoOperacion(false, "Los datos del estudiante no pueden ser nulos");

            if (string.IsNullOrWhiteSpace(estudiante.Nombres) || string.IsNullOrWhiteSpace(estudiante.Apellidos))
                return new ResultadoOperacion(false, "Nombre y apellidos son requeridos");

            try
            {
                int idEstudiante = _estudianteRepository.RegistrarEstudiante(estudiante);
                return idEstudiante > 0
                    ? new ResultadoOperacion(true, "Estudiante registrado correctamente", idEstudiante)
                    : new ResultadoOperacion(false, "No se pudo registrar el estudiante");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar estudiante: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al registrar el estudiante");
            }
        }

        public EstudianteDto ObtenerEstudiantePorId(int idEstudiante)
        {
            if (idEstudiante <= 0)
                return null;

            try
            {
                return _estudianteRepository.ObtenerEstudiantePorId(idEstudiante);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estudiante {idEstudiante}: {ex.Message}");
                return null;
            }
        }

        public List<CursoDto> ObtenerCursosInscritos(int idEstudiante)
        {
            if (idEstudiante <= 0)
                return new List<CursoDto>();

            try
            {
                return _cursoRepository.ObtenerCursosPorEstudiante(idEstudiante)
                    ?? new List<CursoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener cursos para estudiante {idEstudiante}: {ex.Message}");
                return new List<CursoDto>();
            }
        }
    }
}