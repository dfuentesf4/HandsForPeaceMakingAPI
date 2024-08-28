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
    public class BanksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BanksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Banks/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetBanks()
        {
            try
            {
                var banks = await _context.Banks.ToListAsync();

                // Convertimos la lista de banks a JSON y luego la encriptamos
                string banksJson = JsonSerializer.Serialize(banks);
                string encryptedBanks = EncryptionHelper.Encrypt(banksJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBanks });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/Banks/GetBank
        [HttpPost("GetBank")]
        public async Task<ActionResult<Bank>> GetBank([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Bank");
                }

                var bank = await _context.Banks.FindAsync(id);

                if (bank == null)
                {
                    return NotFound("The Bank with that information wasn't found");
                }

                string bankJson = JsonSerializer.Serialize(bank);
                string encryptedBank = EncryptionHelper.Encrypt(bankJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBank });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/Banks/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateBank([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var bank = JsonSerializer.Deserialize<Bank>(decryptedData);

                if (bank == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Banks.Add(bank);
                await _context.SaveChangesAsync();

                string bankJson = JsonSerializer.Serialize(bank);
                string encryptedBank = EncryptionHelper.Encrypt(bankJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBank });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/Banks/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateBank([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var bank = JsonSerializer.Deserialize<Bank>(decryptedData);

                if (bank == null || bank.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(bank).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankExists(bank.Id))
                    {
                        return NotFound("The Bank with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string bankJson = JsonSerializer.Serialize(bank);
                string encryptedBank = EncryptionHelper.Encrypt(bankJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBank });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/Banks/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteBank([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Bank");
                }

                var bank = await _context.Banks.FindAsync(id);
                if (bank == null)
                {
                    return NotFound("The Bank with that information wasn't found");
                }

                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool BankExists(int id)
        {
            return _context.Banks.Any(e => e.Id == id);
        }
    }
}
