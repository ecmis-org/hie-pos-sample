using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp_MPI.Models
{
    public class Encounter
    {
        public Guid Id { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }
        public int ServicePointId { get; set; }
        [ForeignKey(nameof(ServicePointId))]
        public ServicePoint? ServicePoint { get; set; }
    }

    public class Department 
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class ServicePoint 
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
