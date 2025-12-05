using Microsoft.AspNetCore.Identity;

namespace SampleApp_MPI.Models
{
    public class Vital
    {
        public Guid Id { get; set; }
        public string? Meta { get; set; }
        public string? Text { get; set; }
        public string? Extension { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? Code { get; set; }
        public string? Subject { get; set; }
        public DateTime  Date { get; set; }
        public double ValueQuantity { get; set; }
        public string? Interpretation { get; set; }
        public string? Specimen { get; set; }
    }
}
