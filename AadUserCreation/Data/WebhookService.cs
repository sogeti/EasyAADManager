using AadUserCreation.Data.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AadUserCreation.Data
{
    public class WebhookService: IWebhookService
    {


        public async Task<string> CallWebHook(string userInjson, string webhookToCall)
        {
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://s2events.azure-automation.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var stringContent = new StringContent(userInjson);

                var response = await client.PostAsync(webhookToCall, stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var reader = response.Content;
                    var content = await reader.ReadAsStringAsync();
                    return content;
                }
                return "Error connecting to runbook";

            }
        }
    }
}
