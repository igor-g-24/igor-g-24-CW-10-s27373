using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            try
            {
                var success = await _clientService.DeleteClientAsync(idClient);
                if (success)
                {
                    return NoContent();
                }
                return NotFound(new { message = "Client not found or deletion failed." }); 
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred during client deletion.");
            }
        }
    }
}