namespace TestEngineering.DTO;

public class TestReportDTO
{
    public int Id { get; set; }
    public string Workstation { get; set; }
    public string SerialNumber { get; set; }
    public string Status { get; set; }
    public DateTime TestDateTimeStarted { get; set; }
    public TimeSpan TestingTime { get; set; }
    public string FixtureSocket { get; set; }
    public string Failure { get; set; }
    public bool IsFirstPass { get; set; }
    public bool? IsFalseCall { get; set; }
    public DateTime RecordCreated { get; set; }
}
