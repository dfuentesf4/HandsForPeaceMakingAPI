using HandsForPeaceMakingAPI.Data;
using HandsForPeaceMakingAPI.Models;
using HandsForPeaceMakingAPI.Services.EncryptionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;
using System.Text.Json;

namespace HandsForPeaceMakingAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                // Convertimos la lista de usuarios a JSON y luego la encriptamos
                string usersJson = JsonSerializer.Serialize(users);
                string encryptedUsers = EncryptionHelper.Encrypt(usersJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedUsers });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpPost("GetUser")]
        public async Task<ActionResult<User>> GetUser([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the User");
                }

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("The User with that information wasn't found");
                }

                string userJson = JsonSerializer.Serialize(user);
                string encryptedUser = EncryptionHelper.Encrypt(userJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedUser });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateUser([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var user = JsonSerializer.Deserialize<User>(decryptedData);

                if (user == null)
                {
                    return BadRequest("Invalid data");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                string userJson = JsonSerializer.Serialize(user);
                string encryptedUser = EncryptionHelper.Encrypt(userJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedUser });
            }
            catch (DbUpdateException dbEx)
            {
                // Catch SQL-related exceptions and return the error message
                var sqlException = dbEx.InnerException as PostgresException;
                if (sqlException != null)
                {
                    return BadRequest("SQL Error: " + sqlException.Message);
                }

                return BadRequest("Database update error: " + dbEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                // Desencriptar y deserializar los datos
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var updatedUser = JsonSerializer.Deserialize<User>(decryptedData);

                if (updatedUser == null || updatedUser.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                // Buscar el usuario original en la base de datos
                var existingUser = await _context.Users.FindAsync(updatedUser.Id);
                if (existingUser == null)
                {
                    return NotFound("The User with that information wasn't found");
                }

                // Actualizar los campos permitidos
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.Email = updatedUser.Email;
                existingUser.PhoneNumber = updatedUser.PhoneNumber;
                existingUser.JobPosition = updatedUser.JobPosition;
                existingUser.BirthDate = updatedUser.BirthDate;
                existingUser.Gender = updatedUser.Gender;
                existingUser.IsActive = updatedUser.IsActive; 

                // Guardar cambios
                await _context.SaveChangesAsync();

                // Encriptar la respuesta
                string userJson = JsonSerializer.Serialize(existingUser);
                string encryptedUser = EncryptionHelper.Encrypt(userJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedUser });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteUser([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the User");
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound("The User with that information wasn't found");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                // Desencriptar y deserializar los datos
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var userRequest = JsonSerializer.Deserialize<User>(decryptedData);

                if (userRequest == null || string.IsNullOrEmpty(userRequest.UserName) || string.IsNullOrEmpty(userRequest.Password))
                {
                    return BadRequest("Invalid login data");
                }

                // Buscar al usuario por su UserName
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userRequest.UserName);

                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Verificar que la contraseña es correcta
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userRequest.Password, user.Password);
                if (!isPasswordValid)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Aquí puedes generar un token JWT si lo deseas o devolver una respuesta exitosa
                // Ejemplo sin JWT, solo como respuesta exitosa:
                var response = new User
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                string responseJson = JsonSerializer.Serialize(response);
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
                string utf8ResponseJson = Encoding.UTF8.GetString(responseBytes);

                string encryptedResponse = EncryptionHelper.Encrypt(utf8ResponseJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedResponse });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or processing login: " + ex.Message);
            }
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

