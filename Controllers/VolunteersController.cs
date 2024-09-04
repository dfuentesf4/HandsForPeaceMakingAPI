using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HandsForPeaceMakingAPI.Data;
using HandsForPeaceMakingAPI.Models;
using HandsForPeaceMakingAPI.Services.EncryptionServices;
using System.Text.Json;

namespace HandsForPeaceMakingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VolunteersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VolunteersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Volunteers/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetVolunteers()
        {
            try
            {
                var volunteers = await _context.Volunteers
                                            .Include(v => v.Project)
                                            .Select(v => new
                                            {
                                                v.Id,
                                                v.FullName,
                                                v.Gender,
                                                v.IsActive,
                                                v.PhoneNumber,
                                                v.Role,
                                                v.ProjectId,
                                                Project = v.Project == null ? null : new
                                                {
                                                    v.Project.Id,
                                                    v.Project.Name,
                                                    v.Project.Description
                                                }
                                            })
                                            .ToListAsync();

                // Convertimos la lista de volunteers a JSON y luego la encriptamos
                string volunteersJson = JsonSerializer.Serialize(volunteers);
                string encryptedVolunteers = EncryptionHelper.Encrypt(volunteersJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedVolunteers });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/Volunteers/GetVolunteer
        [HttpPost("GetVolunteer")]
        public async Task<ActionResult<Volunteer>> GetVolunteer([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Volunteer");
                }

                var volunteer = await _context.Volunteers
                                              .Include(v => v.Project)
                                              .FirstOrDefaultAsync(v => v.Id == id);

                if (volunteer == null)
                {
                    return NotFound("The Volunteer with that information wasn't found");
                }

                string volunteerJson = JsonSerializer.Serialize(volunteer);
                string encryptedVolunteer = EncryptionHelper.Encrypt(volunteerJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedVolunteer });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/Volunteers/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateVolunteer([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var volunteer = JsonSerializer.Deserialize<Volunteer>(decryptedData);

                if (volunteer == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Volunteers.Add(volunteer);
                await _context.SaveChangesAsync();

                string volunteerJson = JsonSerializer.Serialize(volunteer);
                string encryptedVolunteer = EncryptionHelper.Encrypt(volunteerJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedVolunteer });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/Volunteers/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateVolunteer([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var volunteer = JsonSerializer.Deserialize<Volunteer>(decryptedData);

                if (volunteer == null || volunteer.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(volunteer).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.Id))
                    {
                        return NotFound("The Volunteer with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string volunteerJson = JsonSerializer.Serialize(volunteer);
                string encryptedVolunteer = EncryptionHelper.Encrypt(volunteerJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedVolunteer });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/Volunteers/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteVolunteer([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Volunteer");
                }

                var volunteer = await _context.Volunteers.FindAsync(id);
                if (volunteer == null)
                {
                    return NotFound("The Volunteer with that information wasn't found");
                }

                _context.Volunteers.Remove(volunteer);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.Id == id);
        }
    }
}
