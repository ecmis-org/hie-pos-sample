using static SampleApp_MPI.Utilities.Constants;

namespace SampleApp_MPI.Models
{
    public class Practitioner
    {
        public Guid Id { get; set; }
        public string PIN { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Sex Sex { get; set; }
        public DateTime DOB { get; set; }
    }
}
