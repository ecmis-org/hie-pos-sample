using Hl7.Fhir.Model;

namespace SampleApp_MPI.Translators
{
    public static class LocationTranslator
    {
        public static Location ToFhir(Models.Location entity) 
        {
            var fhirLocation = new Location
            {
                Id = entity.Id.ToString(),
                Meta = new Meta
                {
                    Profile = new List<string>
                    {
                        "http://192.168.10.200:3447/fhir/StructureDefinition/SzLocation"
                    }
                },
                Status = Location.LocationStatus.Active,
                Name = entity.Name,
                Type = new List<CodeableConcept>
                {
                    new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
                                Code = entity.Code,
                                Display = "Outpatient Facility"
                            }
                        }
                    }
                },
                Telecom = new List<ContactPoint> 
                { 
                    new ContactPoint 
                    {
                        System = ContactPoint.ContactPointSystem.Phone,
                        Value = entity.Tell
                    }
                }
            };

            return fhirLocation;
        }
    }
}
