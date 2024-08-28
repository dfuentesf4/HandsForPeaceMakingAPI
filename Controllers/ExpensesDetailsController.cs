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
    public class ExpensesDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpensesDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ExpensesDetails/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetExpensesDetails()
        {
            try
            {
                var expensesDetails = await _context.ExpensesDetails.ToListAsync();

                // Convertimos la lista de expensesDetails a JSON y luego la encriptamos
                string expensesDetailsJson = JsonSerializer.Serialize(expensesDetails);
                string encryptedExpensesDetails = EncryptionHelper.Encrypt(expensesDetailsJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedExpensesDetails });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/ExpensesDetails/GetExpensesDetail
        [HttpPost("GetExpensesDetail")]
        public async Task<ActionResult<ExpensesDetail>> GetExpensesDetail([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the ExpensesDetail");
                }

                var expensesDetail = await _context.ExpensesDetails.FindAsync(id);

                if (expensesDetail == null)
                {
                    return NotFound("The ExpensesDetail with that information wasn't found");
                }

                string expensesDetailJson = JsonSerializer.Serialize(expensesDetail);
                string encryptedExpensesDetail = EncryptionHelper.Encrypt(expensesDetailJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedExpensesDetail });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/ExpensesDetails/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateExpensesDetail([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var expensesDetail = JsonSerializer.Deserialize<ExpensesDetail>(decryptedData);

                if (expensesDetail == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.ExpensesDetails.Add(expensesDetail);
                await _context.SaveChangesAsync();

                string expensesDetailJson = JsonSerializer.Serialize(expensesDetail);
                string encryptedExpensesDetail = EncryptionHelper.Encrypt(expensesDetailJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedExpensesDetail });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/ExpensesDetails/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateExpensesDetail([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var expensesDetail = JsonSerializer.Deserialize<ExpensesDetail>(decryptedData);

                if (expensesDetail == null || expensesDetail.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(expensesDetail).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpensesDetailExists(expensesDetail.Id))
                    {
                        return NotFound("The ExpensesDetail with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string expensesDetailJson = JsonSerializer.Serialize(expensesDetail);
                string encryptedExpensesDetail = EncryptionHelper.Encrypt(expensesDetailJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedExpensesDetail });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/ExpensesDetails/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteExpensesDetail([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the ExpensesDetail");
                }

                var expensesDetail = await _context.ExpensesDetails.FindAsync(id);
                if (expensesDetail == null)
                {
                    return NotFound("The ExpensesDetail with that information wasn't found");
                }

                _context.ExpensesDetails.Remove(expensesDetail);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool ExpensesDetailExists(int id)
        {
            return _context.ExpensesDetails.Any(e => e.Id == id);
        }
    }
}
