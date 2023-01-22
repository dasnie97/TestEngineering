namespace ProductTest.Common;

public abstract class WorkstationBase
{
    public string Name { get; protected set; }
    public string OperatorName { get; protected set; }

    protected WorkstationBase(string name = "", string operatorName = "")
    {
        Name = name;
        OperatorName = operatorName;
    }
}
