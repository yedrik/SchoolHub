using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging; // Para ILogger
using SchoolHub.dtos; // Asegúrate que este es el namespace correcto para tus DTOs
using SchoolHub.repositories.Models; // Asegúrate que es el namespace para UsuarioRepository
using System; // Para Exception
using System.Collections.Generic;

using System.Text.Json;

namespace SchoolHub.Controllers // Asegúrate que este es el namespace de tus controladores
{
    public class HomeController : Controller
    {
        // Campos para las dependencias inyectadas
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ProfesorRepository _profesorRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        // Si necesitas otros repositorios/servicios en otras acciones, también los inyectarías aquí
        // private readonly TareaRepository _tareaRepository; 

        // Constructor modificado para Inyección de Dependencias
        public HomeController(
            UsuarioRepository usuarioRepository,
            ProfesorRepository profesorRepository,
            ILogger<HomeController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _profesorRepository = profesorRepository ?? throw new ArgumentException(nameof(profesorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        // Acción de login (GET)
        [HttpGet]
        public IActionResult IniciarSesion()
        {
            var usuario = ObtenerUsuarioDeCookie();
            if (usuario != null)
            {
                return RedirectToAction("Dashboard");
            }
            ViewBag.Message = "Login Pagina.";
            return View();
        }

        // Acción de login (POST)
        [HttpPost]
        public IActionResult IniciarSesion(string correo, string contrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.ErrorMessage = "Debe ingresar correo y contraseña";
                return View(); // Regresa a la vista de login GET
            }

            // Llama al método de validación (que ahora debería usar _usuarioRepository)
            var usuarioValido = ValidarCredenciales(correo, contrasena);

            if (usuarioValido != null && usuarioValido.IdUsuario > 0) // Verifica también que el usuario tenga un ID válido
            {
                var userJson = JsonSerializer.Serialize(usuarioValido);

                // Cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddHours(1),
                    IsEssential = true,
                    Path = "/" //Disponible para todo el sitio
                };

                _httpContextAccessor.HttpContext.Response.Cookies.Append("UserLogged", userJson, cookieOptions);

                return RedirectToAction("Dashboard");
            }

            ViewBag.ErrorMessage = usuarioValido?.Mensaje ?? "Credenciales incorrectas o usuario no encontrado.";
            return View(); // Regresa a la vista de login GET
        }

        // Método de validación. AHORA DEBERÍA USAR _usuarioRepository.IniciarSesion
        private UserDto ValidarCredenciales(string correo, string contrasena)
        {
            _logger.LogInformation($"Intentando validar credenciales para el correo: {correo}");
            try
            {
                // Llama al método IniciarSesion de tu UsuarioRepository
                UserDto usuario = _usuarioRepository.IniciarSesion(correo, contrasena);

                if (usuario != null && usuario.IdUsuario > 0 && (usuario.Mensaje == null || usuario.Mensaje.Contains("exitoso")))
                {
                     _logger.LogInformation($"Validación exitosa para: {correo}");
                    return usuario;
                }
                else
                {
                    _logger.LogWarning($"Validación fallida para: {correo}. Mensaje del repo: {usuario?.Mensaje}");
                    // Devuelve el usuario con el mensaje de error del repositorio si existe
                    return usuario ?? new UserDto { Mensaje = "Credenciales inválidas o error desconocido." };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en ValidarCredenciales para el correo: {correo}");
                return new UserDto { Mensaje = "Ocurrió un error durante la validación." };
            }
        }

        public IActionResult CerrarSesion()
        {

            // Eliminar la cookie
            if (_httpContextAccessor.HttpContext.Request.Cookies["UserLogged"] != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete("UserLogged", new CookieOptions { Path = "/" });
                _logger.LogInformation("Cookie 'UserLogged' eliminada.");
            }
            return RedirectToAction("IniciarSesion");
        }

        public IActionResult Tarea()
        {
            var usuario = ObtenerUsuarioDeCookie();
            if (usuario == null) return RedirectToAction("IniciarSesion");
            ViewBag.Usuario = usuario;

          
            // Esto debería usar _tareaRepository si lo inyectas
            var tareas = ObtenerTareas(usuario.IdUsuario);
            ViewBag.Tareas = tareas ?? new List<TareaDto>();

            return View();
        }

        // Este método debería usar _tareaRepository (inyectado)
        private List<TareaDto> ObtenerTareas(int idUsuario)
        {
            _logger.LogInformation($"Obteniendo tareas para el usuario ID: {idUsuario}");
            // EJEMPLO: return _tareaRepository.ObtenerTareasPorUsuario(idUsuario);
            // Por ahora, devolviendo datos de ejemplo:
            return new List<TareaDto>
            {
                new TareaDto {
                    IdTarea = 1, Titulo = "Tarea de ejemplo desde HomeController", Descripcion = "Descripción...",
                    FechaEntrega = DateTime.Now.AddDays(7), Estado = "Pendiente", IdEstudiante = idUsuario
                }
            };
        }

        public IActionResult Dashboard()
        {
            var usuario = ObtenerUsuarioDeCookie();
            if (usuario == null)
            {
                return RedirectToAction("IniciarSesion");
            }

            ViewBag.Usuario = usuario;
            // Ajustado para usar la estructura de UserDto con PersonaDto anidado
            ViewBag.Message = $"Bienvenido, {usuario.Nombres ?? "Usuario"} {usuario.Apellidos ?? ""}";
            return View();
        }

        // En HomeController.cs
        public UserDto? ObtenerUsuarioDeCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                _logger.LogWarning("HttpContext es null.");
                return null;
            }

            var cookieValor = httpContext.Request.Cookies["UserLogged"];
            _logger.LogInformation($"Valor cookie UserLogged (crudo): {cookieValor}");

            if (string.IsNullOrEmpty(cookieValor))
            {
                _logger.LogInformation("Cookie 'UserLogged' no existe o está vacía.");
                return null;
            }

            var jsonDesencoded = Uri.UnescapeDataString(cookieValor);
            _logger.LogInformation($"Valor cookie UserLogged (decodificado): {jsonDesencoded}");

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var usuario = JsonSerializer.Deserialize<UserDto>(jsonDesencoded, options);
                if (usuario == null)
                {
                    _logger.LogWarning("Deserialización devolvió null.");
                }
                else
                {
                    _logger.LogInformation($"Usuario deserializado: {usuario.Nombres} {usuario.Apellidos}");
                }
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializando cookie.");
                httpContext.Response.Cookies.Delete("UserLogged");
                return null;
            }
        }



