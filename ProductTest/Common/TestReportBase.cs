namespace ProductTest.Common;

public abstract class TestReportBase
{
    public string SerialNumber { get; protected set; }
    public string Status { get; protected set; }
    public Workstation Workstation { get; protected set; }
    public IEnumerable<TestStepBase> TestSteps { get; protected set; }
    public DateTime TestDateTimeStarted { get; protected set; }
    public string Failure { get; protected set; }
    public string? FixtureSocket { get; protected set; }
    public TimeSpan? TestingTime { get; protected set; }
    //public bool IsFirstPass { get; set; }
    //public bool FalseCall { get; set; }

    protected TestReportBase(string serialNumber,
                            string status,      
                            string workstation,
                            DateTime testStarted,
                            IEnumerable<TestStep> testSteps,
                            string failure = "",
                            string fixtureSocket = "",
                            TimeSpan? testingTime = null
    )
    {
        SerialNumber = serialNumber;
        Status = status;
        Workstation = new Workstation(workstation);
        TestDateTimeStarted = testStarted;
        TestSteps = testSteps;
        Failure = failure;
        FixtureSocket = fixtureSocket;
        TestingTime = testingTime;
    }
}