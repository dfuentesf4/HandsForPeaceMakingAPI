using HandsForPeaceMakingAPI.Data;
using HandsForPeaceMakingAPI.Models;
using HandsForPeaceMakingAPI.Services.EncryptionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HandsForPeaceMakingAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DonorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DonorsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("Get")]
        public async Task<ActionResult<string>> GetDonors()
        {
            try
            {
                var donors = await _context.Donors.ToListAsync();

                // Convertimos la lista de donors a JSON y luego la encriptamos
                string donorsJson = System.Text.Json.JsonSerializer.Serialize(donors);
                string encryptedDonors = EncryptionHelper.Encrypt(donorsJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedDonors });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }


        
        [HttpPost("GetDonor")]
        public async Task<ActionResult<Donor>> GetDonor([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to find the Donor");
                }

                var donor = await _context.Donors.FindAsync(id);

                if (donor == null)
                {
                    return NotFound("The Donor with that information was't found");
                }

                string donorJson = JsonSerializer.Serialize(donor);
                string encryptedDonor = EncryptionHelper.Encrypt(donorJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedDonor });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting data: " + ex.Message);
            }
        }



        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateDonor([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var donor = JsonSerializer.Deserialize<Donor>(decryptedData);

                if (donor == null)
                {
                    return BadRequest("Invalid data");
                }

                _context.Donors.Add(donor);
                await _context.SaveChangesAsync();

                string donorJson = JsonSerializer.Serialize(donor);
                string encryptedDonor = EncryptionHelper.Encrypt(donorJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedDonor });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or saving data: " + ex.Message);
            }
        }


        [HttpPost("Update")]
        public async Task<IActionResult> UpdateDonor([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedData = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                var donor = JsonSerializer.Deserialize<Donor>(decryptedData);

                if (donor == null || donor.Id == 0)
                {
                    return BadRequest("Invalid data");
                }

                _context.Entry(donor).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonorExists(donor.Id))
                    {
                        return NotFound("The Donor with that information wasn't found");
                    }
                    else
                    {
                        throw;
                    }
                }

                string donorJson = JsonSerializer.Serialize(donor);
                string encryptedDonor = EncryptionHelper.Encrypt(donorJson);

                return Ok(new EncryptedResponse { EncryptedData = encryptedDonor });
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or updating data: " + ex.Message);
            }
        }



        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteDonor([FromBody] EncryptedRequest encryptedRequest)
        {
            try
            {
                string decryptedId = EncryptionHelper.Decrypt(encryptedRequest.EncryptedData);
                if (!int.TryParse(decryptedId, out int id))
                {
                    return BadRequest("Invalid data to delete the Donor");
                }

                var donor = await _context.Donors.FindAsync(id);
                if (donor == null)
                {
                    return NotFound("The Donor with that information wasn't found");
                }

                _context.Donors.Remove(donor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error decrypting or deleting data: " + ex.Message);
            }
        }





        private bool DonorExists(int id)
        {
            return _context.Donors.Any(e => e.Id == id);
        }
    }
}
