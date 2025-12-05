using Hl7.Fhir.Model;
using SampleApp_MPI.Models;

namespace SampleApp_MPI.Translators
{
    public static class MedicationRequestTranslator
    {
        public static MedicationRequest ToFhir(Models.Medication entity) 
        {
            var fhirMedicationRequest = new MedicationRequest
            {
                Id = entity.Id.ToString(),
                Meta = new Meta 
                {
                    Profile = new List<string> 
                    {
                        "http://192.168.10.200:3447/fhir/StructureDefinition/SzMedicationRequest"
                    }
                },
                Status = MedicationRequest.MedicationrequestStatus.Active,
                Intent = MedicationRequest.MedicationRequestIntent.Order,
                Medication = new CodeableConcept 
                {
                    Coding = new List<Coding> 
                    {
                        new Coding 
                        {
                            System = "http://localhost:3447/fhir/CodeSystem/SzProductCodeCS",
                            Code  = entity.Product.Code,
                            Display = entity.Product.Name
                        }
                    },
                    Text = entity.Product.Name
                },
                DosageInstruction = new List<Dosage> 
                {
                    new Dosage
                    {
                        Text = $"Take {entity.Quantity} {entity.Product.Form} {entity.Frequency}",
                        Timing = new Timing 
                        {
                            Repeat = new Timing.RepeatComponent
                            {
                                Frequency = (int)entity.Frequency,
                                Period = entity.Duration,  
                                PeriodUnit = Timing.UnitsOfTime.D
                            }
                        },
                        DoseAndRate = new List<Dosage.DoseAndRateComponent>
                        {
                            new Dosage.DoseAndRateComponent
                            {
                                Dose = new Quantity
                                {
                                    Value = entity.Quantity,
                                    Unit = entity.Product.Form.ToString()
                                }
                            }
                        }
                    }
                },
                AuthoredOn = DateTime.Now.ToString("yyyy-MM-dd"),
                Subject = new ResourceReference 
                {
                    Reference = $"Patient/{entity.Prescription.PatientId}"
                },
                Encounter = new ResourceReference 
                {
                    Reference = $"Encounter/{entity.Prescription.EncounterId}"
                },
                Requester = new ResourceReference
                {
                    Reference = $"Practitioner/{entity.Prescription.PractitionerId}"
                }
            };

            return fhirMedicationRequest;
        }

        public static Models.Medication FromFhir(MedicationRequest fhirMedicationRequest) 
        {
            var dosageInstructions = fhirMedicationRequest
                .DosageInstruction
                .FirstOrDefault();

            var medicationCoding = (fhirMedicationRequest.Medication as CodeableConcept)?
                .Coding
                .FirstOrDefault();

            var doseAndRateComponent = dosageInstructions?
                .DoseAndRate
                .FirstOrDefault();

            var entity = new Models.Medication
            {
                Product = new Product
                {
                    Code = medicationCoding.Code,
                    Name = medicationCoding.Display
                },
                Id = Guid.Parse(fhirMedicationRequest.Id),
                Frequency = (Frequency)(dosageInstructions.Timing.Repeat.Frequency ?? 0),
                Duration = (int)(dosageInstructions.Timing.Repeat.Period ?? 0), //To Do: Pull this from duration
                Quantity = (int)(doseAndRateComponent.Dose as Quantity).Value,
            };

            return entity;
        }
    }
}