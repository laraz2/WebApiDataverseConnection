using WebApiDataverseConnection.Models.Accounts;

namespace WebApiDataverseConnection.Services
{
    public interface IAccountService
    {
        Task<List<GetAccountsModel>> GetAccountCases();
    }
}
