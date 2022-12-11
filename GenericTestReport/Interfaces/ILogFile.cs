namespace GenericTestReport.Interfaces
{
    public interface ILogFile<T>
    {
        public string Workstation { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
        public string? FixtureSocket { get; set; }
        public string Failure { get; set; }
        public string? Operator { get; set; }
        public string? TestProgramFilePath { get; set; }
        public List<T>? TestSteps { get; set; }
        public TimeSpan? TestingTime { get; set; }
        public DateTime TestDateTimeStarted { get; set; }
    }
}
