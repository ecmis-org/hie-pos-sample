using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using SampleApp_MPI.Utilities;

namespace SampleApp_MPI.Translators
{
    public static class PractitionerTranslator
    {
        public static Practitioner ToFhir(Models.Practitioner entity) 
        {
            var fhirPractitioner = new Practitioner 
            {
                Id = entity.Id.ToString(),
                Meta = new Meta
                {
                    Profile = new List<string>
                    {
                        "http://192.168.10.200:3447/fhir/StructureDefinition/SzPractitioner"
                    }
                },
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        Use = Identifier.IdentifierUse.Official,
                        System = "http://homeaffairs.sys",
                        Value = entity.PIN,
                        Type = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://192.168.10.200:3447/fhir/CodeSystem/SzPersonIdentificationsCS",
                                    Code = "PI",
                                    Display = "Personal ID Number"
                                }
                            }
                        }

                    }
                },
                Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Family = entity.LastName,
                        Given = new List<string>
                        {
                            entity.FirstName
                        }
                    }
                },
                Gender = entity.Sex.ToFhirGender(),
                BirthDate = entity.DOB.ToFhirDate()
            };

            return fhirPractitioner;
        }
    }
}
