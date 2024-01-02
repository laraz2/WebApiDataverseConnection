using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiDataverseConnection.Models.Cases
{
    public class GetCasesModel
    {
        public string incidentid { get; set; }
        public string title { get; set; }
        public string ticketnumber { get; set; }
        public string statuscode { get; set; }
        public string severitycode { get; set; }
    }
}
