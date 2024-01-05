namespace WebApiDataverseConnection.Models.Emails
{
    public class GetEmailsModel
    {
        public string Subject { get; set; }
        public string Regarding { get; set; }
        public string Priority { get; set; }
        public string ActualEnd { get; set; }
        public string Description { get; set; }
        public string Sender { get; set; }
    }
}
