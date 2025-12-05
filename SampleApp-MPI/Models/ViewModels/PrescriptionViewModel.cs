using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using SampleApp_MPI.DAL;
using SampleApp_MPI.Translators;
using SampleApp_MPI.Utilities;

namespace SampleApp_MPI.Models.ViewModels
{
    public class PrescriptionViewModel
    {
        public Prescription Prescription { get; set; }
        public Medication Medication { get; set; }
        public Encounter Encounter { get; set; }
        public Patient? Patient { get; set; }
        public SelectList? Departments { get; set; }
        public SelectList? ServicePoints { get; set; }
        public SelectList? Locations { get; set; }
        public SelectList? Products { get; set; }
        public SelectList? Prescribers { get; set; }

        public async Task<PrescriptionViewModel> Init(DataContext _context, string patientId)
        {
            Departments = new SelectList(_context.Department.ToList(), "Id", "Name", Encounter?.DepartmentId);

            ServicePoints = new SelectList(_context.ServicePoint.ToList(), "Id", "Name", Encounter?.ServicePointId);

            Locations = new SelectList(_context.Location.ToList(), "Id", "Name");

            Products = new SelectList(_context.Product.ToList(), "Id", "Name");

            Prescribers = new SelectList(_context.Practitioner
                .OrderBy(m => m.LastName)
                .Select(m => new { Id = m.Id, Name = $"{m.LastName} {m.FirstName}" }).ToList(), "Id", "Name");

            var patient = await FHIRParser.GetPatientResource("1af5f264-52cc-4fcc-8a01-353fe94bd43a");
            if (patient != null)
            {
                Patient = patient;

                patient.AlternateId = "XYZ"; //TO DO: Ensure this is fetched correctly from HIE
                _context.Patient.Add(patient);
                _context.SaveChanges();
            }
            return this;
        }
    }
}
