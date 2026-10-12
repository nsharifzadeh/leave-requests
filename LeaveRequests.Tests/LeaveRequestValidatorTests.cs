namespace LeaveRequests.Tests;

public class UnitTest1
{
    public static readonly DateOnly Jan1 = new DateOnly(2026, 1, 1);
    public static readonly DateOnly Jan5 = new DateOnly(2026, 1, 5);
    [Fact]
    public void Valid_request_has_no_error()
    {
        var errors = LeaveRequestValidator
            .Validate(new LeaveRequest("E12345", Jan1, Jan5));
        Assert.Empty(errors);
    }
    [Fact]
    public void Missing_employee_id_is_rejected()
    {
        var errors = LeaveRequestValidator.Validate(new LeaveRequest("", Jan1, Jan5));
        Assert.Contains("Employee Id is requred", errors);
    }

    [Fact]
    public void End_before_start_is_rejected()
    {
        var errors = LeaveRequestValidator.Validate(new LeaveRequest("E123", Jan5, Jan1));
        Assert.Contains("End must be on or after Start.", errors);
    }

    [Fact]
    public void Null_body_is_rejected()
    {
        var errors = LeaveRequestValidator.Validate(null);
        Assert.Contains("Body is required.", errors);
    }

}
