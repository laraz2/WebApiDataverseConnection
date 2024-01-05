using WebApiDataverseConnection.Helpers;
using WebApiDataverseConnection.Models.Contacts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace WebApiDataverseConnection.Services
{
    public class ContactServices
    {
        private readonly string clientId = "";
        private readonly string clientSecret="";
        private readonly string authority = "";
        private readonly string resource = "";
        private readonly string apiUrl = "";
        private readonly IConfiguration configuration;
        public ContactServices()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appSettings.json")
                .Build();
            this.clientId = configuration["ClientId"];
            this.clientSecret = configuration["ClientSecret"];
            this.authority = configuration["Authority"];
            this.resource = configuration["Resource"];
            this.apiUrl = configuration["ApiUrl"];
        }
        public async Task PostContacts()
        {
            try
            {
                DataverseAuthentication dataverseAuth = new DataverseAuthentication(clientId, clientSecret, authority, resource);
                String accessToken = await dataverseAuth.GetAccessToken();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.ReadKey();
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Example: Retrieve records from a Dataverse entity
                    AddContactsModel model = new AddContactsModel
                    {
                        firstname = "Bob",
                        lastname = "Bob",
                        fullname = "Bob Bob",
                        emailaddress1 = "Bob@gmail.com",
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage postresponse = await httpClient.PostAsync(apiUrl + "contacts", content);

                    if (!postresponse.IsSuccessStatusCode)
                    {
                        var json = await postresponse.Content.ReadAsStringAsync();

                        try
                        {
                            var errorObject = JObject.Parse(json);
                            var err = errorObject.ToObject<ErrorModel>();

                            Console.WriteLine(err.error.message);
                            Console.ReadKey();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);

                        }

                    }
                    else
                    {
                        Console.WriteLine("Success!");
                    }

                }
            }

            catch (Exception ex)
            {
                throw new AppException("The error is" + ex.Message.ToString(),ex.GetHashCode);

            }
        }

        public async Task GetContactServices()
        {
            try
            {
                DataverseAuthentication dataverseAuth = new DataverseAuthentication(clientId, clientSecret, authority, resource);
                String accessToken = await dataverseAuth.GetAccessToken();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"\n");
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage getresponse = await httpClient.GetAsync(apiUrl + "contacts");
                    var json = await getresponse.Content.ReadAsStringAsync();
                    if (!getresponse.IsSuccessStatusCode)
                    {
                        try
                        {
                            var errorObject = JObject.Parse(json);
                            var err = errorObject.ToObject<ErrorModel>();

                            Console.WriteLine(err.error.message);
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            throw new AppException(ex.Message.ToString(), ex.GetHashCode);

                        }
                    }
                    else
                    {
                        // Parse the JSON string into a JObject
                        var jsonObject = JObject.Parse(json);

                        // Access the "value" array
                        var valueArray = jsonObject["value"];

                        if (valueArray != null)
                        {
                            // Count the number of records inside the "value" array
                            int numberOfRecords = valueArray.Count();
                            Console.WriteLine($"Number of records: {numberOfRecords}");
                            Console.WriteLine(valueArray.ToString());
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("No 'value' array found in the JSON.");
                            Console.ReadKey();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AppException("The error is " + ex.Message.ToString(), ex.GetHashCode);
            }
        }
    }
}