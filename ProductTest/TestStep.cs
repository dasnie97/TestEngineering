using ProductTest.Interfaces;
using ProductTest.Common;

namespace ProductTest;

public class TestStep : TestStepBase, ITestStep
{
    public TestStep(string testName, DateTime testDateTime, string testStatus, string testType = "", string testValue = "", string valueUnit = "", string testLL = "", string testUL = "", string failure = "")
    {
        this.TestName = testName;
        this.TestType = testType;
        this.TestDateTimeFinish = testDateTime;
        this.TestStatus = testStatus;
        this.TestValue = testValue;
        this.ValueUnit = valueUnit;
        this.TestLowerLimit = testLL;
        this.TestUpperLimit = testUL;
        this.Failure = failure;
        this.IsNumeric = EvaluateTestType();
    }

    /// <summary>
    /// Check if test step is of numeric type.
    /// </summary>
    /// <returns>True if test step is numeric and can be used for MSA. Returns false if test step is of pass/fail or string type.</returns>
    private bool EvaluateTestType()
    {
        return float.TryParse(this.TestValue, out _) && (this.TestLowerLimit != this.TestUpperLimit);
    }
}
