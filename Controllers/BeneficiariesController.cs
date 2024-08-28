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
    public class BeneficiariesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BeneficiariesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Beneficiaries/Get
        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetBeneficiaries()
        {
            try
            {
                var beneficiaries = await _context.Beneficiaries.ToListAsync();

                // Convertimos la lista de beneficiaries a JSON y luego la encriptamos
                string beneficiariesJson = JsonSerializer.Serialize(beneficiaries);
                string encryptedBeneficiaries = EncryptionHelper.Encrypt(beneficiariesJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBeneficiaries });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: api/Beneficiaries/GetBeneficiary
        [HttpPost("GetBeneficiary")]
        public async Task<ActionResult<Beneficiary>> GetBeneficiary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Beneficiary");
                }

                var beneficiary = await _context.Beneficiaries
                                                .Include(b => b.Project)
                                                .FirstOrDefaultAsync(b => b.Id == id);

                if (beneficiary == null)
                {
                    return NotFound("The Beneficiary with that information wasn't found");
                }

                string beneficiaryJson = JsonSerializer.Serialize(beneficiary);
                string encryptedBeneficiary = EncryptionHelper.Encrypt(beneficiaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBeneficiary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }

        // POST: api/Beneficiaries/Create
        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateBeneficiary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var beneficiary = JsonSerializer.Deserialize<Beneficiary>(decryptedData);

                if (beneficiary == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Beneficiaries.Add(beneficiary);
                await _context.SaveChangesAsync();

                string beneficiaryJson = JsonSerializer.Serialize(beneficiary);
                string encryptedBeneficiary = EncryptionHelper.Encrypt(beneficiaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBeneficiary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }

        // POST: api/Beneficiaries/Update
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateBeneficiary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var beneficiary = JsonSerializer.Deserialize<Beneficiary>(decryptedData);

                if (beneficiary == null || beneficiary.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(beneficiary).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficiaryExists(beneficiary.Id))
                    {
                        return NotFound("The Beneficiary with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string beneficiaryJson = JsonSerializer.Serialize(beneficiary);
                string encryptedBeneficiary = EncryptionHelper.Encrypt(beneficiaryJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedBeneficiary });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }

        // POST: api/Beneficiaries/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteBeneficiary([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Beneficiary");
                }

                var beneficiary = await _context.Beneficiaries.FindAsync(id);
                if (beneficiary == null)
                {
                    return NotFound("The Beneficiary with that information wasn't found");
                }

                _context.Beneficiaries.Remove(beneficiary);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }

        private bool BeneficiaryExists(int id)
        {
            return _context.Beneficiaries.Any(e => e.Id == id);
        }
    }
}
