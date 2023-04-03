using ProductTest.Interfaces;

namespace ProductTest.Models;

public class Workstation : Common.Workstation, IWorkstation
{ 
    public Workstation(string name = "", string operatorName = "")
    {
        Name = name;
        OperatorName = operatorName;
    }
}
