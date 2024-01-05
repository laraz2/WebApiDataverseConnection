using WebApiDataverseConnection.Services;
using WebApiDataverseConnection.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace WebApiDataverseConnection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("GetAccountsCases")]
        public async Task<IActionResult> GetAccountsCases()
        {
            try
            {
                //Function that gets user by ID
                var result = await _accountService.GetAccountCases();

                return Ok(result);
            }
            catch (AppException e)
            {
                return BadRequest(new { message = e.Message, error = e.InnerException });
            }
        }
    }
}
