using WebApiDataverseConnection.Models.Accounts;
using WebApiDataverseConnection.Models.Emails;

namespace WebApiDataverseConnection.Services
{
    public interface IEmailServices
    {
        Task<List<GetEmailsModel>> GetEmailCases(string incidentid);

    }
}
