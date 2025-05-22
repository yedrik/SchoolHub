using Microsoft.Data.SqlClient;
using SchoolHub.dtos;
using SchoolHub.Utilities;
using System;
using System.Collections.Generic;
using System.Data;


namespace SchoolHub.repositories.Models
{
    public class UsuarioRepository
    {
        private readonly DBContextUtility _conexion;

        public UsuarioRepository(DBContextUtility conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        public UserDto RegistrarUsuario(UserDto usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            var usuarioResp = new UserDto();

            try
            {
                _conexion.Connect();
                const string sql = @"
                    INSERT INTO Usuario 
                    (id_rol, nombres, apellidos, correo, contrasena, fecha_nacimiento, genero, activo)
                    OUTPUT INSERTED.id_usuario
                    VALUES 
                    (@IdRol, @Nombres, @Apellidos, @Correo, @Contrasena, @FechaNacimiento, @Genero, 1)";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdRol", SqlDbType.Int).Value = usuario.IdRol;
                    command.Parameters.Add("@Nombres", SqlDbType.NVarChar, 100).Value = usuario.Nombres ?? (object)DBNull.Value;
                    command.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 100).Value = usuario.Apellidos ?? (object)DBNull.Value;
                    command.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = usuario.Correo;
                    command.Parameters.Add("@Contrasena", SqlDbType.NVarChar, 255).Value =
                        EncriptarContrasenaUtility.Encriptar(usuario.Contrasena);
                    command.Parameters.Add("@FechaNacimiento", SqlDbType.DateTime).Value =
                        usuario.FechaNacimiento != default ? (object)usuario.FechaNacimiento : DBNull.Value;
                    command.Parameters.Add("@Genero", SqlDbType.NVarChar, 20).Value =
                        usuario.Genero ?? (object)DBNull.Value;

                    var idUsuario = command.ExecuteScalar();
                    if (idUsuario != null && idUsuario != DBNull.Value)
                    {
                        usuarioResp = ObtenerUsuarioPorId(Convert.ToInt32(idUsuario));
                        usuarioResp.Mensaje = "Registro exitoso";
                    }
                    else
                    {
                        usuarioResp.Mensaje = "No se pudo registrar el usuario";
                    }
                }
            }
            catch (SqlException ex)
            {
                usuarioResp.Mensaje = ex.Number == 2627 ?
                    "El correo electrónico ya está registrado" :
                    $"Error en el registro: {ex.Message}";
            }
            catch (Exception ex)
            {
                usuarioResp.Mensaje = $"Error en el registro: {ex.Message}";
                // Log the full error
                Console.WriteLine($"Error en RegistrarUsuario: {ex}");
            }
            finally
            {
                _conexion.Disconnect();
            }

