using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace Function_INQUEUE
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

    public static class FunctionInqueue
    {
        [FunctionName("FunctionInqueue")]
        [return: ServiceBus("entrada", ServiceBusEntityType.Queue)]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "Autos/")] HttpRequest req,
            ILogger log
        )
        {
            string method = req.Method;
            string id = req.Query["id"];
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            var auto = JsonConvert.DeserializeObject<Auto>(body);
            var message = new Request
            {
                Method = method,
                Id = id,
                Body = auto
            };
            return JsonConvert.SerializeObject(message);
        }
    }
}
