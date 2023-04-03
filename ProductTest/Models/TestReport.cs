namespace ProductTest.Models;

public class TestReport : Common.TestReport
{
    public TestReport(string serialNumber,
                            Common.Workstation workstation,
                            IEnumerable<Common.TestStep> testSteps) : 
        base(serialNumber, workstation, testSteps)
	{}

    protected TestReport() : base()
    { }
}