            return usuarioResp;
        }

        public UserDto ObtenerUsuarioPorId(int idUsuario)
        {
            var usuario = new UserDto();

            try
            {
                _conexion.Connect();
                const string sql = @"
                    SELECT u.id_usuario, u.id_rol, r.nombre AS nombre_rol, 
                           u.nombres, u.apellidos, u.correo, u.fecha_nacimiento, 
                           u.genero, u.activo, u.fecha_creacion
                    FROM Usuario u
                    INNER JOIN Rol r ON u.id_rol = r.id_rol
                    WHERE u.id_usuario = @IdUsuario";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario.IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario"));
                            usuario.IdRol = reader.GetInt32(reader.GetOrdinal("id_rol"));
                            usuario.Nombres = reader.IsDBNull(reader.GetOrdinal("nombres")) ?
                                null : reader.GetString(reader.GetOrdinal("nombres"));
                            usuario.Apellidos = reader.IsDBNull(reader.GetOrdinal("apellidos")) ?
                                null : reader.GetString(reader.GetOrdinal("apellidos"));
                            usuario.Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ?
                                null : reader.GetString(reader.GetOrdinal("correo"));
                            usuario.Genero = reader.IsDBNull(reader.GetOrdinal("genero")) ?
                                null : reader.GetString(reader.GetOrdinal("genero"));

                            if (!reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento")))
                                usuario.FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento"));

                            if (!reader.IsDBNull(reader.GetOrdinal("fecha_creacion")))
                                usuario.FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion"));

                            usuario.Activo = !reader.IsDBNull(reader.GetOrdinal("activo"));
                                reader.GetBoolean(reader.GetOrdinal("activo"));

                            usuario.Mensaje = "Usuario encontrado";
                        }
                        else
                        {
                            usuario.Mensaje = "Usuario no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                usuario.Mensaje = $"Error al obtener usuario: {ex.Message}";
                Console.WriteLine($"Error en ObtenerUsuarioPorId: {ex}");
            }
            finally
            {
                _conexion.Disconnect();
            }

            return usuario;
        }

        public List<UserDto> ListarUsuariosPorRol(int idRol)
        {
            var usuarios = new List<UserDto>();

            try
            {
                _conexion.Connect();
                const string sql = @"
                    SELECT u.id_usuario, u.nombres, u.apellidos, u.correo, r.nombre AS nombre_rol
                    FROM Usuario u
                    INNER JOIN Rol r ON u.id_rol = r.id_rol 
                    WHERE u.id_rol = @IdRol AND u.activo = 1";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdRol", SqlDbType.Int).Value = idRol;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuario = new UserDto
                            {
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Nombres = reader.IsDBNull(reader.GetOrdinal("nombres")) ?
                                    null : reader.GetString(reader.GetOrdinal("nombres")),
                                Apellidos = reader.IsDBNull(reader.GetOrdinal("apellidos")) ?
                                    null : reader.GetString(reader.GetOrdinal("apellidos")),
                                Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ?
                                    null : reader.GetString(reader.GetOrdinal("correo")),
                                IdRol = idRol
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar usuarios por rol: {ex}");
                throw;
            }
            finally
            {
                _conexion.Disconnect();
            }

            return usuarios;
        }

        public UserDto IniciarSesion(string correo, string contrasena)
        {
            var usuario = new UserDto();

            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                usuario.Mensaje = "Correo y contraseña son requeridos";
                return usuario;
            }

            try
            {
                _conexion.Connect();
                const string sql = @"
                    SELECT u.id_usuario, u.id_rol, r.nombre AS nombre_rol, 
                           u.nombres, u.apellidos, u.correo, u.contrasena, u.activo
                    FROM Usuario u
                    INNER JOIN Rol r ON u.id_rol = r.id_rol
                    WHERE u.correo = @Correo";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = correo;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var contrasenaEncriptadaDb = reader.IsDBNull(reader.GetOrdinal("contrasena"))?
                                null : reader.GetString(reader.GetOrdinal("contrasena"));

                            if (!string.IsNullOrEmpty(contrasenaEncriptadaDb) &&
                                EncriptarContrasenaUtility.ValidarContrasena(contrasena, contrasenaEncriptadaDb))
                            {
                                usuario.IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario"));
                                usuario.IdRol = reader.GetInt32(reader.GetOrdinal("id_rol"));
                                usuario.Nombres = reader.IsDBNull(reader.GetOrdinal("nombres")) ?
                                    null : reader.GetString(reader.GetOrdinal("nombres"));
                                usuario.Apellidos = reader.IsDBNull(reader.GetOrdinal("apellidos")) ?
                                    null : reader.GetString(reader.GetOrdinal("apellidos"));
                                usuario.Correo = correo;
                                usuario.Activo = reader.GetBoolean(reader.GetOrdinal("activo"));

                                usuario.Mensaje = "Inicio de sesión exitoso";
                            }
                            else
                            {
                                usuario.Mensaje = "Credenciales inválidas";
                            }
                        }
                        else
                        {
                            usuario.Mensaje = "Usuario no encontrado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                usuario.Mensaje = $"Error al iniciar sesión: {ex.Message}";
                Console.WriteLine($"Error en IniciarSesion: {ex}");
            }
            finally
            {
                _conexion.Disconnect();
            }

            return usuario;
        }

        public bool ExisteCorreo(string correo)
        {
            try
            {
                _conexion.Connect();
                const string sql = "SELECT 1 FROM Usuario WHERE correo = @Correo";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = correo;
                    using (var reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar correo: {ex}");
                throw;
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public int ActualizarUsuario(UserDto usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            try
            {
                _conexion.Connect();
                const string sql = @"
                    UPDATE Usuario 
                    SET nombres = @Nombres, 
                        apellidos = @Apellidos, 
                        correo = @Correo, 
                        fecha_nacimiento = @FechaNacimiento, 
                        genero = @Genero, 
                        id_rol = @IdRol
                    WHERE id_usuario = @IdUsuario";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@Nombres", SqlDbType.NVarChar, 100).Value =
                        usuario.Nombres ?? (object)DBNull.Value;
                    command.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 100).Value =
                        usuario.Apellidos ?? (object)DBNull.Value;
                    command.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = usuario.Correo;
                    command.Parameters.Add("@FechaNacimiento", SqlDbType.DateTime).Value =
                        usuario.FechaNacimiento != default ? (object)usuario.FechaNacimiento : DBNull.Value;
                    command.Parameters.Add("@Genero", SqlDbType.NVarChar, 20).Value =
                        usuario.Genero ?? (object)DBNull.Value;
                    command.Parameters.Add("@IdRol", SqlDbType.Int).Value = usuario.IdRol;
                    command.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = usuario.IdUsuario;

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar usuario: {ex}");
                throw;
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public bool DesactivarUsuario(int idUsuario)
        {
            try
            {
                _conexion.Connect();
                const string sql = "UPDATE Usuario SET activo = 0 WHERE id_usuario = @IdUsuario";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desactivar usuario: {ex}");
                throw;
            }
            finally
            {
                _conexion.Disconnect();
            }
        }

        public bool ActualizarContrasena(int idUsuario, string nuevaContrasena)
        {
            if (string.IsNullOrWhiteSpace(nuevaContrasena))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(nuevaContrasena));

            try
            {
                _conexion.Connect();
                const string sql = "UPDATE Usuario SET contrasena = @Contrasena WHERE id_usuario = @IdUsuario";

                using (var command = new SqlCommand(sql, _conexion.Conexion()))
                {
                    command.Parameters.Add("@Contrasena", SqlDbType.NVarChar, 255).Value =
                        EncriptarContrasenaUtility.Encriptar(nuevaContrasena);
                    command.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar contraseña: {ex}");
                throw;
            }
            finally
            {
                _conexion.Disconnect();
            }
        }
    }
}