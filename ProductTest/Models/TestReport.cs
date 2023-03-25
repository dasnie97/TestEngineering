using ProductTest.Common;

namespace ProductTest.Models;

public class TestReport : TestReportBase
{
    public static TestReport Create(string serialNumber,
                            string workstation,
                            List<TestStepBase> testSteps)
    {
        return new TestReport(serialNumber, workstation, testSteps);
    }

    protected TestReport(string serialNumber,
                            string workstation,
                            List<TestStepBase> testSteps) : 
        base(serialNumber, workstation, testSteps)
	{}

    protected TestReport() : base()
    { }
}
