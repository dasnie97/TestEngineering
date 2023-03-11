using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTest.Interfaces;

public interface IWorkstation
{
    string Name { get; set; }
    string OperatorName { get; set; }
}
