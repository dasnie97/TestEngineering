using ProductTest.Common;
using ProductTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTestTest;

public class FileTestReportCreator
{
    public string SerialNumber = Guid.NewGuid().ToString();
    public string Workstation = Guid.NewGuid().ToString();
    public string Status = TestStatus.Failed;
    public DateTime DateAndTime = new DateTime(2023, 1, 14, 21, 37, 00);
    public List<TestStepBase> TestSteps { get; set; } = new List<TestStepBase>() { TestStep.Create("Test1", DateTime.Now, TestStatus.Failed) };

    public FileTestReport Create()
    {
        TestSteps = new List<TestStepBase>()
    {
        TestStep.Create("Test1", DateAndTime, Status)
    };
        return FileTestReport.Create(SerialNumber, Workstation, TestSteps);
    }
}
