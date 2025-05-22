using System;
using System.Net;
using System.Net.Mail;

namespace SchoolHub.Utilities
{
    public class CorreoUtility
    {
        // Reemplaza estos valores con los reales o usa appsettings/config
        private const string FromEmail = "tuemail@dominio.com";
        private const string FromName = "Nombre de tu app";
        private const string FromPassword = "tu_contraseña"; // ⚠ Recomendación: No dejar esto en código duro
        private const string SmtpHost = "smtp.dominio.com";
        private const int SmtpPort = 587;

        /// <summary>
        /// Envía un correo electrónico.
        /// </summary>
        /// <param name="to">Correo de destino</param>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="body">Contenido del mensaje</param>
        /// <returns>True si se envió correctamente, false si hubo error</returns>
        public bool EnviarCorreo(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
            {
                Console.WriteLine("Datos de entrada inválidos para el correo.");
                return false;
            }

            try
            {
                var fromAddress = new MailAddress(FromEmail, FromName);
                var toAddress = new MailAddress(to);

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true  // Cambia a false si no deseas HTML
                })
                {
                    using (var smtp = new SmtpClient
                    {
                        Host = SmtpHost,
                        Port = SmtpPort,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(fromAddress.Address, FromPassword),
                        Timeout = 20000
                    })
                    {
                        smtp.Send(message);
                    }
                }

                return true;
            }
            catch (FormatException fe)
            {
                Console.WriteLine("Formato de correo inválido: " + fe.Message);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("Error al enviar el correo: " + smtpEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado al enviar correo: " + ex.Message);
            }

            return false;
        }
    }
}
