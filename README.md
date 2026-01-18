# DurableFunctions
Durable functions

### Overview
This repository contains examples and templates for building durable functions using C# .

Durable functions are an extension of Azure Functions that enable you to write stateful functions in a serverless environment. They allow you to define workflows using code, manage state, and handle long-running processes.
### Features
- Orchestrator Functions: Define workflows that can call other functions, wait for external events, and manage state.
- Activity Functions: Implement the individual tasks that make up the workflow.
- Durable Entities: Create stateful objects that can maintain state across function invocations.
- Timers and Delays: Schedule tasks to run at specific times or after certain delays.
- Error Handling: Built-in support for retry policies and error handling in workflows.

Recommended Project Structure: 

/HydraluxEnquiryDurableFunction
    /Orchestrators
        EnquiryOrchestrator.cs
    /Activities
        SaveEnquiryToDatabase.cs
        SendNewEnquiryToHydralux.cs
        SendReceivedEnquiryToCustomer.cs
    /Triggers
        EnquiryHttpStart.cs
    /Models
        EnquiryRequest.cs
        Enquiry.cs
    /Services
        IEnquiryRepository.cs
        EnquiryRepository.cs
        IEmailService.cs
        EmailService.cs
    /Startup
        DependencyInjection.cs
    FunctionHost.cs
