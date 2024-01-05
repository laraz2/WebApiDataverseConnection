using WebApiDataverseConnection.Models.Activities;
using WebApiDataverseConnection.Models.Cases;


namespace WebApiDataverseConnection.Models.Contacts
{
    public class GetCasesPerContactModel
    {
        public string contactid { get; set; }
        public string fullname { get; set; }
        public string emailaddress1 { get; set; }
        public List<GetActivitiesPerCase> ActivitiesPerCase { get; set; }
    }
}
