using HydraluxEnquiryDurableFunction.Models;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HydraluxEnquiryDurableFunction.Activities
{
    public  class SaveEnquiry
    {
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
    }
}
