using System;
using System.IO;

namespace SchoolHub.Utilities
{
    public class ArchivoUtility
    {
        /// <summary>
        /// Guarda un archivo en el sistema de archivos.
        /// </summary>
        /// <param name="path">Ruta completa donde se desea guardar el archivo.</param>
        /// <param name="archivo">Contenido binario del archivo.</param>
        /// <param name="mensaje">Mensaje informativo o de error.</param>
        /// <returns>true si el archivo se guarda correctamente, false si ocurre un error.</returns>
        public bool GuardarArchivo(string path, byte[] archivo, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    mensaje = "La ruta del archivo no puede estar vacía.";
                    return false;
                }

                if (archivo == null || archivo.Length == 0)
                {
                    mensaje = "El contenido del archivo está vacío.";
                    return false;
                }

                var rutaCompleta = Path.GetFullPath(path);
                File.WriteAllBytes(rutaCompleta, archivo);
                mensaje = "Archivo guardado correctamente.";
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                mensaje = "No tienes permisos suficientes para guardar el archivo.";
                return false;
            }
            catch (IOException ioEx)
            {
                mensaje = "Error de entrada/salida: " + ioEx.Message;
                return false;
            }
            catch (Exception ex)
            {
                mensaje = "Error inesperado al guardar el archivo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Lee un archivo del sistema de archivos.
        /// </summary>
        /// <param name="path">Ruta completa del archivo.</param>
        /// <returns>Contenido binario del archivo.</returns>
        public byte[] LeerArchivo(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("La ruta del archivo no puede estar vacía.", nameof(path));

            try
            {
                var rutaCompleta = Path.GetFullPath(path);
                return File.ReadAllBytes(rutaCompleta);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("El archivo no fue encontrado en la ruta especificada.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("No tienes permisos suficientes para leer el archivo.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error inesperado al leer el archivo: " + ex.Message);
            }
        }
    }
}
