using TestEngineering.Interfaces;
using TestEngineering.Models;

namespace TestEngineering.DTO;

public static class DTOConverter
{
    public static TestReportDTO ToTestReportDTO(TestReport source)
    {
        TestReportDTO dto = new TestReportDTO();

        dto.Workstation = source.Workstation.Name;
        dto.SerialNumber = source.SerialNumber;
        dto.Status = source.Status.ToString();
        dto.TestDateTimeStarted = source.TestDateTimeStarted;
        dto.TestingTime = source.TestingTime;
        dto.FixtureSocket = source.FixtureSocket;
        dto.Failure = source.Failure;

        return dto;
    }
}
