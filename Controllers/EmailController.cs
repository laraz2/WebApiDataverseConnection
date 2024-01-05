using WebApiDataverseConnection.Services;
using WebApiDataverseConnection.Helpers;
using Microsoft.AspNetCore.Mvc;


namespace WebApiDataverseConnection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailServices _emailServices;

        public EmailController(IEmailServices emailServices)
        {
            _emailServices = emailServices;
        }

        [HttpGet("GetGetEmailCases")]
        public async Task<IActionResult> GetEmailCases()
        {
            try
            {
                //Function that gets user by ID
                var result = await _emailServices.GetEmailCases();

                return Ok(result);
            }
            catch (AppException e)
            {
                return BadRequest(new { message = e.Message, error = e.InnerException });
            }
        }
    }
}
