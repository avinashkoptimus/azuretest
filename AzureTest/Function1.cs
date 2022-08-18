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

namespace AzureTest
{
    public static class Function1
    {
        [FunctionName("AccessLogs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
            [Sql("AccessLogs",ConnectionStringSetting = "myconnections")] IAsyncCollector<AccessLogs> logsitems
            ,ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<List<AccessLogs>>(requestBody);
            foreach (var item in data)
            {
                await logsitems.AddAsync(item);
            }
            return new OkObjectResult("Record added");
        }
    }
}
