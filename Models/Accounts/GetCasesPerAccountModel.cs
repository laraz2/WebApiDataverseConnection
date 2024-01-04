using System.Collections.Generic;
using WebApiDataverseConnection.Models.Cases;
using WebApiDataverseConnection.Models.Contacts;

namespace WebApiDataverseConnection.Models.Accounts
{
    public class GetCasesPerAccountModel
    {
        public string accountid { get; set; }
        public string name { get; set; }
        public List<GetCasesPerContactModel> casesPerContact { get; set; }

        
    }
}
