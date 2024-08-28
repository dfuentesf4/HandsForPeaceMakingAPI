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
    public class PettyCashSummaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PettyCashSummaryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PettyCashSummary/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetPettyCashSummaries()
        {
            try
            {
                var summaries = await _context.PettyCashSummaries.ToListAsync();

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

        // GET: api/PettyCashSummary/GetSummary
        [HttpPost("GetSummary")]
        public async Task<ActionResult<PettyCashSummary>> GetPettyCashSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the PettyCash Summary");
                }

                var summary = await _context.PettyCashSummaries.FindAsync(id);

                if (summary == null)
                {
                    return NotFound("The PettyCash Summary with that information wasn't found");
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

        // POST: api/PettyCashSummary/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreatePettyCashSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var summary = JsonSerializer.Deserialize<PettyCashSummary>(decryptedData);

                if (summary == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.PettyCashSummaries.Add(summary);
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

        // POST: api/PettyCashSummary/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdatePettyCashSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var summary = JsonSerializer.Deserialize<PettyCashSummary>(decryptedData);

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
                    if (!PettyCashSummaryExists(summary.Id))
                    {
                        return NotFound("The PettyCash Summary with that information wasn't found");
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

        // POST: api/PettyCashSummary/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeletePettyCashSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the PettyCash Summary");
                }

                var summary = await _context.PettyCashSummaries.FindAsync(id);
                if (summary == null)
                {
                    return NotFound("The PettyCash Summary with that information wasn't found");
                }

                _context.PettyCashSummaries.Remove(summary);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool PettyCashSummaryExists(int id)
        {
            return _context.PettyCashSummaries.Any(e => e.Id == id);
        }
    }
}
