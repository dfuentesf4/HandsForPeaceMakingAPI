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
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetProjects()
        {
            try
            {
                var projects = await _context.Projects.ToListAsync();

                // Convertimos la lista de proyectos a JSON y luego la encriptamos
                string projectsJson = JsonSerializer.Serialize(projects);
                string encryptedProjects = EncryptionHelper.Encrypt(projectsJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedProjects });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/Projects/GetProject
        [HttpPost("GetProject")]
        public async Task<ActionResult<Project>> GetProject([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Project");
                }

                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return NotFound("The Project with that information wasn't found");
                }

                string projectJson = JsonSerializer.Serialize(project);
                string encryptedProject = EncryptionHelper.Encrypt(projectJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedProject });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/Projects/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateProject([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var project = JsonSerializer.Deserialize<Project>(decryptedData);

                if (project == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                string projectJson = JsonSerializer.Serialize(project);
                string encryptedProject = EncryptionHelper.Encrypt(projectJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedProject });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/Projects/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateProject([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var project = JsonSerializer.Deserialize<Project>(decryptedData);

                if (project == null || project.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(project).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound("The Project with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string projectJson = JsonSerializer.Serialize(project);
                string encryptedProject = EncryptionHelper.Encrypt(projectJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedProject });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/Projects/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteProject([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Project");
                }

                var project = await _context.Projects.FindAsync(id);
                if (project == null)
                {
                    return NotFound("The Project with that information wasn't found");
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
