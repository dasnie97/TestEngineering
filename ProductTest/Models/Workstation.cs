using ProductTest.Common;
using ProductTest.Interfaces;

namespace ProductTest.Models;

public class Workstation : WorkstationBase, IWorkstation
{ 
    public Workstation(string name = "", string operatorName = "")
    {
        Name = name;
        OperatorName = operatorName;
    }
}
