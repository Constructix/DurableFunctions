using HydraluxEnquiryDurableFunction.Activities;
using HydraluxEnquiryDurableFunction.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace HydraluxEnquiryDurableFunction.Orchestrators;

public static class HydraluxEnquiryOrchestrator
{
    [Function(nameof(HydraluxEnquiryOrchestrator))]
    public static async Task<string> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {

        var input = context.GetInput<EnquiryRequest>();
        ILogger logger = context.CreateReplaySafeLogger(nameof(HydraluxEnquiryOrchestrator));
        logger.LogInformation("HydraluxEnquiry Orchestrator is currently executing.");
        var enquiry = await context.CallActivityAsync<Enquiry>(nameof(SaveEnquiry.SaveEnquiryToDatabase), input);
        await context.CallActivityAsync(nameof(SendActivities.SendReceivedEnquiryToCustomer), enquiry);
        await context.CallActivityAsync(nameof(SendActivities.SendNewEnquiryToHydralux), enquiry);
        logger.LogInformation("HydraluxEnquiry Orchestrator has completed calling activities.");
        return "OK";
    }
  
    
}