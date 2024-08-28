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
    public class TransfersSummaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransfersSummaryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TransfersSummary/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetTransfersSummary()
        {
            try
            {
                var transfersSummary = await _context.TransfersSummaries.ToListAsync();

                // Convertimos la lista de transfersSummary a JSON y luego la encriptamos
                string transfersSummaryJson = JsonSerializer.Serialize(transfersSummary);
                string encryptedTransfersSummary = EncryptionHelper.Encrypt(transfersSummaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedTransfersSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/TransfersSummary/GetSummary
        [HttpPost("GetSummary")]
        public async Task<ActionResult<TransfersSummary>> GetTransfersSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Transfers Summary");
                }

                var transfersSummary = await _context.TransfersSummaries.FindAsync(id);

                if (transfersSummary == null)
                {
                    return NotFound("The Transfers Summary with that information wasn't found");
                }

                string transfersSummaryJson = JsonSerializer.Serialize(transfersSummary);
                string encryptedTransfersSummary = EncryptionHelper.Encrypt(transfersSummaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedTransfersSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/TransfersSummary/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateTransfersSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var transfersSummary = JsonSerializer.Deserialize<TransfersSummary>(decryptedData);

                if (transfersSummary == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.TransfersSummaries.Add(transfersSummary);
                await _context.SaveChangesAsync();

                string transfersSummaryJson = JsonSerializer.Serialize(transfersSummary);
                string encryptedTransfersSummary = EncryptionHelper.Encrypt(transfersSummaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedTransfersSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/TransfersSummary/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateTransfersSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var transfersSummary = JsonSerializer.Deserialize<TransfersSummary>(decryptedData);

                if (transfersSummary == null || transfersSummary.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(transfersSummary).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransfersSummaryExists(transfersSummary.Id))
                    {
                        return NotFound("The Transfers Summary with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string transfersSummaryJson = JsonSerializer.Serialize(transfersSummary);
                string encryptedTransfersSummary = EncryptionHelper.Encrypt(transfersSummaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedTransfersSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/TransfersSummary/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteTransfersSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Transfers Summary");
                }

                var transfersSummary = await _context.TransfersSummaries.FindAsync(id);
                if (transfersSummary == null)
                {
                    return NotFound("The Transfers Summary with that information wasn't found");
                }

                _context.TransfersSummaries.Remove(transfersSummary);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool TransfersSummaryExists(int id)
        {
            return _context.TransfersSummaries.Any(e => e.Id == id);
        }
    }
}