        public IActionResult MostrarPerfil()
        {
            var usuario = ObtenerUsuarioDeCookie();
            if (usuario == null)
            {
                return RedirectToAction("IniciarSesion");
            }
            return View(usuario);
        }

        public IActionResult Index()
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies["UserLogged"] != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        // ----- NUEVA ACCIÓN PARA PROBAR LA CONEXIÓN A LA BASE DE DATOS -----
        public IActionResult TestConexionDB()
        {
            string mensajeTest = "";
            try
            {
                _logger.LogInformation("Iniciando prueba de conexión a la base de datos...");
                // Intenta obtener el usuario con ID 1.
                // Si tu tabla de usuarios está vacía o no existe el ID 1,
                // la conexión aún podría ser exitosa pero no se encontraría el usuario.
                UserDto usuario = _usuarioRepository.ObtenerUsuarioPorId(1);

                if (usuario != null && usuario.IdUsuario != 0)
                {
                    mensajeTest = $"¡Conexión exitosa! Usuario ID {usuario.IdUsuario} encontrado: {usuario.Nombres ?? "N/A"} {usuario.Apellidos ?? ""}. Mensaje del DTO: {usuario.Mensaje}";
                    _logger.LogInformation(mensajeTest);
                }
                else if (usuario != null && (usuario.Mensaje ?? "").Contains("no encontrado"))
                {
                    mensajeTest = $"Conexión a la base de datos exitosa, pero el usuario con ID 1 no fue encontrado. Mensaje del DTO: {usuario.Mensaje}";
                    _logger.LogInformation(mensajeTest);
                }
                else
                {
                    mensajeTest = $"Posible conexión exitosa, pero no se recuperó un usuario válido o el usuario ID 1 no existe. Mensaje del DTO: {usuario?.Mensaje ?? "Sin mensaje."}";
                    _logger.LogWarning(mensajeTest);
                }
            }
            catch (SqlException sqlEx) // Captura errores específicos de SQL
            {
                mensajeTest = $"ERROR DE SQL: No se pudo conectar o consultar la base de datos. Detalles: {sqlEx.Message}";
                _logger.LogError(sqlEx, "Error de SQL durante la prueba de conexión.");
            }
            catch (InvalidOperationException invOpEx) // Puede ser por cadena de conexión no encontrada en DBContextUtility
            {
                 mensajeTest = $"ERROR DE OPERACIÓN INVÁLIDA: {invOpEx.Message}. ¿Está la cadena de conexión en appsettings.json y DBContextUtility la lee bien?";
                _logger.LogError(invOpEx, "Error de operación inválida durante la prueba de conexión.");
            }
            catch (Exception ex) // Captura cualquier otro error
            {
                mensajeTest = $"ERROR GENERAL: Ocurrió un problema. Detalles: {ex.Message}";
                _logger.LogError(ex, "Error general durante la prueba de conexión.");
            }

            ViewBag.DbTestMessage = mensajeTest;
            // Asegúrate de tener una vista Views/Home/TestConexionDB.cshtml
            return View();
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(UserDto usuario, string confirmarContrasena){
            _logger.LogInformation($"Intento de registro para el correo: {usuario?.Correo}");

            // 1. Validar ModelState (si usas DataAnnotations en UserDto)
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState no es válido para el registro.");
               
                return View(usuario);
            }

            // 2. Validación personalizada adicional
            if (string.IsNullOrWhiteSpace(usuario.Nombres) ||
                string.IsNullOrWhiteSpace(usuario.Apellidos) ||
                string.IsNullOrWhiteSpace(usuario.Correo) ||
                string.IsNullOrWhiteSpace(usuario.Contrasena) ||
                usuario.IdRol <= 0) // Asumiendo que los IDs de rol son positivos
            {
                ViewBag.ErrorMessage = "Todos los campos marcados con * son obligatorios.";
                _logger.LogWarning("Faltan campos obligatorios para el registro.");
                return View(usuario);
            }

            if (usuario.Contrasena != confirmarContrasena)
            {
                ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                _logger.LogWarning("Las contraseñas no coinciden durante el registro.");
                return View(usuario); // Devuelve la vista con los datos ingresados
            }

            // (Opcional) Validar complejidad de la contraseña si es necesario (ej. longitud)
            if (usuario.Contrasena.Length < 8) // Ejemplo de validación de longitud
            {
                ViewBag.ErrorMessage = "La contraseña debe tener al menos 8 caracteres.";
                _logger.LogWarning("La contraseña es demasiado corta.");
                return View(usuario);
            }

            // (Opcional) Validar si FechaNacimiento y Genero son proporcionados si son obligatorios
            // if (usuario.FechaNacimiento == default(DateTime)) { /* Error */ }
            // if (string.IsNullOrWhiteSpace(usuario.Genero)) { /* Error */ }


            try
            {
                // 3. Verificar si el correo ya existe
                if (_usuarioRepository.ExisteCorreo(usuario.Correo))
                {
                    ViewBag.ErrorMessage = "El correo electrónico ya está registrado. Por favor, intente con otro.";
                    _logger.LogWarning($"Intento de registro con correo existente: {usuario.Correo}");
                    return View(usuario);
                }

                // 4. Preparar el DTO para el repositorio
                usuario.Activo = true; // Los nuevos usuarios suelen estar activos por defecto
                usuario.FechaCreacion = DateTime.UtcNow; // Usar UtcNow para consistencia
                                                         // La contraseña ya está en usuario.Contrasena, el repositorio la encriptará.

                // 5. Llamar al repositorio para registrar al usuario
                UserDto usuarioRegistrado = _usuarioRepository.RegistrarUsuario(usuario);

                if (usuarioRegistrado != null && usuarioRegistrado.IdUsuario > 0)
                {
                    _logger.LogInformation($"Usuario registrado exitosamente con ID: {usuarioRegistrado.IdUsuario}, Correo: {usuarioRegistrado.Correo}");
                    TempData["SuccessMessage"] = "¡Registro exitoso! Ahora puedes iniciar sesión.";
                    return RedirectToAction("IniciarSesion");

                }
                else
                {
                    ViewBag.ErrorMessage = usuarioRegistrado?.Mensaje ?? "Ocurrió un error durante el registro. Por favor, intente de nuevo.";
                    _logger.LogError($"Error al registrar usuario en el repositorio. Mensaje del repo: {usuarioRegistrado?.Mensaje}");
                    return View(usuario);
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, $"Error de base de datos durante el registro para el correo: {usuario.Correo}");
                ViewBag.ErrorMessage = "Ocurrió un error de base de datos. Por favor, intente más tarde.";
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado durante el registro para el correo: {usuario.Correo}");
                ViewBag.ErrorMessage = "Ocurrió un error inesperado. Por favor, intente más tarde.";
                return View(usuario);
            }
        }

