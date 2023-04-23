namespace TestEngineering.Models;

public class TestStep
{
    public string Name { get; protected set; }
    public DateTime DateTimeFinish { get; protected set; }
    public TestStatus Status { get; protected set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public string Unit { get; set; }
    public string LowerLimit { get; set; }
    public string UpperLimit { get; set; }
    public string Failure { get; set; }
    public bool IsNumeric { get; protected set; }

    public TestStep(string name,
                    DateTime dateTimeFinish,
                    TestStatus status)
    {
        Name = name;
        DateTimeFinish = dateTimeFinish;
        Status = status;
        CheckIfNumericAndSet();
    }
    private void CheckIfNumericAndSet()
    {
        IsNumeric = float.TryParse(Value, out _) && LowerLimit != UpperLimit;
    }
}