using System;
using System.Collections.Generic;
using SchoolHub.dtos;
using SchoolHub.repositories.Models;

namespace SchoolHub.Services
{
    public class NotificationService
    {
        private readonly MensajeRepository _mensajeRepository;

        // Use dependency injection instead of direct instantiation
        public NotificationService(MensajeRepository mensajeRepository)
        {
            _mensajeRepository = mensajeRepository ?? throw new ArgumentNullException(nameof(mensajeRepository));
        }

        /// <summary>
        /// Envía una notificación (mensaje).
        /// </summary>
        /// <param name="mensaje">Objeto MensajeDto a registrar</param>
        /// <returns>Mensaje de resultado</returns>
        public string EnviarNotificacion(MensajeDto mensaje)
        {
            if (mensaje == null)
                return "El objeto mensaje no puede ser nulo.";

            if (string.IsNullOrWhiteSpace(mensaje.Contenido))
                return "El contenido del mensaje no puede estar vacío.";

            if (mensaje.IdRemitente <= 0 || mensaje.IdDestinatario <= 0)
                return "Los IDs de remitente y destinatario deben ser válidos.";

            try
            {
                int resultado = _mensajeRepository.RegistrarMensaje(mensaje);
                return resultado > 0
                    ? "Notificación enviada correctamente."
                    : "Error al enviar la notificación.";
            }
            catch (Exception ex)
            {
                // Consider using proper logging framework
                Console.WriteLine($"Error al enviar notificación de {mensaje.IdRemitente} a {mensaje.IdDestinatario}: {ex.Message}");
                return "Error inesperado al enviar la notificación.";
            }
        }

        /// <summary>
        /// Obtiene todas las notificaciones recibidas para un usuario específico.
        /// </summary>
        /// <param name="idUsuario">ID del usuario destinatario</param>
        public List<MensajeDto> ObtenerNotificacionesRecibidas(int idUsuario)
        {
            if (idUsuario <= 0)
                return new List<MensajeDto>();

            try
            {
                return _mensajeRepository.ObtenerMensajesRecibidos(idUsuario) ?? new List<MensajeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mensajes recibidos para usuario {idUsuario}: {ex.Message}");
                return new List<MensajeDto>();
            }
        }

        /// <summary>
        /// Obtiene todas las notificaciones enviadas por un usuario específico.
        /// </summary>
        /// <param name="idUsuario">ID del usuario remitente</param>
        public List<MensajeDto> ObtenerNotificacionesEnviadas(int idUsuario)
        {
            if (idUsuario <= 0)
                return new List<MensajeDto>();

            try
            {
                return _mensajeRepository.ObtenerMensajesEnviados(idUsuario) ?? new List<MensajeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mensajes enviados por usuario {idUsuario}: {ex.Message}");
                return new List<MensajeDto>();
            }
        }

        /// <summary>
        /// Obtiene mensajes de profesores para un estudiante específico.
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        public List<MensajeDto> ObtenerNotificacionesProfesores(int idEstudiante)
        {
            if (idEstudiante <= 0)
                return new List<MensajeDto>();

            try
            {
                return _mensajeRepository.ObtenerMensajesProfesores(idEstudiante) ?? new List<MensajeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mensajes de profesores para estudiante {idEstudiante}: {ex.Message}");
                return new List<MensajeDto>();
            }
        }

        /// <summary>
        /// Obtiene mensajes de administración para un usuario específico.
        /// </summary>
        /// <param name="idUsuario">ID del usuario</param>
        public List<MensajeDto> ObtenerNotificacionesAdministracion(int idUsuario)
        {
            if (idUsuario <= 0)
                return new List<MensajeDto>();

            try
            {
                return _mensajeRepository.ObtenerMensajesAdministracion(idUsuario) ?? new List<MensajeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mensajes de administración para usuario {idUsuario}: {ex.Message}");
                return new List<MensajeDto>();
            }
        }
    }
}