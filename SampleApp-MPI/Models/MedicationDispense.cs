using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp_MPI.Models
{
    public class MedicationDispense
    {
        public Guid Id { get; set; }
        public Guid MedicationId { get; set; }
        [ForeignKey(nameof(MedicationId))]
        public Medication? Medication { get; set; }
        public Guid? DispenserId { get; set; }
        public DateTime DatePrepared { get; set; }
        public DateTime? DateDispensed { get; set; }    
        public int QuantityDispensed { get; set; }
        public int DaysSupply { get; set; }
    }
}
