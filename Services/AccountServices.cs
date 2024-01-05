using System.Net.Http.Headers;
using WebApiDataverseConnection.Helpers;
using WebApiDataverseConnection.Models.Contacts;
using WebApiDataverseConnection.Models.Cases;
using Newtonsoft.Json;
using WebApiDataverseConnection.Models.Accounts;
using WebApiDataverseConnection.Models.Emails;
using WebApiDataverseConnection.Models.ActivitiesModel;
using WebApiDataverseConnection.Models.Activities;

namespace WebApiDataverseConnection.Services
{
    public class AccountServices : IAccountService
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string authority;
        private readonly string resource;
        private readonly string apiUrl;
        private readonly IConfiguration configuration;
        public AccountServices()
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
        public async Task<List<GetCasesPerAccountModel>> GetAccountCases()
        {
            List<GetCasesPerAccountModel> AccountCaseList = new List<GetCasesPerAccountModel>();
            try
            {
                DataverseAuthentication dataverseAuth = new DataverseAuthentication(clientId, clientSecret, authority, resource);
                String accessToken = await dataverseAuth.GetAccessToken();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"\n");
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Get accounts
                    HttpResponseMessage accountResponse = await httpClient.GetAsync(apiUrl + "accounts");

                    string accountJson;
                    if (accountResponse.IsSuccessStatusCode)
                    {
                        accountJson = await accountResponse.Content.ReadAsStringAsync();
                        // Parse accounts
                        var accounts = JsonConvert.DeserializeObject<dynamic>(accountJson);
                        foreach (var a in accounts.value)
                        {
                            string accountId = a["accountid"].ToString();
                            string accountName = a["name"].ToString();

                            // Get contacts for each account
                            HttpResponseMessage contactResponse = await httpClient.GetAsync(apiUrl + $"contacts?$filter=_parentcustomerid_value eq {accountId}");
                            string contactJson;
                            contactJson = await contactResponse.Content.ReadAsStringAsync();

                            if (contactResponse.IsSuccessStatusCode)
                            {
                                List<GetCasesPerContactModel> contactList = new List<GetCasesPerContactModel>();
                                var contacts = JsonConvert.DeserializeObject<dynamic>(contactJson);
                                foreach (var c in contacts.value)
                                {

                                    string contactid = c["contactid"].ToString();
                                    string fullname = c["fullname"].ToString();
                                    string emailaddress1 = c["emailaddress1"].ToString();
                                    string caseJson;
                                    HttpResponseMessage caseResponse = await httpClient.GetAsync(apiUrl + $"incidents?$filter=_accountid_value eq {accountId} and _primarycontactid_value eq {contactid}");
                                    if (caseResponse.IsSuccessStatusCode)
                                    {
                                        caseJson = await caseResponse.Content.ReadAsStringAsync();
                                        var cases = JsonConvert.DeserializeObject<dynamic>(caseJson);

                                        List<GetActivitiesPerCase> activitiesPercaseList = new List<GetActivitiesPerCase>();
                                        ActivityServices activityServices = new ActivityServices ();

                                        foreach (var cs in cases.value)
                                        {
                                            string str = cs.incidentid;
                                            List<GetActivitiesModel> activitiesList = await activityServices.GetActivitesCases(str);
                                            GetActivitiesPerCase activitiesPerCaseModel= new GetActivitiesPerCase()
                                            {
                                                incidentid = cs["incidentid"].ToString(),
                                                title = cs["title"].ToString(),
                                                ticketnumber = cs["ticketnumber"].ToString(),
                                                statuscode = cs["statuscode"].ToString(),
                                                severitycode = cs["_ownerid_value"].ToString(),
                                                activities = activitiesList
                                            };
                                            activitiesPercaseList.Add(activitiesPerCaseModel);

                                        }
                                        // Create CasePerContact object and add to the list
                                        GetCasesPerContactModel casePerContact = new GetCasesPerContactModel
                                        {
                                            contactid = c["contactid"].ToString(),
                                            fullname = c["fullname"].ToString(),
                                            emailaddress1 = c["emailaddress1"].ToString(),
                                            ActivitiesPerCase = activitiesPercaseList
                                        };

                                        contactList.Add(casePerContact);

                                    }
                                    else
                                    {
                                        caseJson = await caseResponse.Content.ReadAsStringAsync();
                                        var cases = JsonConvert.DeserializeObject<ErrorModel>(caseJson);
                                        Console.WriteLine(cases.error.message);
                                        Console.ReadKey();
                                    }
                                }
                                GetCasesPerAccountModel accountsl = new GetCasesPerAccountModel
                                {
                                    accountid = accountId,
                                    name = accountName,
                                    casesPerContact = contactList
                                };
                                AccountCaseList.Add(accountsl);
                            }
                            else
                            {
                                contactJson = await contactResponse.Content.ReadAsStringAsync();
                                var contact = JsonConvert.DeserializeObject<ErrorModel>(contactJson);
                                Console.WriteLine(contact.error.message);
                                Console.ReadKey();
                            }

                        }
                        Console.WriteLine(AccountCaseList.Count);

                    }
                    else
                    {
                        accountJson = await accountResponse.Content.ReadAsStringAsync();
                        var accounts = JsonConvert.DeserializeObject<ErrorModel>(accountJson);
                        Console.WriteLine(accounts.error.message);
                        Console.ReadKey();
                    }
                    return AccountCaseList;
                }
            }
            catch (HttpRequestException httpEx)
            {
                throw new AppException(httpEx.Message, httpEx.GetHashCode);

            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message, ex.GetHashCode);
            }

            return AccountCaseList;
        }
    }
}
