using ProductTest.Common;
using ProductTest.Interfaces;

namespace ProductTest.Models;

public class Workstation : IWorkstation
{
    public string Name { get; set; }
    public string OperatorName { get; set; }

    public Workstation(string name = "", string operatorName = "")
    {
        Name = name;
        OperatorName = operatorName;
    }
}
