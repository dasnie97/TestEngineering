using ProductTest.Models;

namespace ProductTestTest;

public class FileTestReportCreator
{
    public string SerialNumber = Guid.NewGuid().ToString();
    public Workstation Workstation = new Workstation(Guid.NewGuid().ToString());
    public List<TestStep> TestSteps { get; set; } = new List<TestStep>()
    { 
        new TestStep("Test1", new DateTime(2023, 1, 14, 21, 37, 00), ProductTest.Common.TestStatus.Failed) 
    };

    public FileTestReport Create()
    {
        return FileTestReport.Create(SerialNumber, Workstation, TestSteps);
    }
}