        public IActionResult ComunicacionMensaje() => View();
        // Si Calificaciones y otras vistas necesitan datos, también deben tener lógica para obtenerlos,
        // verificar sesión y pasarlos a la vista.
        public IActionResult Calificaciones()
        {
            var usuario = ObtenerUsuarioDeCookie();
            if (usuario == null) return RedirectToAction("IniciarSesion");
            ViewBag.Usuario = usuario;
            // Aquí deberías cargar las calificaciones, estudiantes, materias desde tus repositorios/servicios
            // y pasarlos a la vista mediante ViewBag o un ViewModel.
            ViewData["calificaciones"] = new List<CalificacionDto>(); // Ejemplo vacío
            ViewData["estudiantes"] = new List<EstudianteDto>();   // Ejemplo vacío
            ViewData["materias"] = new List<MateriaDto>();     // Ejemplo vacío
            return View();
        }
        public IActionResult BuscarCorreo() => View();
        public IActionResult CambiarContrasena() => View();
        public IActionResult MostrarUsuario() => View();
        public IActionResult Cursos() => View();
        public IActionResult Calendario() => View();
        public IActionResult Clases() => View();
        public IActionResult Estudiantes() => View();
        // En HomeController.cs (o tu controlador de Profesores)

        // Asumiendo que tienes un ProfesorService inyectado:
        // private readonly ProfesorService _profesorService;
        // public HomeController(..., ProfesorService profesorService, ...)
        // {
        //     _profesorService = profesorService;
        // }

        public IActionResult Profesores()
        {
            
            var usuarioCookieJson = HttpContext.Request.Cookies["UserLogged"];
            if (string.IsNullOrEmpty(usuarioCookieJson))
            {
                return RedirectToAction("IniciarSesion", "Home"); // O a donde corresponda
            }
            UserDto usuario = JsonSerializer.Deserialize<UserDto>(usuarioCookieJson);
            ViewBag.UsuarioActual = usuario; // Para el layout o la vista

            _logger.LogInformation("Accediendo a la página de Profesores.");
            try
            {
                List<ProfesorDto> listaProfesores = _profesorRepository.ObtenerTodos();


                ViewData["profesores"] = listaProfesores;
                _logger.LogInformation($"Se encontraron {listaProfesores.Count} profesores.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de profesores para la vista.");
                ViewData["profesores"] = new List<ProfesorDto>(); // Enviar lista vacía en caso de error
                ViewBag.ErrorMessage = "Error al cargar la lista de profesores."; // Opcional: mensaje de error
            }

            return View();
        }
        public IActionResult Horarios() => View();
    }
}