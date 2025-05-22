using System;
using System.Security.Cryptography;
using System.Text;

namespace SchoolHub.Utilities
{
    public static class EncriptarContrasenaUtility
    {
        /// <summary>
        /// Encripta una contraseña usando SHA256.
        /// </summary>
        /// <param name="contrasena">Contraseña en texto plano</param>
        /// <returns>Contraseña encriptada en formato hexadecimal</returns>
        public static string Encriptar(string contrasena)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        /// <summary>
        /// Valida si la contraseña ingresada coincide con la contraseña encriptada.
        /// </summary>
        /// <param name="contrasenaIngresada">Contraseña ingresada por el usuario</param>
        /// <param name="contrasenaEncriptada">Contraseña encriptada almacenada</param>
        /// <returns>true si coinciden, false en caso contrario</returns>
        public static bool ValidarContrasena(string contrasenaIngresada, string contrasenaEncriptada)
        {
            string hashIngresado = Encriptar(contrasenaIngresada);
            return hashIngresado == contrasenaEncriptada;
        }

        internal static object Encriptar(object contrasena)
        {
            throw new NotImplementedException();
        }
    }
}
