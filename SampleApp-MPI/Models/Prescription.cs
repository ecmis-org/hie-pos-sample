using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp_MPI.Models
{
    public class Prescription
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }
        public Guid EncounterId { get; set; }
        [ForeignKey(nameof(EncounterId))]
        public Encounter? Encounter { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public Guid PractitionerId { get; set; }
        [ForeignKey(nameof(PractitionerId))]
        public Practitioner? Practitioner { get; set; }
        public ICollection<Medication>? Dosages { get; set; }
    }

    public class Medication 
    {
        public Guid Id { get; set; }
        public Guid PrescriptionId { get; set; }
        [ForeignKey(nameof(PrescriptionId))]
        public Prescription? Prescription { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }
        public int Quantity { get; set; }
        public Frequency Frequency { get; set; }
        public int Duration { get; set; }
    }

    public class Product 
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ProductForm Form { get; set; }
    }

    public enum ProductForm
    {
        Tablet = 1,
        Capsule = 2,
        Vial = 3
    }

    public enum Frequency 
    {
        Daily = 1,
        InTheMorning = 2,
        TwiceADay = 3,
        ThreeTimesADay = 4
    }
}
