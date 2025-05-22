using SchoolHub.dtos;
using SchoolHub.repositories.Models;
using System;
using System.Collections.Generic;

namespace SchoolHub.Services
{
    public class RecursoService
    {
        private readonly RecursoRepository _recursoRepository;

        public RecursoService(RecursoRepository recursoRepository)
        {
            _recursoRepository = recursoRepository ?? throw new ArgumentNullException(nameof(recursoRepository));
        }

        public List<RecursoDto> ObtenerRecursosPorTipo(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return new List<RecursoDto>();

            try
            {
                var recursos = _recursoRepository.ObtenerRecursos();
                return recursos?
                    .Where(r => r.Tipo?.Equals(tipo, StringComparison.OrdinalIgnoreCase) ?? false)
                    .ToList() ?? new List<RecursoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener recursos por tipo {tipo}: {ex.Message}");
                return new List<RecursoDto>();
            }
        }

        public ResultadoOperacion SubirRecurso(RecursoDto recurso)
        {
            if (recurso == null)
                return new ResultadoOperacion(false, "Los datos del recurso no pueden ser nulos");

            if (string.IsNullOrWhiteSpace(recurso.Nombre) || string.IsNullOrWhiteSpace(recurso.ArchivoUrl))
                return new ResultadoOperacion(false, "Nombre y URL del archivo son requeridos");

            try
            {
                int resultado = _recursoRepository.RegistrarRecurso(recurso);
                return resultado > 0
                    ? new ResultadoOperacion(true, "Recurso subido correctamente")
                    : new ResultadoOperacion(false, "No se pudo subir el recurso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir recurso: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al subir el recurso");
            }
        }
    }
}