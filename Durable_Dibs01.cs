using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Durablefunc01
{
    public static class Durable_Dibs01
    {
        [FunctionName("Durable_Dibs01")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("Durable_Dibs01_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("Durable_Dibs01_Hello2", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("Durable_Dibs01_Hello3", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("Durable_Dibs01_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello1 {name}!";
        }


        [FunctionName("Durable_Dibs01_Hello2")]
        public static string SayHello2([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello2 to {name}.");
            return $"Hello2 {name}!";
        }

        [FunctionName("Durable_Dibs01_Hello3")]
        public static string SayHello3([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello3 to {name}.");
            return $"Hello3 {name}!";
        }


        [FunctionName("Durable_Dibs01_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Durable_Dibs01", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}