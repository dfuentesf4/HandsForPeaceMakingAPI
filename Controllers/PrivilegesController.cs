using HandsForPeaceMakingAPI.Data;
using HandsForPeaceMakingAPI.Models;
using HandsForPeaceMakingAPI.Services.EncryptionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HandsForPeaceMakingAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PrivilegesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PrivilegesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetPrivileges()
        {
            try
            {
                var privileges = await _context.Privileges.ToListAsync();

                // Convertir la lista de privilegios a JSON y luego encriptarla
                string privilegesJson = JsonSerializer.Serialize(privileges);
                string encryptedPrivileges = EncryptionHelper.Encrypt(privilegesJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedPrivileges });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpPost("GetPrivilege")]
        public async Task<ActionResult<Privilege>> GetPrivilege([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Privilege");
                }

                var privilege = await _context.Privileges.FindAsync(id);

                if (privilege == null)
                {
                    return NotFound("The Privilege with that information wasn't found");
                }

                string privilegeJson = JsonSerializer.Serialize(privilege);
                string encryptedPrivilege = EncryptionHelper.Encrypt(privilegeJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedPrivilege });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreatePrivilege([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var privilege = JsonSerializer.Deserialize<Privilege>(decryptedData);

                if (privilege == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Privileges.Add(privilege);
                await _context.SaveChangesAsync();

                string privilegeJson = JsonSerializer.Serialize(privilege);
                string encryptedPrivilege = EncryptionHelper.Encrypt(privilegeJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedPrivilege });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdatePrivilege([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var privilege = JsonSerializer.Deserialize<Privilege>(decryptedData);

                if (privilege == null || privilege.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(privilege).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrivilegeExists(privilege.Id))
                    {
                        return NotFound("The Privilege with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string privilegeJson = JsonSerializer.Serialize(privilege);
                string encryptedPrivilege = EncryptionHelper.Encrypt(privilegeJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedPrivilege });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> DeletePrivilege([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Privilege");
                }

                var privilege = await _context.Privileges.FindAsync(id);
                if (privilege == null)
                {
                    return NotFound("The Privilege with that information wasn't found");
                }

                _context.Privileges.Remove(privilege);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool PrivilegeExists(int id)
        {
            return _context.Privileges.Any(e => e.Id == id);
        }
    }
}
