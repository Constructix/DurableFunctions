using HydraluxEnquiryDurableFunction.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HydraluxEnquiryDurableFunction.Activities;

public class SendActivities
{
    [Function(nameof(SendReceivedEnquiryToCustomer))]
    public static void SendReceivedEnquiryToCustomer([ActivityTrigger] Enquiry enquiry, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(SendActivities.SendReceivedEnquiryToCustomer));
        logger.LogInformation("Sending Received Email notification to {name}.", enquiry.Name);
        // Simulate saving to database


    }

    [Function(nameof(SendNewEnquiryToHydralux))]
    public static void SendNewEnquiryToHydralux([ActivityTrigger] Enquiry enquiry, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(SendActivities.SendNewEnquiryToHydralux));
        logger.LogInformation("Sending Enquiry notification to Hydralux, from {name}.", enquiry.Name);
        // Simulate saving to database


    }
}