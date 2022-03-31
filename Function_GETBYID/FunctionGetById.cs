using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace Function_GETBYID
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

    public static class FunctionGetById
    {
        [FunctionName("FunctionGetById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Autos/{id}")] HttpRequest req,
            string id,
            [CosmosDB(
                ConnectionStringSetting = "strCosmos")]
                DocumentClient client,
        ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var colUri = UriFactory.CreateDocumentCollectionUri("dbdistribuida", "tabloide");

            var document = client.CreateDocumentQuery(colUri, option)
                            .Where(x => x.Id == id).AsEnumerable().FirstOrDefault();

            var getAutoId = JsonConvert.DeserializeObject<Auto>(document.ToString());

            var Response = new Response<Auto>
            {
                Data = getAutoId,
                Success = true,
                Method = "GetById"
            };
            return new OkObjectResult(Response);
        }
    }
}
