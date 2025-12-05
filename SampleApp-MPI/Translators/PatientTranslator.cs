using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using SampleApp_MPI.Utilities;
using static SampleApp_MPI.Utilities.Constants;

namespace SampleApp_MPI.Translators;

public static class PatientTranslator
{
    public static Patient ToFhir(this Models.Patient entity)
    {
        return new Patient
        {
            Id = entity.PatientId.ToString(),
            Meta = new Meta
            {
                Profile = new string[] { "http://localhost:8080/StructureDefinition/SzPatient" }
            },
            Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Family = entity.LastName,
                        Given = new List<string>
                        {
                            entity.FirstName ?? string.Empty, entity.MiddleName ?? string.Empty
                        }
                    }
                },
            Gender = entity.Sex.ToFhirGender(),
            BirthDate = entity.DOB.ToFhirDate(),
            Extension = new List<Extension>
                {
                    new Extension
                    {
                        Url = "Code",
                        Value = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://192.168.10.200:3447/fhir/CodeSystem/SzTinkhundlaCS",
                                    Code = entity.Inkhundla,
                                    Display = entity.Inkhundla,
                                }
                            },
                            Text = entity.Inkhundla
                        }
                    },
                    new Extension
                    {
                        Url = "Code",
                        Value = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://192.168.10.200:3447/fhir/ValueSet/SzChiefdomVS",
                                    Code = entity.Chiefdom,
                                    Display = entity.Chiefdom,
                                }
                            },
                            Text = entity.Chiefdom
                        }
                    }
                },
            Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        Use = Identifier.IdentifierUse.Official,
                        System = "urn:homeaffairs:population-register",
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
                    },
                    new Identifier
                    {
                        Use = Identifier.IdentifierUse.Usual,
                        System = "urn:health:cmis",
                        Value = entity.AlternateId,
                        Type = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://192.168.10.200:3447/fhir/CodeSystem/SzPersonIdentificationsCS",
                                    Code = "MR",
                                    Display = "Medical Record Number"
                                }
                            }
                        }

                    }
                }
        };

    }

    public static Models.Patient ToEmrType(this Patient fhirPatient)
    {
        var entity = new Models.Patient
        {
            PatientId = Guid.Parse(fhirPatient.Id),

            FirstName = fhirPatient.Name
                .FirstOrDefault()!
                .Given
                .FirstOrDefault()!,

            MiddleName = fhirPatient.Name
                .FirstOrDefault()
                .Given
                .LastOrDefault(),

            LastName = fhirPatient.Name
                .FirstOrDefault()!
                .Family,
            Sex = fhirPatient.Gender.Value.Equals(AdministrativeGender.Female)
                ? Sex.female : fhirPatient.Gender.Value.Equals(AdministrativeGender.Male)
                ? Sex.male : Sex.other,

            DOB = DateTime.Parse(fhirPatient.BirthDate),

            PIN = fhirPatient.Identifier
                .FirstOrDefault(a => a.System == "urn:homeaffairs:population-register")?.Value,

            AlternateId = fhirPatient.Identifier
                .FirstOrDefault(a => a.System.Contains("urn:health"))?.Value,
        };

        return entity;
    }
}
