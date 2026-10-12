using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public record LeaveRequest(string EmployeeId, DateOnly Start, DateOnly End);

public static class LeaveRequestValidator
{
    public static List<string> Validate(LeaveRequest? r)
    {
        var errors = new List<string>();
        if (r is null) { errors.Add("Body is required."); return errors; }
        if (string.IsNullOrWhiteSpace(r.EmployeeId)) errors.Add("EmployeeId is required.");
        if (r.End < r.Start) errors.Add("End must be on or after Start.");
        return errors;
    }
}

public class SubmitLeaveRequest(ILogger<SubmitLeaveRequest> logger)
{
    [Function("SubmitLeaveRequest")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "leave-requests")] HttpRequest req)
    {
        LeaveRequest? request;
        try
        {
            request = await req.ReadFromJsonAsync<LeaveRequest>();
        }
        catch(JsonException)
        {
            return new BadRequestObjectResult(new[] { "Body must be valid" });
        }
        var errors = LeaveRequestValidator.Validate(request);
        if (errors.Count > 0) return new BadRequestObjectResult(errors);

        logger.LogInformation("Leave request accepted for {EmployeeId}", request!.EmployeeId);
        return new AcceptedResult();
    }
}