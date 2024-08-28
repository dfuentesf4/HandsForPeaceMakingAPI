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
    public class RevenuesDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RevenuesDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RevenuesDetails/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetRevenuesDetails()
        {
            try
            {
                var revenuesDetails = await _context.RevenuesDetails.ToListAsync();

                // Convertimos la lista de revenuesDetails a JSON y luego la encriptamos
                string revenuesDetailsJson = JsonSerializer.Serialize(revenuesDetails);
                string encryptedRevenuesDetails = EncryptionHelper.Encrypt(revenuesDetailsJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedRevenuesDetails });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/RevenuesDetails/GetRevenuesDetail
        [HttpPost("GetRevenuesDetail")]
        public async Task<ActionResult<RevenuesDetail>> GetRevenuesDetail([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the RevenuesDetail");
                }

                var revenuesDetail = await _context.RevenuesDetails.FindAsync(id);

                if (revenuesDetail == null)
                {
                    return NotFound("The RevenuesDetail with that information wasn't found");
                }

                string revenuesDetailJson = JsonSerializer.Serialize(revenuesDetail);
                string encryptedRevenuesDetail = EncryptionHelper.Encrypt(revenuesDetailJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedRevenuesDetail });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/RevenuesDetails/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateRevenuesDetail([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var revenuesDetail = JsonSerializer.Deserialize<RevenuesDetail>(decryptedData);

                if (revenuesDetail == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.RevenuesDetails.Add(revenuesDetail);
                await _context.SaveChangesAsync();

                string revenuesDetailJson = JsonSerializer.Serialize(revenuesDetail);
                string encryptedRevenuesDetail = EncryptionHelper.Encrypt(revenuesDetailJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedRevenuesDetail });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/RevenuesDetails/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateRevenuesDetail([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var revenuesDetail = JsonSerializer.Deserialize<RevenuesDetail>(decryptedData);

                if (revenuesDetail == null || revenuesDetail.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(revenuesDetail).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RevenuesDetailExists(revenuesDetail.Id))
                    {
                        return NotFound("The RevenuesDetail with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string revenuesDetailJson = JsonSerializer.Serialize(revenuesDetail);
                string encryptedRevenuesDetail = EncryptionHelper.Encrypt(revenuesDetailJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedRevenuesDetail });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/RevenuesDetails/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteRevenuesDetail([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the RevenuesDetail");
                }

                var revenuesDetail = await _context.RevenuesDetails.FindAsync(id);
                if (revenuesDetail == null)
                {
                    return NotFound("The RevenuesDetail with that information wasn't found");
                }

                _context.RevenuesDetails.Remove(revenuesDetail);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool RevenuesDetailExists(int id)
        {
            return _context.RevenuesDetails.Any(e => e.Id == id);
        }
    }
}
