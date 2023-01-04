using ProductTest.Interfaces;
using ProductTest.Common;

namespace ProductTest;

public class TestStep : TestStepBase, ITestStep
{
    public static TestStep Create(string name,
                                    DateTime dateTimeCompleted,
                                    string status,
                                    string type = "",
                                    string value = "",
                                    string unit = "",
                                    string lowerLimit = "",
                                    string upperLimit = "",
                                    string failure = "")
    {
        return new TestStep(name, dateTimeCompleted, status, type, value, unit, lowerLimit, upperLimit, failure);
    }
    private TestStep(string name,
                    DateTime dateTimeCompleted,
                    string status,
                    string type = "",
                    string value = "",
                    string unit = "",
                    string lowerLimit = "",
                    string upperLimit = "",
                    string failure = "") : 
        base (name, dateTimeCompleted, status, type, value, unit, lowerLimit, upperLimit, failure)
    {}
}
