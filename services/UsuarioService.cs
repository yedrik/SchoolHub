using SchoolHub.dtos;
using SchoolHub.repositories.Models;
using System;
using System.Collections.Generic;

namespace SchoolHub.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public ResultadoOperacion RegistrarUsuario(UserDto usuario)
        {
            if (usuario == null)
                return new ResultadoOperacion(false, "Los datos del usuario no pueden ser nulos");

            if (!usuario.EsValido())
                return new ResultadoOperacion(false, "Los datos del usuario no son válidos");

            try
            {
                var usuarioRegistrado = _usuarioRepository.RegistrarUsuario(usuario);
                return usuarioRegistrado?.IdUsuario > 0
                    ? new ResultadoOperacion(true, "Usuario registrado correctamente", usuarioRegistrado)
                    : new ResultadoOperacion(false, usuarioRegistrado?.Mensaje ?? "No se pudo registrar el usuario");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return new ResultadoOperacion(false, "Error interno al registrar el usuario");
            }
        }

        public UserDto IniciarSesion(string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
                return new UserDto { Mensaje = "Correo y contraseña son requeridos" };

            try
            {
                return _usuarioRepository.IniciarSesion(correo, contrasena);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al iniciar sesión: {ex.Message}");
                return new UserDto { Mensaje = "Error interno al iniciar sesión" };
            }
        }

        public bool ActualizarContrasena(int idUsuario, string contrasenaActual, string nuevaContrasena)
        {
            if (idUsuario <= 0 || string.IsNullOrWhiteSpace(nuevaContrasena))
                return false;

            try
            {
                // Primero verificar que la contraseña actual sea correcta
                var usuario = _usuarioRepository.ObtenerUsuarioPorId(idUsuario);
                if (usuario == null || usuario.IdUsuario <= 0)
                    return false;

                // Luego actualizar la contraseña
                return _usuarioRepository.ActualizarContrasena(idUsuario, nuevaContrasena);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar contraseña: {ex.Message}");
                return false;
            }
        }
    }
}