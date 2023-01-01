using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTest.Common;

public abstract class TestStepBase
{
    public string TestName { get; protected set; }
    public string TestType { get; protected set; }
    public DateTime TestDateTimeFinish { get; protected set; }
    public string TestStatus { get; protected set; }
    public string TestValue { get; protected set; }
    public string ValueUnit { get; protected set; }
    public string TestLowerLimit { get; protected set; }
    public string TestUpperLimit { get; protected set; }
    public bool IsNumeric { get; protected set; }
    public string Failure { get; protected set; }
}
