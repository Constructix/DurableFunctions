namespace HydraluxEnquiryDurableFunction.Models;

public record Enquiry(int EnquiryId, string Name, string Phone, string Email, string ServiceRequired, string Message);