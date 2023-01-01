using ProductTest.Common;
using ProductTest.Interfaces;

namespace ProductTest;

public class TestReport : TestReportBase, ITestReport
{
    ////public bool IsFirstPass { get; set; }
    ////public bool FalseCall { get; set; }
    ////public string? TestProgramFilePath { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path to existing test report.</param>
    public TestReport(string path) : base(path) { }
    public TestReport(
        string serialNumber,
        string workstation,
        DateTime testStarted,
        IEnumerable<TestStep> testSteps,
        string failure = "",
        string fixtureSocket = "",
        TimeSpan? testingTime = null
        ) : base
        (serialNumber,
        workstation,
        testStarted,
        testSteps,
        failure,
        fixtureSocket,
        testingTime)
    { }

    public string SaveReport(string directoryPath)
    {
        var logFileName = $"{TestDateTimeStarted.Month:00}{TestDateTimeStarted.Day:00}{TestDateTimeStarted.Year}_{TestDateTimeStarted.Hour:00}{TestDateTimeStarted.Minute:00}{TestDateTimeStarted.Second:00}_{SerialNumber}.txt";
        var buffor = new List<string>
        {
            $"PanelBarcode:\t{SerialNumber}",
            $"TestProgram:\t{""}",
            $"TestProgramVer:\t{"1.0"}",
            $"Operator:\t{Workstation.Name}",
            $"ImageBarcode:\t{SerialNumber}",
            $""
        };

        foreach (TestStepBase testStep in TestSteps)
        {
            buffor.Add($"TestName:\t{testStep.TestName}");
            if (!string.IsNullOrEmpty(testStep.TestType))
                buffor.Add($"TestType:\t{testStep.TestType}");
            buffor.Add($"Date:\t{testStep.TestDateTimeFinish.Month:00}/{testStep.TestDateTimeFinish.Day:00}/{testStep.TestDateTimeFinish.Year}");
            buffor.Add($"Time:\t{testStep.TestDateTimeFinish.Hour:00}:{testStep.TestDateTimeFinish.Minute:00}:{testStep.TestDateTimeFinish.Second:00}");
            buffor.Add($"Result:\t{testStep.TestStatus}");
            if (!string.IsNullOrEmpty(testStep.TestValue))
                buffor.Add($"Value:\t{testStep.TestValue}");
            if (!string.IsNullOrEmpty(testStep.ValueUnit))
                buffor.Add($"Units:\t{testStep.ValueUnit}");
            if (!string.IsNullOrEmpty(testStep.TestLowerLimit))
                buffor.Add($"LowerLimit:\t{testStep.TestLowerLimit}");
            if (!string.IsNullOrEmpty(testStep.TestUpperLimit))
                buffor.Add($"UpperLimit:\t{testStep.TestUpperLimit}");
            if (!string.IsNullOrEmpty(testStep.Failure))
                buffor.Add($"FailDesc:\t{testStep.Failure}");
            buffor.Add("~#~");
        }

        var logFilePath = Path.Combine(directoryPath, logFileName);

        File.WriteAllLines(logFilePath, buffor);

        return logFilePath;
    }
}
