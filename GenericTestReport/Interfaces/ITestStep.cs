namespace GenericTestReport.Interfaces
{
    public interface ITestStep
    {
        public string TestName { get;}
        public string TestType { get;}
        public DateTime TestDateTimeFinish { get; }
        public string TestStatus { get; }
        public string TestValue { get; }
        public string ValueUnit { get; }
        public string TestLowerLimit { get; }
        public string TestUpperLimit { get; }
        public bool IsNumeric { get; }
        public string Failure { get; }
    }
}
