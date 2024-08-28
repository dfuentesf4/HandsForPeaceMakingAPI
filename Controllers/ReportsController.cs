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
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Reports/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetReports()
        {
            try
            {
                var reports = await _context.Reports.ToListAsync();

                // Convertimos la lista de reports a JSON y luego la encriptamos
                string reportsJson = JsonSerializer.Serialize(reports);
                string encryptedReports = EncryptionHelper.Encrypt(reportsJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedReports });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/Reports/GetReport
        [HttpPost("GetReport")]
        public async Task<ActionResult<Report>> GetReport([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Report");
                }

                var report = await _context.Reports
                                           .Include(r => r.Project)
                                           .FirstOrDefaultAsync(r => r.Id == id);

                if (report == null)
                {
                    return NotFound("The Report with that information wasn't found");
                }

                string reportJson = JsonSerializer.Serialize(report);
                string encryptedReport = EncryptionHelper.Encrypt(reportJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedReport });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/Reports/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateReport([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var report = JsonSerializer.Deserialize<Report>(decryptedData);

                if (report == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Reports.Add(report);
                await _context.SaveChangesAsync();

                string reportJson = JsonSerializer.Serialize(report);
                string encryptedReport = EncryptionHelper.Encrypt(reportJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedReport });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/Reports/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateReport([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var report = JsonSerializer.Deserialize<Report>(decryptedData);

                if (report == null || report.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(report).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.Id))
                    {
                        return NotFound("The Report with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string reportJson = JsonSerializer.Serialize(report);
                string encryptedReport = EncryptionHelper.Encrypt(reportJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedReport });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/Reports/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteReport([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Report");
                }

                var report = await _context.Reports.FindAsync(id);
                if (report == null)
                {
                    return NotFound("The Report with that information wasn't found");
                }

                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}
