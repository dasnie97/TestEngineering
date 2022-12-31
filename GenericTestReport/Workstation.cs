using GenericTestReport.Interfaces;

namespace GenericTestReport
{
    public class Workstation : IWorkstation
    {
        public Workstation(string name, string state = null, string customer = null)
        {
            Name = name;
            Customer = customer;
            State = state;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Customer { get; set; }
        public string? State { get; set; }
    }
}
