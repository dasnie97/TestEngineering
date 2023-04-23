namespace TestEngineering.Models;

public class TestReport
{
    public Workstation Workstation { get; protected set; }
    public string SerialNumber { get; protected set; }
    public TestStatus Status { get; protected set; }
    public DateTime TestDateTimeStarted { get; protected set; }
    public IEnumerable<TestStep> TestSteps { get; protected set; }
    public TimeSpan TestingTime { get; protected set; }
    public string FixtureSocket { get; protected set; }
    public string Failure { get; protected set; }


    public TestReport(string serialNumber,
                            Workstation workstation,
                            IEnumerable<TestStep> testSteps)
    {
        SerialNumber = serialNumber;
        Workstation = workstation;
        TestSteps = testSteps;
        SetTestDateAndTime();
        SetStatus();
        SetFailedStepData();
        SetTestSocket();
        SetBoardTestingTime();
    }
    public TestReport()
    {
        TestSteps = new List<TestStep>();
        TestDateTimeStarted = DateTime.MinValue;
        Status = TestStatus.NotSet;
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
        foreach (TestStep testStep in TestSteps)
        {
            if (testStep.Status == TestStatus.Passed)
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
            if (test.Status == TestStatus.Failed)
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
            TestingTime = TimeSpan.MinValue;
        }
    }
}