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
    public class TransfersFromUSController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransfersFromUSController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TransfersFromUS/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetTransfersFromUS()
        {
            try
            {
                var transfers = await _context.TransfersFromUs.ToListAsync();

                // Convertimos la lista de transfers a JSON y luego la encriptamos
                string transfersJson = JsonSerializer.Serialize(transfers);
                string encryptedTransfers = EncryptionHelper.Encrypt(transfersJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedTransfers });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/TransfersFromUS/GetTransfer
        [HttpPost("GetTransfer")]
        public async Task<ActionResult<TransfersFromU>> GetTransfer([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Transfer");
                }

                var transfer = await _context.TransfersFromUs.FindAsync(id);

                if (transfer == null)
                {
                    return NotFound("The Transfer with that information wasn't found");
                }

                string transferJson = JsonSerializer.Serialize(transfer);
                string encryptedTransfer = EncryptionHelper.Encrypt(transferJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedTransfer });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/TransfersFromUS/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateTransfer([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var transfer = JsonSerializer.Deserialize<TransfersFromU>(decryptedData);

                if (transfer == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.TransfersFromUs.Add(transfer);
                await _context.SaveChangesAsync();

                string transferJson = JsonSerializer.Serialize(transfer);
                string encryptedTransfer = EncryptionHelper.Encrypt(transferJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedTransfer });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/TransfersFromUS/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateTransfer([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var transfer = JsonSerializer.Deserialize<TransfersFromU>(decryptedData);

                if (transfer == null || transfer.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(transfer).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferExists(transfer.Id))
                    {
                        return NotFound("The Transfer with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string transferJson = JsonSerializer.Serialize(transfer);
                string encryptedTransfer = EncryptionHelper.Encrypt(transferJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedTransfer });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/TransfersFromUS/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteTransfer([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Transfer");
                }

                var transfer = await _context.TransfersFromUs.FindAsync(id);
                if (transfer == null)
                {
                    return NotFound("The Transfer with that information wasn't found");
                }

                _context.TransfersFromUs.Remove(transfer);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool TransferExists(int id)
        {
            return _context.TransfersFromUs.Any(e => e.Id == id);
        }
    }
}
