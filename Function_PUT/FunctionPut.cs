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

namespace Function_PUT
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

    public static class FunctionPut
    {
        [FunctionName("FunctionPut")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Autos/{id}")] HttpRequest req,
            string id,
            [CosmosDB(
                ConnectionStringSetting = "strCosmos")]
                DocumentClient client
        )
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updateAuto = JsonConvert.DeserializeObject<Auto>(requestBody);
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var colUri = UriFactory.CreateDocumentCollectionUri("dbproyecto", "proyecto");
            var document = client.CreateDocumentQuery(colUri, option)
                            .Where(x => x.Id == id).AsEnumerable().FirstOrDefault();
            if (document != null)
            {
                document.SetPropertyValue("id", updateAuto.Id);
                document.SetPropertyValue("name", updateAuto.Name);
                document.SetPropertyValue("modelo", updateAuto.Modelo);
                document.SetPropertyValue("marca", updateAuto.Marca);
                document.SetPropertyValue("serie", updateAuto.Serie);
                document.SetPropertyValue("precio", updateAuto.Precio);
                document.SetPropertyValue("color", updateAuto.Color);
                await client.ReplaceDocumentAsync(document);
            }
            var Response = new Response<Auto>
            {
                Data = updateAuto,
                Success = true,
                Method = "PUT"
            };
            return new OkObjectResult(Response);

        }
    }
}
