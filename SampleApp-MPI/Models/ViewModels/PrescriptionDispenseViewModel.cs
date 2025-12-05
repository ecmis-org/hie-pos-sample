using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using SampleApp_MPI.DAL;
using SampleApp_MPI.Translators;
using SampleApp_MPI.Utilities;

namespace SampleApp_MPI.Models.ViewModels
{
    public class PrescriptionDispenseViewModel
    {
        public Patient? Patient { get; set; }
        public List<Models.Medication>? Medications { get; set; }
        public List<MedicationDispense> MedicationDispenseList { get; set; }
        public SelectList? Dispenser { get; set; }

        public async Task<PrescriptionDispenseViewModel> Init(DataContext _context, string baseApiUrl, Guid patientId)
        {
            Dispenser = new SelectList(_context.Practitioner
                .OrderBy(m => m.LastName)
                .Select(m => new { Id = m.Id, Name = $"{m.LastName} {m.FirstName}" }).ToList(), "Id", "Name");

            Medications = await GetMedications(baseApiUrl, patientId.ToString());

            var patient = await FHIRParser.GetPatientResource(patientId.ToString());
            if (patient != null)
            {
                Patient = patient;
            }

            return this;
        }

        private async Task<List<Medication>> GetMedications(string baseApiUrl, string patientId)
        {
            var medicationList = new List<Models.Medication>();

            var response = await HttpClientHelper.GetAsync($"{baseApiUrl}/fhir/MedicationRequest?subject=Patient/{patientId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();

                FhirJsonParser parser = new FhirJsonParser();

                var fhirBundle = parser.Parse<Hl7.Fhir.Model.Bundle>(jsonResult);

                foreach (var entry in fhirBundle.Entry)
                {
                    if (entry.Resource.TypeName == "MedicationRequest")
                    {
                        var medication = MedicationRequestTranslator.FromFhir(entry.Resource as MedicationRequest);
                        medicationList.Add(medication);
                    }
                }
            }

            return medicationList;
        }
    }
}
