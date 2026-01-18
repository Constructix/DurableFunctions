namespace HydraluxEnquiryDurableFunction.Models;

public record EnquiryRequest(string Name, string Phone, string Email, string ServiceRequired, string Message);