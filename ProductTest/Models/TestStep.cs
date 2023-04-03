namespace ProductTest.Models;

public class TestStep : Common.TestStep
{
    public TestStep(string name,
                    DateTime dateTimeCompleted,
                    TestStatus status) :
        base(name, dateTimeCompleted, status)
    { }
}
