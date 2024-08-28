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
    public class BankSummaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BankSummaryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BankSummary/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetBankSummaries()
        {
            try
            {
                var bankSummaries = await _context.BankSummaries
                    .Include(bs => bs.Bank) // Incluir la entidad relacionada Bank
                    .ToListAsync();

                // Convertimos la lista de bankSummaries a JSON y luego la encriptamos
                string bankSummariesJson = JsonSerializer.Serialize(bankSummaries);
                string encryptedBankSummaries = EncryptionHelper.Encrypt(bankSummariesJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBankSummaries });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/BankSummary/GetBankSummary
        [HttpPost("GetBankSummary")]
        public async Task<ActionResult<BankSummary>> GetBankSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Bank_Summary");
                }

                var bankSummary = await _context.BankSummaries
                    .Include(bs => bs.Bank) // Incluir la entidad relacionada Bank
                    .FirstOrDefaultAsync(bs => bs.Id == id);

                if (bankSummary == null)
                {
                    return NotFound("The Bank_Summary with that information wasn't found");
                }

                string bankSummaryJson = JsonSerializer.Serialize(bankSummary);
                string encryptedBankSummary = EncryptionHelper.Encrypt(bankSummaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBankSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/BankSummary/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateBankSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var bankSummary = JsonSerializer.Deserialize<BankSummary>(decryptedData);

                if (bankSummary == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.BankSummaries.Add(bankSummary);
                await _context.SaveChangesAsync();

                string bankSummaryJson = JsonSerializer.Serialize(bankSummary);
                string encryptedBankSummary = EncryptionHelper.Encrypt(bankSummaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBankSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/BankSummary/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateBankSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var bankSummary = JsonSerializer.Deserialize<BankSummary>(decryptedData);

                if (bankSummary == null || bankSummary.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(bankSummary).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankSummaryExists(bankSummary.Id))
                    {
                        return NotFound("The Bank_Summary with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string bankSummaryJson = JsonSerializer.Serialize(bankSummary);
                string encryptedBankSummary = EncryptionHelper.Encrypt(bankSummaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBankSummary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/BankSummary/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteBankSummary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Bank_Summary");
                }

                var bankSummary = await _context.BankSummaries.FindAsync(id);
                if (bankSummary == null)
                {
                    return NotFound("The Bank_Summary with that information wasn't found");
                }

                _context.BankSummaries.Remove(bankSummary);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool BankSummaryExists(int id)
        {
            return _context.BankSummaries.Any(e => e.Id == id);
        }
    }
}
