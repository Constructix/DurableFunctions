using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HydraluxEnquiryDurableFunction;

public static class Function1
{
    [Function(nameof(Function1))]
    public static async Task<string> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {

        var input = context.GetInput<EnquiryRequest>();
        ILogger logger = context.CreateReplaySafeLogger(nameof(Function1));
        logger.LogInformation("Saying hello.");
        var enquiry = await context.CallActivityAsync<Enquiry>(nameof(SaveEnquiryToDatabase), input);
        await context.CallActivityAsync(nameof(SendNewEquiryToHydralux), enquiry);
        await context.CallActivityAsync(nameof(SendReceivedEnquiryToCustomer), enquiry);

        return "OK";
    }
    [Function(nameof(SaveEnquiryToDatabase))]
    public static Enquiry SaveEnquiryToDatabase([ActivityTrigger] EnquiryRequest enquiry, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SaveEnquiryToDatabase");
        logger.LogInformation("Saving enquiry from {name} to database.", enquiry.Name);
        // Simulate saving to database
        var newEnquiry = new Enquiry(1, enquiry.Name, enquiry.Phone, enquiry.Email, enquiry.ServiceRequired,
            enquiry.Message);
        return newEnquiry;
    }
    [Function(nameof(SendNewEquiryToHydralux))]
    public static void SendNewEquiryToHydralux([ActivityTrigger] Enquiry enquiry, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SendNewEquiryToHydralux");
        logger.LogInformation("Sending Enquiry notification to Hydralux, from {name}.", enquiry.Name);
        // Simulate saving to database
        
        
    }
    [Function(nameof(SendReceivedEnquiryToCustomer))]
    public static void SendReceivedEnquiryToCustomer([ActivityTrigger] Enquiry enquiry, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SendReceivedEnquiryToCustomer");
        logger.LogInformation("Sending Received Email notification to {name}.", enquiry.Name);
        // Simulate saving to database


    }



    [Function("Function1_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous,  "post")] HttpRequestData req,
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
                nameof(Function1), payload);

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
        catch(JsonReaderException jsonReaderEx)
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

public record payloadRequest(string Message);

public record EnquiryRequest(string Name, string Phone, string Email, string ServiceRequired, string Message);
public record Enquiry(int EnquiryId, string Name, string Phone, string Email, string ServiceRequired, string Message);

