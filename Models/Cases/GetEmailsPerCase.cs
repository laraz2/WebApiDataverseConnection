using WebApiDataverseConnection.Models.Emails;

namespace WebApiDataverseConnection.Models.Cases
{
    public class GetEmailsPerCase
    {
        public string incidentid { get; set; }
        public string title { get; set; }
        public string ticketnumber { get; set; }
        public string statuscode { get; set; }
        public string severitycode { get; set; }
        public  List<GetEmailsModel> Emails { get; set; }
    }
}
