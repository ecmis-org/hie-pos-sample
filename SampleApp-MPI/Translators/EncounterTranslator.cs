using Hl7.Fhir.Model;

namespace SampleApp_MPI.Translators
{
    public static class EncounterTranslator
    {
        public static Encounter ToFhir(Models.Encounter entity, Guid locationId)
        {
            var encounter = new Encounter
            {
                Id = entity.Id.ToString(),
                Meta = new Meta
                {
                    Profile = new List<string>
                    {
                        "http://192.168.10.200:3447/fhir/StructureDefinition/SzEncounter"
                    }
                },
                Status = Encounter.EncounterStatus.Finished,
                Class = new Coding
                {
                    System = "http://localhost:3447/fhir/CodeSystem/SzEncounterClassificationCS",
                    Code = entity.Department.Code,
                    Display = entity.Department.Name
                },
                ReasonCode = new List<CodeableConcept>
                {
                    new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://localhost:3447/fhir/CodeSystem/SzServicePointCS",
                                Code = entity.ServicePoint.Code,
                                Display = entity.ServicePoint.Name
                            }
                        },
                        Text = entity.ServicePoint.Name
                    }
                },
                Location = new List<Encounter.LocationComponent>
                {
                    new Encounter.LocationComponent
                    {
                        Location = new ResourceReference
                        {
                            Reference = $"{nameof(Location)}/{locationId}"
                        },
                        Status = Encounter.EncounterLocationStatus.Completed
                    }
                }
            };

            return encounter;
        }
    }
}
