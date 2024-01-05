using System.Net.Http.Headers;
using WebApiDataverseConnection.Helpers;
using Newtonsoft.Json;
using WebApiDataverseConnection.Models.Activities;
using HtmlAgilityPack;

namespace WebApiDataverseConnection.Services
{
    public class ActivityServices
    {
        private readonly string clientId = "";
        private readonly string clientSecret = "";
        private readonly string authority = "";
        private readonly string resource = "";
        private readonly string apiUrl = "";
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
            List<GetActivitiesModel> activityList = new ();
            try
            {
                DataverseAuthentication dataverseAuth = new (clientId, clientSecret, authority, resource);
                String accessToken = await dataverseAuth.GetAccessToken();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"\n");
                using (HttpClient httpClient = new ())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    // Get activities
                    HttpResponseMessage activityResponse = await httpClient.GetAsync(apiUrl + $"activitypointers?$filter=_regardingobjectid_value eq {incidentid}");
                    string activityJson;
                    if (activityResponse.IsSuccessStatusCode)
                    {
                        activityJson = await activityResponse.Content.ReadAsStringAsync();
                        // Parse activities
                        var activitiesList = JsonConvert.DeserializeObject<dynamic>(activityJson);
                        foreach (var e in activitiesList.value)
                        {
                            if (e != null)
                            {
                                HttpResponseMessage userNameResponse = await httpClient.GetAsync(apiUrl + $"systemusers?$filter=systemuserid eq {e._createdby_value}");
                                string userJson;
                                if (userNameResponse.IsSuccessStatusCode)
                                {
                                    userJson = await userNameResponse.Content.ReadAsStringAsync();
                                    // Parse USERS
                                    var usersList = JsonConvert.DeserializeObject<dynamic>(userJson);
                                    string username = usersList["value"][0].domainname;
                                    GetActivitiesModel activities = new ()
                                    {
                                        activityid = e["activityid"]?.ToString(),
                                        statecode = e["statecode"]?.ToString(),
                                        description = ConvertHtmlToPlainText(e["description"]?.ToString()),
                                        subject = e["subject"]?.ToString(),
                                        activitytypecode = e["activitytypecode"],
                                        actualend = e["actualend"]?.ToString(),
                                        username = username,
                                    };

                                    activityList.Add(activities);
                                }
                                else
                                {
                                    userJson = await userNameResponse.Content.ReadAsStringAsync();
                                    var user = JsonConvert.DeserializeObject<ErrorModel>(userJson);
                                    Console.WriteLine(user.error.message);
                                    Console.ReadKey();
                                }
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
        }
        public string ConvertHtmlToPlainText(string html)
        {
            if (html != null)
            {
                try
                {
                    HtmlDocument doc = new ();
                    doc.LoadHtml(html);

                    return doc.DocumentNode.InnerText;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "";
                }
            }
            return "";

        }
    }
}







