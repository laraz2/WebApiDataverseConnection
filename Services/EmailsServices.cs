﻿using System.Net.Http.Headers;
using WebApiDataverseConnection.Helpers;
using Newtonsoft.Json;
using WebApiDataverseConnection.Models.Emails;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.Identity.Client;
namespace WebApiDataverseConnection.Services
{
    public class EmailsServices : IEmailServices
    {
        private readonly string clientId = "";
        private readonly string clientSecret = "";
        private readonly string authority = "";
        private readonly string resource = "";
        private readonly string apiUrl = "";
        private readonly IConfiguration configuration;
        public EmailsServices()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appSettings.json")
                .Build();
            this.clientId = configuration["ClientId"]!;
            this.clientSecret = configuration["ClientSecret"]!;
            this.authority = configuration["Authority"]!;
            this.resource = configuration["Resource"]!;
            this.apiUrl = configuration["ApiUrl"]!;
        }
        public async Task<List<GetEmailsModel>> GetEmailCases(string incidentid)
        {
            List<GetEmailsModel> EmailsList = new ();
            try
            {
                DataverseAuthentication dataverseAuth = new(clientId, clientSecret, authority, resource);
                String accessToken = await dataverseAuth.GetAccessToken();

                Console.WriteLine($"Access Token: {accessToken}");
                Console.WriteLine($"\n");
                using (HttpClient httpClient = new ())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    // Get emails
                    HttpResponseMessage emailResponse = await httpClient.GetAsync(apiUrl + $"emails?$filter=_regardingobjectid_value eq {incidentid}");
                    string emailJson;
                    if (emailResponse.IsSuccessStatusCode)
                    {
                        emailJson = await emailResponse.Content.ReadAsStringAsync();
                        // Parse emails
                        var emails = JsonConvert.DeserializeObject<dynamic>(emailJson);
                        foreach (var e in emails.value)
                        {
                            if (e != null)
                            {
                                GetEmailsModel email = new ()
                                {
                                    Subject = e["subject"]?.ToString()!,
                                    Regarding = e["regarding"]?.ToString()!,
                                    Priority = e["priority"]?.ToString()!,
                                    Description = ConvertHtmlToPlainText(e["description"]?.ToString())!,
                                    Sender = e["systemsender"]?.ToString()!
                                };

                                EmailsList.Add(email);
                            }
                        }

                    }
                    else
                    {
                        emailJson = await emailResponse.Content.ReadAsStringAsync();
                        var cases = JsonConvert.DeserializeObject<ErrorModel>(emailJson);
                        Console.WriteLine(cases.error.message);
                       
                    }
                    return EmailsList;
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
                    HtmlDocument doc = new();
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







