namespace SampleApp_MPI.Models
{
    public class LabResult
    {
        public Guid Id { get; set; }
        public string? Meta { get; set; }
        public string? Text { get; set; }
        public string? Extension { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? Subject { get; set; }
        public string? Encounter { get; set; }
        public string? Performer { get; set; }
        public decimal ValueQuantity { get; set; }
        public string? Interpretation { get; set; }
        public string? Specimen { get; set; }
    }
}
