using Hl7.Fhir.Model;
using Hl7.Fhir.Support;

namespace SampleApp_MPI.Translators
{
    public static class MedicationDispenseTranslator
    {
        public static MedicationDispense ToFhir(Models.MedicationDispense entity, Guid locationId) 
        {
            var fhirMedicationDispense = new MedicationDispense
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta
                {
                    Profile = new List<string>
                    {
                        "http://192.168.10.200:3447/fhir/StructureDefinition/SzMedicationDispense"
                    }
                },
                Status = MedicationDispense.MedicationDispenseStatusCodes.Completed,
                Medication = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://localhost:3447/fhir/CodeSystem/SzProductCodeCS",
                            Code  = entity.Medication.Product.Code,
                            Display = entity.Medication.Product.Name
                        }
                    },
                    Text = entity.Medication.Product.Name
                },
                Quantity = new Quantity 
                {
                    Value = entity.QuantityDispensed,
                    Unit = entity.Medication.Product.Form.ToString()
                },
                DaysSupply = new Quantity 
                {
                    Value = entity.DaysSupply,
                    Unit = "days"
                },
                WhenPrepared = entity.DatePrepared.ToFhirDate(),
                WhenHandedOver = entity.DateDispensed.ToFhirDate(),
                DosageInstruction = new List<Dosage>
                {
                    new Dosage
                    {
                        Text = $"Take {entity.Medication.Quantity} {entity.Medication.Product.Form} {entity.Medication.Frequency}",
                        Timing = new Timing
                        {
                            Repeat = new Timing.RepeatComponent
                            {
                                Frequency = (int)entity.Medication.Frequency, //TO DO: Change based on Prescription
                                Period = 1, //TO DO:  Change based on Prescritipon 
                                PeriodUnit = Timing.UnitsOfTime.D
                            }
                        },
                        DoseAndRate = new List<Dosage.DoseAndRateComponent>
                        {
                            new Dosage.DoseAndRateComponent
                            {
                                Dose = new Quantity
                                {
                                    Value = entity.Medication.Quantity,
                                    Unit = entity.Medication.Product.Form.ToString()
                                }
                            }
                        }
                    }
                },
                Subject = new ResourceReference
                {
                    Reference = $"Patient/{entity.Medication.Prescription.PatientId}"
                },
                Performer = new List<MedicationDispense.PerformerComponent> 
                {
                    new MedicationDispense.PerformerComponent
                    {
                        Actor = new ResourceReference
                        {
                            Reference = $"Practitioner/{entity.DispenserId}"
                        }
                    }
                },
                Location = new ResourceReference 
                {
                    Reference = $"Location/{locationId}"
                },
                AuthorizingPrescription = new List<ResourceReference> 
                {
                    new ResourceReference 
                    {
                        Reference = $"MedicationRequest/{entity.MedicationId}"
                    }
                }
            };

            return fhirMedicationDispense;
        }

        public static Models.MedicationDispense FromFhir(MedicationDispense fhirMedicationDispense) 
        {
            var medicationRequest = fhirMedicationDispense.AuthorizingPrescription
                .FirstOrDefault();

            var actor = fhirMedicationDispense.Performer
                .FirstOrDefault()
                .Actor;

            var entity = new Models.MedicationDispense
            {
                Id = Guid.Parse(fhirMedicationDispense.Id),
                MedicationId = Guid.Parse(medicationRequest.Reference),
                DispenserId = Guid.Parse(actor.Reference),
                DatePrepared = DateTime.Parse(fhirMedicationDispense.WhenPrepared),
                DateDispensed = DateTime.Parse(fhirMedicationDispense.WhenHandedOver),
                QuantityDispensed = (int)(fhirMedicationDispense.Quantity.Value ?? 0),
                DaysSupply = (int)(fhirMedicationDispense.DaysSupply.Value ?? 0)
            };

            return entity;
        }
    }
}
