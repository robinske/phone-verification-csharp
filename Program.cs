using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace ConsoleApp
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();

            var app = provider.GetService<Application>();
            await app.StartPhoneVerificationAsync("1", "5551234567");

            // await app.CheckPhoneVerificationAsync("1", "5551234567", "6494");

        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<Application>();
        }
    }

    public class Application
    {
        public async Task StartPhoneVerificationAsync(string country_code, string phone_number)
        {
            var client = new HttpClient();

            var AuthyAPIKey = "YOUR_API_KEY";
            client.DefaultRequestHeaders.Add("X-Authy-API-Key", AuthyAPIKey);

            var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("via", "sms"),
                new KeyValuePair<string, string>("phone_number", phone_number),
                new KeyValuePair<string, string>("country_code", country_code),
            });

            HttpResponseMessage response = await client.PostAsync(
                "https://api.authy.com/protected/json/phones/verification/start",
                requestContent);

            HttpContent responseContent = response.Content;

            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                Console.WriteLine(await reader.ReadToEndAsync());
            }
        }

        public async Task CheckPhoneVerificationAsync(string country_code, string phone_number, string verification_code)
        {
            var client = new HttpClient();

            var AuthyAPIKey = "YOUR_API_KEY";
            client.DefaultRequestHeaders.Add("X-Authy-API-Key", AuthyAPIKey);

            string query;
            using (var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("verification_code", verification_code),
                new KeyValuePair<string, string>("phone_number", phone_number),
                new KeyValuePair<string, string>("country_code", country_code),
            }))
            {
                query = requestContent.ReadAsStringAsync().Result;
            }

            HttpResponseMessage response = await client.GetAsync(
                "https://api.authy.com/protected/json/phones/verification/check?" + query);

            HttpContent responseContent = response.Content;

            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                Console.WriteLine(await reader.ReadToEndAsync());
            }
        }
    }
}