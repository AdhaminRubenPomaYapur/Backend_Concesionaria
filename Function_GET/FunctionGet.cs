using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Function_GET
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

    public class Response<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public static class FunctionGet
    {
        [FunctionName("FunctionGet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Autos/")] HttpRequest req,
            [CosmosDB(
                databaseName: "dbproyecto",
                collectionName: "proyecto",
                ConnectionStringSetting = "strCosmos")]
                IEnumerable<Auto> itemsAutos,
        ILogger log)
        {
            var Response = new Response<IEnumerable<Auto>>
            {
                Data = itemsAutos,
                Success = true,
                Method = "GET"
            };
            return new OkObjectResult(Response);
        }
    }
}
