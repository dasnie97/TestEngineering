namespace ProductTest.Common;

public abstract class WorkstationBase
{
    public string Name { get; protected set; }

    protected WorkstationBase(string name)
    {
        Name = name;
    }
}
