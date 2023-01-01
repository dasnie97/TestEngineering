using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTest.Common;

public class TestStatus
{
    public static readonly TestStatus Passed = Passed;
    public static readonly TestStatus Failed = Failed;
    public static readonly TestStatus Terminated = Terminated;
    public static readonly TestStatus Error = Error;
}
