using HydraluxEnquiryDurableFunction.Models;
using HydraluxEnquiryDurableFunction.Orchestrators;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HydraluxEnquiryDurableFunction.Triggers
{
    public class HttpTriggerFunction
    {

        [Function("HydraluxEnquiryStart")]
        public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
        {
            try
            {
                var payload = await req.ReadFromJsonAsync<EnquiryRequest>();

                if (payload == null)
                {
                    var response = req.CreateResponse(HttpStatusCode.BadRequest);
                    await response.WriteStringAsync("No Request body submitted.");
                    return response;
                }

                var purgeResult = await client.PurgeAllInstancesAsync(new PurgeInstancesFilter());

                ILogger logger = executionContext.GetLogger("Function1_HttpStart");

                // Function input comes from the request content.
                string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                    nameof(HydraluxEnquiryOrchestrator), payload);

                logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

                // Returns an HTTP 202 response with an instance management payload.
                // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
                return await client.CreateCheckStatusResponseAsync(req, instanceId);
            }
            catch (AggregateException aggregateException)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync("Request body is valid, multiple errors encountered.");
                return response;
            }
            catch (JsonReaderException jsonReaderEx)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync("No Request body submitted.");
                return response;
            }
            catch (JsonException jsonException)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync("Request body is not valid Json.");
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
