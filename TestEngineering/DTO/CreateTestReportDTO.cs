namespace TestEngineering.DTO;

public class CreateTestReportDTO
{
    public string Workstation { get; set; }
    public string SerialNumber { get; set; }
    public string Status { get; set; }
    public DateTime TestDateTimeStarted { get; set; }
    public TimeSpan TestingTime { get; set; }
    public string FixtureSocket { get; set; }
    public string Failure { get; set; }
}
