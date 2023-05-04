namespace TestEngineering.Interfaces;

public interface IWorkstation
{
    string Name { get; }
    string OperatorName { get; }
    public string ProcessStep { get; }
}
