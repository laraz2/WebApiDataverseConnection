using WebApiDataverseConnection.Models.Accounts;

namespace WebApiDataverseConnection.Services
{
    public interface IAccountService
    {
        Task<List<GetCasesPerAccountModel>> GetAccountCases();
    }
}
