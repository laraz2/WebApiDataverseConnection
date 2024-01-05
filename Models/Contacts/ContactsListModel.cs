
namespace WebApiDataverseConnection.Models.Contacts
{
    public class CustomersList
    {
        public class M365Customers
        {
            public string ODataContext { get; set; } = "";
            public List<GetContactsModel> Value { get; set; } = [];
        }

    }
}
