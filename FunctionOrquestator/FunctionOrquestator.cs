using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionOrquestator
{
    public class Auto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("modelo")]
        public string Modelo { get; set; }
        [JsonProperty("marca")]
        public string Marca { get; set; }
        [JsonProperty("serie")]
        public string Serie { get; set; }
        [JsonProperty("precio")]
        public double Precio { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
    }

    public class Request
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("body")]
        public Auto Body { get; set; }
    }
    public static class FunctionOrquestator
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("FunctionOrquestator")]
        [return: ServiceBus("salida", EntityType.Queue)]
        public static async Task<string> Run([ServiceBusTrigger("entrada", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
        {
            var request = JsonConvert.DeserializeObject<Request>(myQueueItem);
            string message = string.Empty;
            if (request.Method == "GET")
            {
                var response = await httpClient.GetAsync("https://functiongetautos.azurewebsites.net/api/Autos/");
                message = await response.Content.ReadAsStringAsync();

            }
            if (request.Method == "POST")
            {
                var response = await httpClient.PostAsJsonAsync("https://functionpostautos.azurewebsites.net/api/Autos/", request.Body);
                message = await response.Content.ReadAsStringAsync();
            }
            if (request.Method == "PUT")
            {
                var response = await httpClient.PutAsJsonAsync("https://functionputautos.azurewebsites.net/api/Autos/" + request.Id, request.Body);
                message = await response.Content.ReadAsStringAsync();
            }
            if (request.Method == "DELETE")
            {
                var response = await httpClient.DeleteAsync("https://functiondeleteautos.azurewebsites.net/api/Autos/" + request.Id);
                message = await response.Content.ReadAsStringAsync();
            }
            return message;
        }
    }
}
