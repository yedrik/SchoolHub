using SchoolHub.dtos;
using SchoolHub.repositories.Models;
using System;
using System.Collections.Generic;

namespace SchoolHub.Services
{
    public class CursoService
    {
        private readonly CursoRepository _cursoRepository;

        public CursoService(CursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository ?? throw new ArgumentNullException(nameof(cursoRepository));
        }

        public ResultadoOperacion RegistrarCurso(CursoDto curso)
        {
            if (curso == null)
                return new ResultadoOperacion(false, "Los datos del curso no pueden ser nulos");

            if (string.IsNullOrWhiteSpace(curso.Nombre))
                return new ResultadoOperacion(false, "El nombre del curso es requerido");

            try
            {
                int idCurso = _cursoRepository.RegistrarCurso(curso);
                return idCurso > 0
                    ? new ResultadoOperacion(true, "Curso registrado correctamente", idCurso)
                    : new ResultadoOperacion(false, "No se pudo registrar el curso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar curso: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al registrar el curso");
            }
        }

        public List<CursoDto> ObtenerTodosLosCursos()
        {
            try
            {
                return _cursoRepository.ObtenerCursos() ?? new List<CursoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener cursos: {ex.Message}");
                return new List<CursoDto>();
            }
        }

        public CursoDto ObtenerCursoPorId(int idCurso)
        {
            if (idCurso <= 0)
                return null;

            try
            {
                return _cursoRepository.ObtenerCursoPorId(idCurso);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener curso {idCurso}: {ex.Message}");
                return null;
            }
        }
    }
}