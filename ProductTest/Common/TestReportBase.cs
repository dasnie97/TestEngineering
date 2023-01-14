using ProductTest.Models;

namespace ProductTest.Common;

public abstract class TestReportBase
{
    public string SerialNumber { get; protected set; }
    public string Status { get; protected set; }
    public WorkstationBase Workstation { get; protected set; }
    public IEnumerable<TestStepBase> TestSteps { get; protected set; }
    public DateTime TestDateTimeStarted { get; protected set; }
    public string Failure { get; protected set; }
    public string? FixtureSocket { get; protected set; }
    public TimeSpan? TestingTime { get; protected set; }
    protected TestReportBase(string serialNumber,
                            string status,      
                            string workstation,
                            DateTime testStarted,
                            List<TestStepBase> testSteps
    )
    {
        SerialNumber = serialNumber;
        Status = status;
        Workstation = new Workstation(workstation);
        TestDateTimeStarted = testStarted;
        TestSteps = testSteps;
        SetFailedStepData();
        SetTestSocket();
        SetBoardTestingTime();
    }
    protected TestReportBase()
    {
        SerialNumber = string.Empty;
        Status = string.Empty;
        Workstation = new Workstation("Default");
        TestDateTimeStarted = DateTime.Now;
        TestSteps = new List<TestStepBase>();
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
}