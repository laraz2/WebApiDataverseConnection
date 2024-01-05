using WebApiDataverseConnection.Models.ActivitiesModel;

namespace WebApiDataverseConnection.Models.Activities
{
    public class GetActivitiesPerCase
    {
        public string incidentid { get; set; }
        public string title { get; set; }
        public string ticketnumber { get; set; }
        public string statuscode { get; set; }
        public string severitycode { get; set; }
        public List<GetActivitiesModel> activities { get; set; }

    }
}
