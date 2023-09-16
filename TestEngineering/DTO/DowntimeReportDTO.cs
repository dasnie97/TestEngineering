using TestEngineering.Models;

namespace TestEngineering.DTO;

public class DowntimeReportDTO
{
    public int Id { get; set; }
    public string ProblemDescription { get; set; }
    public string ActionTaken { get; set; }
    public string Technician { get; set; }
    public string Workstation { get; set; }
    public string Operator { get; set; }
    public DateTime TimeStarted { get; set; }
    public DateTime TimeFinished { get; set; }
    public TimeSpan TotalDowntime { get; set; }
    public DateTime RecordCreated { get; set; }
}
