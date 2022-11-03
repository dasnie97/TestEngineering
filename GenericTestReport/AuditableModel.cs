namespace GenericTestReport
{
    public abstract class AuditableModel
    {
        public string Status { get; set; } = null!;
        public DateTime RecordCreated { get; set; }
        public DateTime TestDateTimeStarted { get; set; }
    }
}
