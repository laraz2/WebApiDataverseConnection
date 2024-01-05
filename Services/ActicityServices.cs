using System.Net.Http.Headers;
using WebApiDataverseConnection.Helpers;
using Newtonsoft.Json;
using WebApiDataverseConnection.Models.Emails;
using WebApiDataverseConnection.Models.ActivitiesModel;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.Identity.Client;
using WebApiDataverseConnection.Models.ActivitiesModel;
namespace WebApiDataverseConnection.Services
{
    public class ActivityServices 
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string authority;
        private readonly string resource;
        private readonly string apiUrl;
        private readonly IConfiguration configuration;
        public ActivityServices()
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
        public async Task<List<GetActivitiesModel>> GetActivitesCases(string incidentid)
        {
            List<GetActivitiesModel> activityList = new List<GetActivitiesModel>();
            try
            {
                DataverseAuthentication dataverseAuth = new DataverseAuthentication(clientId, clientSecret, authority, resource);
                String accessToken = await dataverseAuth.GetAccessToken();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"\n");
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    // Get emails
                    HttpResponseMessage activityResponse = await httpClient.GetAsync(apiUrl + $"activitypointers?$filter=_regardingobjectid_value eq {incidentid}");
                    string activityJson;
                    if (activityResponse.IsSuccessStatusCode)
                    {
                        activityJson = await activityResponse.Content.ReadAsStringAsync();
                        // Parse emails
                        var emails = JsonConvert.DeserializeObject<dynamic>(activityJson);
                        foreach (var e in emails.value)
                        {
                            if (e != null)
                            {
                                GetActivitiesModel activities = new GetActivitiesModel()
                                {
                                    activityid = e["activityid"]?.ToString(),
                                    statecode = e["statecode"]?.ToString(),
                                    description = ConvertHtmlToPlainText(e["description"]?.ToString()),
                                    subject = e["subject"]?.ToString(),
                                    activitytypecode = e["activitytypecode"],
                                    actualend = e["actualend"]?.ToString(),
                                    _sendermailboxid_value = e["_sendermailboxid_value"]?.ToString(),
                                    
                                };

                                activityList.Add(activities);
                            }
                        }

                    }
                    else
                    {
                        activityJson = await activityResponse.Content.ReadAsStringAsync();
                        var cases = JsonConvert.DeserializeObject<ErrorModel>(activityJson);
                        Console.WriteLine(cases.error.message);
                        Console.ReadKey();
                    }
                    return activityList;
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
            return activityList;
        }
        public string ConvertHtmlToPlainText(string html)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                return doc.DocumentNode.InnerText;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}







