using ProductTest.Interfaces;
using ProductTest.Models;

namespace ProductTest.Common;

public abstract class TestReportBase
{
    public string SerialNumber { get; protected set; }
    public WorkstationBase Workstation { get; protected set; } = new Workstation();
    public IEnumerable<TestStepBase> TestSteps { get; protected set; }
    public DateTime TestDateTimeStarted { get; protected set; }
    public string Status { get; protected set; }
    public string Failure { get; protected set; }
    public string? FixtureSocket { get; protected set; }
    public TimeSpan? TestingTime { get; protected set; }

    protected TestReportBase(string serialNumber,
                            string workstation,
                            List<TestStepBase> testSteps)
    {
        SerialNumber = serialNumber;
        Workstation.Name = workstation;
        TestSteps = testSteps;
        SetTestDateAndTime();
        SetStatus();
        SetFailedStepData();
        SetTestSocket();
        SetBoardTestingTime();
    }
    protected TestReportBase()
    {
        SerialNumber = string.Empty;
        TestSteps = new List<TestStepBase>();
        TestDateTimeStarted = DateTime.Now;
        Status = string.Empty;
        Failure = string.Empty;
    }

    protected virtual void SetTestDateAndTime()
    {
        try
        {
            var min = TestSteps.First().DateTimeFinish;
            foreach (var testStep in TestSteps)
            {
                if (testStep.DateTimeFinish < min)
                    min = testStep.DateTimeFinish;
            }
            TestDateTimeStarted = min;
        }
        catch
        {
            TestDateTimeStarted = DateTime.MinValue;
        }
    }

    protected virtual void SetStatus()
    {
        int passedTests = 0;
        foreach (TestStepBase testStep in TestSteps)
        {
            if (testStep.Status.Contains("pass", StringComparison.OrdinalIgnoreCase))
                passedTests++;
        }
        if (passedTests == TestSteps.Count() && passedTests != 0)
            Status = TestStatus.Passed;
        else
            Status = TestStatus.Failed;
    }

    protected virtual void SetFailedStepData()
    {
        var failDetails = "";
        foreach (var test in TestSteps)
        {
            if (test.Status.Contains("fail", StringComparison.OrdinalIgnoreCase))
                failDetails = $"{test.Name}\nValue measured: {test.Value}\nLower limit: {test.LowerLimit}\nUpper limit: {test.UpperLimit}";
        }
        Failure = failDetails;
    }

    protected virtual void SetTestSocket()
    {
        var socketNumberTest = TestSteps.Where(testStep => testStep.Name == "Test Socket Number");
        if (socketNumberTest.Any())
        {
            FixtureSocket = socketNumberTest.First().Value;
        }
        else
        {
            FixtureSocket = "";
        }
    }

    protected virtual void SetBoardTestingTime()
    {
        try
        {
            var minTime = TestSteps.Min(testStep => testStep.DateTimeFinish);
            var maxTime = TestSteps.Max(testStep => testStep.DateTimeFinish);
            TestingTime = maxTime - minTime;
        }
        catch
        {
            TestingTime = null;
        }
    }
}