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
    public class FolderBankController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FolderBankController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FolderBank/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetFolderBanks()
        {
            try
            {
                var folderBanks = await _context.FolderBanks
                    .Include(fb => fb.Bank)
                    .Select(v => new
                    {
                        v.Id,
                        v.Year,
                        v.Month,
                        v.DollarExchange,
                        v.Amount,
                        v.BankId,
                        v.TransactionId,
                        v.Folder,
                        v.IsActive,
                        Bank = v.Bank == null ? null : new
                        {
                            v.Bank.Id,
                            v.Bank.Name
                        }
                    })
                    .ToListAsync();

                // Convertimos la lista de folderBanks a JSON y luego la encriptamos
                string folderBanksJson = JsonSerializer.Serialize(folderBanks);
                string encryptedFolderBanks = EncryptionHelper.Encrypt(folderBanksJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedFolderBanks });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/FolderBank/GetFolderBank
        [HttpPost("GetFolderBank")]
        public async Task<ActionResult<FolderBank>> GetFolderBank([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Folder_Bank");
                }

                var folderBank = await _context.FolderBanks
                    .Include(fb => fb.Bank) // Incluir la entidad relacionada Bank
                    .FirstOrDefaultAsync(fb => fb.Id == id);

                if (folderBank == null)
                {
                    return NotFound("The Folder_Bank with that information wasn't found");
                }

                string folderBankJson = JsonSerializer.Serialize(folderBank);
                string encryptedFolderBank = EncryptionHelper.Encrypt(folderBankJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedFolderBank });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/FolderBank/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateFolderBank([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var folderBank = JsonSerializer.Deserialize<FolderBank>(decryptedData);

                if (folderBank == null)
                {
                    return BadRequest("Invalid data");
                }
                folderBank.Bank = null;

                _context.FolderBanks.Add(folderBank);
                await _context.SaveChangesAsync();

                string folderBankJson = JsonSerializer.Serialize(folderBank);
                string encryptedFolderBank = EncryptionHelper.Encrypt(folderBankJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedFolderBank });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/FolderBank/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateFolderBank([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var folderBank = JsonSerializer.Deserialize<FolderBank>(decryptedData);

                if (folderBank == null || folderBank.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(folderBank).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FolderBankExists(folderBank.Id))
                    {
                        return NotFound("The Folder_Bank with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string folderBankJson = JsonSerializer.Serialize(folderBank);
                string encryptedFolderBank = EncryptionHelper.Encrypt(folderBankJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedFolderBank });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/FolderBank/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteFolderBank([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Folder_Bank");
                }

                var folderBank = await _context.FolderBanks.FindAsync(id);
                if (folderBank == null)
                {
                    return NotFound("The Folder_Bank with that information wasn't found");
                }

                _context.FolderBanks.Remove(folderBank);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool FolderBankExists(int id)
        {
            return _context.FolderBanks.Any(e => e.Id == id);
        }
    }
}
