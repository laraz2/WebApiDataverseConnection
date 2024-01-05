using WebApiDataverseConnection.Services;
using WebApiDataverseConnection.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        [HttpGet("GetEmailCases")]
        public  async void GetEmailCases()
        {
            try
            {
                //Function that gets user by ID
              _emailServices.GetEmailCases();

                
            }
            catch (AppException e)
            {
               //BadRequest(new { message = e.Message, error = e.InnerException });
            }
        }
    }
}
