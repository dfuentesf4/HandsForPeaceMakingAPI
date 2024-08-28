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
    public class BankBookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BankBookController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BankBook/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetBankBooks()
        {
            try
            {
                var bankBooks = await _context.BankBooks
                    .Include(bb => bb.Bank) // Incluir la entidad relacionada Bank
                    .ToListAsync();

                // Convertimos la lista de bankBooks a JSON y luego la encriptamos
                string bankBooksJson = JsonSerializer.Serialize(bankBooks);
                string encryptedBankBooks = EncryptionHelper.Encrypt(bankBooksJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBankBooks });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/BankBook/GetBankBook
        [HttpPost("GetBankBook")]
        public async Task<ActionResult<BankBook>> GetBankBook([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Bank_Book");
                }

                var bankBook = await _context.BankBooks
                    .Include(bb => bb.Bank) // Incluir la entidad relacionada Bank
                    .FirstOrDefaultAsync(bb => bb.Id == id);

                if (bankBook == null)
                {
                    return NotFound("The Bank_Book with that information wasn't found");
                }

                string bankBookJson = JsonSerializer.Serialize(bankBook);
                string encryptedBankBook = EncryptionHelper.Encrypt(bankBookJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBankBook });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/BankBook/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateBankBook([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var bankBook = JsonSerializer.Deserialize<BankBook>(decryptedData);

                if (bankBook == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.BankBooks.Add(bankBook);
                await _context.SaveChangesAsync();

                string bankBookJson = JsonSerializer.Serialize(bankBook);
                string encryptedBankBook = EncryptionHelper.Encrypt(bankBookJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBankBook });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/BankBook/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateBankBook([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var bankBook = JsonSerializer.Deserialize<BankBook>(decryptedData);

                if (bankBook == null || bankBook.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(bankBook).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankBookExists(bankBook.Id))
                    {
                        return NotFound("The Bank_Book with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string bankBookJson = JsonSerializer.Serialize(bankBook);
                string encryptedBankBook = EncryptionHelper.Encrypt(bankBookJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBankBook });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/BankBook/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteBankBook([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Bank_Book");
                }

                var bankBook = await _context.BankBooks.FindAsync(id);
                if (bankBook == null)
                {
                    return NotFound("The Bank_Book with that information wasn't found");
                }

                _context.BankBooks.Remove(bankBook);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool BankBookExists(int id)
        {
            return _context.BankBooks.Any(e => e.Id == id);
        }
    }
}
