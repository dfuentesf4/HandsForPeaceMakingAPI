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
    public class SummaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SummaryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Summary/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetSummaries()
        {
            try
            {
                var summaries = await _context.Summaries.ToListAsync();

                // Convertimos la lista de summaries a JSON y luego la encriptamos
                string summariesJson = JsonSerializer.Serialize(summaries);
                string encryptedSummaries = EncryptionHelper.Encrypt(summariesJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedSummaries });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/Summary/GetSummary
        [HttpPost("GetSummary")]
        public async Task<ActionResult<Summary>> GetSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Summary");
                }

                var summary = await _context.Summaries.FindAsync(id);

                if (summary == null)
                {
                    return NotFound("The Summary with that information wasn't found");
                }

                string summaryJson = JsonSerializer.Serialize(summary);
                string encryptedSummary = EncryptionHelper.Encrypt(summaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/Summary/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var summary = JsonSerializer.Deserialize<Summary>(decryptedData);

                if (summary == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Summaries.Add(summary);
                await _context.SaveChangesAsync();

                string summaryJson = JsonSerializer.Serialize(summary);
                string encryptedSummary = EncryptionHelper.Encrypt(summaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/Summary/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var summary = JsonSerializer.Deserialize<Summary>(decryptedData);

                if (summary == null || summary.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(summary).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SummaryExists(summary.Id))
                    {
                        return NotFound("The Summary with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string summaryJson = JsonSerializer.Serialize(summary);
                string encryptedSummary = EncryptionHelper.Encrypt(summaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/Summary/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Summary");
                }

                var summary = await _context.Summaries.FindAsync(id);
                if (summary == null)
                {
                    return NotFound("The Summary with that information wasn't found");
                }

                _context.Summaries.Remove(summary);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool SummaryExists(int id)
        {
            return _context.Summaries.Any(e => e.Id == id);
        }
    }
}
