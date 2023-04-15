using ProductTest.Interfaces;

namespace ProductTest.Models;

public class Workstation : IWorkstation
{
    public string Name { get; protected set; }
    public string OperatorName { get; protected set; }
    public string ProcessStep { get; protected set; }

    public Workstation(string name = "", string operatorName = "")
    {
        Name = name;
        OperatorName = operatorName;
    }
}
