using SchoolHub.dtos;
using SchoolHub.repositories.Models;
using System;
using System.Collections.Generic;

namespace SchoolHub.Services
{
    public class AsistenciaService
    {
        private readonly AsistenciaRepository _asistenciaRepository;

        public AsistenciaService(AsistenciaRepository asistenciaRepository)
        {
            _asistenciaRepository = asistenciaRepository ?? throw new ArgumentNullException(nameof(asistenciaRepository));
        }

        public ResultadoOperacion RegistrarAsistencia(AsistenciaDto asistencia)
        {
            if (asistencia == null)
                return new ResultadoOperacion(false, "Los datos de asistencia no pueden ser nulos");

            if (asistencia.IdEstudiante <= 0)
                return new ResultadoOperacion(false, "ID de estudiante inválido");

            try
            {
                int resultado = _asistenciaRepository.RegistrarAsistencia(asistencia);
                return resultado > 0
                    ? new ResultadoOperacion(true, "Asistencia registrada correctamente")
                    : new ResultadoOperacion(false, "No se pudo registrar la asistencia");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar asistencia: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al registrar la asistencia");
            }
        }

        public List<AsistenciaDto> ObtenerAsistenciasPorEstudiante(int idEstudiante)
        {
            if (idEstudiante <= 0)
                return new List<AsistenciaDto>();

            try
            {
                return _asistenciaRepository.ObtenerAsistenciasPorEstudiante(idEstudiante)
                    ?? new List<AsistenciaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener asistencias para estudiante {idEstudiante}: {ex.Message}");
                return new List<AsistenciaDto>();
            }
        }
    }
}