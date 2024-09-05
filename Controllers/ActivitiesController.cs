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
    public class ActivitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivitiesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Activities/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetActivities()
        {
            try
            {
                var activities = await _context.Activities
                                                .Include(a => a.Project)
                                                .Select(a => new 
                                                {
                                                    a.Id,
                                                    a.Name,
                                                    a.Description,
                                                    a.StartDate,
                                                    a.EndDate,
                                                    a.IsActive,
                                                    a.ProjectId,
                                                    Project = a.Project == null ? null : new
                                                    {
                                                        a.Project.Id,
                                                        a.Project.Name,
                                                        a.Project.Description
                                                    }
                                                })
                                                .ToListAsync();

                // Convertimos la lista de activities a JSON y luego la encriptamos
                string activitiesJson = JsonSerializer.Serialize(activities);
                string encryptedActivities = EncryptionHelper.Encrypt(activitiesJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedActivities });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/Activities/GetActivity
        [HttpPost("GetActivity")]
        public async Task<ActionResult<Activity>> GetActivity([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Activity");
                }

                var activity = await _context.Activities
                                             .Include(a => a.Project)
                                             .FirstOrDefaultAsync(a => a.Id == id);

                if (activity == null)
                {
                    return NotFound("The Activity with that information wasn't found");
                }

                string activityJson = JsonSerializer.Serialize(activity);
                string encryptedActivity = EncryptionHelper.Encrypt(activityJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedActivity });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/Activities/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateActivity([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var activity = JsonSerializer.Deserialize<Activity>(decryptedData);

                if (activity == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();

                string activityJson = JsonSerializer.Serialize(activity);
                string encryptedActivity = EncryptionHelper.Encrypt(activityJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedActivity });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/Activities/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateActivity([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var activity = JsonSerializer.Deserialize<Activity>(decryptedData);

                if (activity == null || activity.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(activity).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.Id))
                    {
                        return NotFound("The Activity with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string activityJson = JsonSerializer.Serialize(activity);
                string encryptedActivity = EncryptionHelper.Encrypt(activityJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedActivity });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/Activities/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteActivity([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Activity");
                }

                var activity = await _context.Activities.FindAsync(id);
                if (activity == null)
                {
                    return NotFound("The Activity with that information wasn't found");
                }

                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.Id == id);
        }
    }
}
