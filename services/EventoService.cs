using SchoolHub.dtos;
using SchoolHub.repositories.Models;
using System;
using System.Collections.Generic;

namespace SchoolHub.Services
{
    public class EventoService
    {
        private readonly EventoRepository _eventoRepository;

        public EventoService(EventoRepository eventoRepository)
        {
            _eventoRepository = eventoRepository ?? throw new ArgumentNullException(nameof(eventoRepository));
        }

        public List<EventoDto> ObtenerProximosEventos(int dias = 30)
        {
            try
            {
                var eventos = _eventoRepository.ObtenerEventos();
                var fechaLimite = DateTime.Today.AddDays(dias);
                return eventos?
                    .Where(e => DateTime.Parse(e.Fecha) >= DateTime.Today &&
                               DateTime.Parse(e.Fecha) <= fechaLimite)
                    .OrderBy(e => DateTime.Parse(e.Fecha))
                    .ToList() ?? new List<EventoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener próximos eventos: {ex.Message}");
                return new List<EventoDto>();
            }
        }

        public ResultadoOperacion RegistrarEvento(EventoDto evento)
        {
            if (evento == null)
                return new ResultadoOperacion(false, "Los datos del evento no pueden ser nulos");

            if (string.IsNullOrWhiteSpace(evento.Nombre))
                return new ResultadoOperacion(false, "El nombre del evento es requerido");

            if (DateTime.TryParse(evento.Fecha, out DateTime fechaEvento) && fechaEvento < DateTime.Today)
                return new ResultadoOperacion(false, "La fecha del evento no puede ser en el pasado");

            try
            {
                int resultado = _eventoRepository.RegistrarEvento(evento);
                return resultado > 0
                    ? new ResultadoOperacion(true, "Evento registrado correctamente")
                    : new ResultadoOperacion(false, "No se pudo registrar el evento");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar evento: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al registrar el evento");
            }
        }
    }
}