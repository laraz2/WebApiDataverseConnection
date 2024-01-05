using System.Net.Http.Headers;
using WebApiDataverseConnection.Helpers;
using WebApiDataverseConnection.Models.Contacts;
using WebApiDataverseConnection.Models.Cases;
using Newtonsoft.Json;
using WebApiDataverseConnection.Models.Accounts;
using static System.Net.WebRequestMethods;
using WebApiDataverseConnection.Models.Emails;

namespace WebApiDataverseConnection.Services
{
    public class EmailsServices: IEmailServices
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string authority;
        private readonly string resource;
        private readonly string apiUrl;
        private readonly IConfiguration configuration;
        public EmailsServices()
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
        public async void GetEmailCases()
        {
           // List<GetEmailsPerCaseModel> EmailsList = new List<GetEmailsPerCaseModel>();
            try
            {
                DataverseAuthentication dataverseAuth = new DataverseAuthentication(clientId, clientSecret, authority, resource);
                String accessToken = await dataverseAuth.GetAccessToken();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"\n");
                Console.ReadKey();
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Get accounts
                    HttpResponseMessage emailResponse = await httpClient.GetAsync(apiUrl + "email");

                    string emailJson;
                    if (emailResponse.IsSuccessStatusCode)
                    {
                        emailJson = await emailResponse.Content.ReadAsStringAsync();
                        // Parse accounts
                        var emails = JsonConvert.DeserializeObject<dynamic>(emailJson);
                        Console.WriteLine(emails.ToString());
                    }
                  
                }
        
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message );
            }
        }
    }
}
                            ////foreach (var a in accounts.value)
                            ////{
                            ////    string accountId = a["accountid"].ToString();
                            ////    string accountName = a["name"].ToString();

////    // Get contacts for each account
////    HttpResponseMessage contactResponse = await httpClient.GetAsync(apiUrl + $"email");
//    string contactJson;
//    contactJson = await contactResponse.Content.ReadAsStringAsync();

//    if (contactResponse.IsSuccessStatusCode)
//    {
//        List<GetCasesPerContactModel> contactList = new List<GetCasesPerContactModel>();
//        var contacts = JsonConvert.DeserializeObject<dynamic>(contactJson);


