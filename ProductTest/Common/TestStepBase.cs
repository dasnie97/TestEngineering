namespace ProductTest.Common;

public abstract class TestStepBase
{
    public string Name { get; protected set; }
    public string Type { get; protected set; }
    public DateTime DateTimeFinish { get; protected set; }
    public string Status { get; protected set; }
    public string Value { get; protected set; }
    public string Unit { get; protected set; }
    public string LowerLimit { get; protected set; }
    public string UpperLimit { get; protected set; }
    public bool IsNumeric { get; protected set; }
    public string Failure { get; protected set; }

    protected TestStepBase(string name,
                            DateTime dateTimeFinish,
                            string status,
                            string type = "",
                            string value = "",
                            string unit = "",
                            string lowerLimit = "",
                            string upperLimit = "",
                            string failure = "")
    {
        Name = name;
        DateTimeFinish = dateTimeFinish;
        Status = status;
        Type = type;
        Value = value;
        Unit = unit;
        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        Failure = failure;
        SetNumeric();
    }
    private void SetNumeric()
    {
        IsNumeric = float.TryParse(Value, out _) && (LowerLimit != UpperLimit);
    }
}