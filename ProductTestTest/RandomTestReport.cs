using ProductTest.Common;
using ProductTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProductTestTest;

public static class RandomTestReport
{
    public static string Status = TestStatus.Failed;
    public static string Workstation = "TEST_WORKSTATION";
    public static DateTime DateTimeStarted = new DateTime(2023, 1, 14, 21, 37, 00);
    public static List<TestStepBase> TestSteps = new List<TestStepBase>()
    {
        TestStep.Create("Test1", new DateTime(2023, 1, 14, 21, 37, 00), TestStatus.Failed)
    };
    public static string SerialNumber = RandomNumberGenerator.GetInt32(1000).ToString();

    public static FileTestReport ArrangeTestReportWithDefaultAndRandomData()
    {
        return FileTestReport.Create(SerialNumber, Workstation, TestSteps);
    }
}
