using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using SampleApp_MPI.Models;
using static SampleApp_MPI.Utilities.Constants;

namespace SampleApp_MPI.Utilities
{
    public class FHIRParser
    {
        public static async Task<string> AddPatientResource(Models.Patient patient)
        {
            var response = await HttpClientHelper.PostAsync("http://172.209.216.154:5001/fhir/Patient", ToFHIR(patient));

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<Models.Patient> GetPatientResource(string searchTerm)
        {
            var response = await HttpClientHelper.GetAsync($"http://172.209.216.154:5001/fhir/Patient/{searchTerm}");

            if (response.IsSuccessStatusCode)
            {
                var patientJson = await response.Content.ReadAsStringAsync();

                FhirJsonParser parser = new FhirJsonParser();
                var fhirPatient = parser.Parse<Hl7.Fhir.Model.Patient>(patientJson);

                return FromFHIR(fhirPatient);
            }

            return null;
        }

        public static Hl7.Fhir.Model.Patient ToFHIR(Models.Patient patient)
        {
            var fhirPatient = new Hl7.Fhir.Model.Patient
            {
                Id = patient.PatientId.ToString(),
                Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Family = patient.LastName,
                        Given = new List<string>
                        {
                            patient.FirstName ?? string.Empty, patient.MiddleName ?? string.Empty
                        }
                    }
                },
                Gender = setGender(patient.Sex),
                BirthDate = patient.DOB.ToString("yyyy-MM-dd"),
                Extension = new List<Extension>
                {
                    new Extension
                    {
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
                                            System = "urn:iso:std:iso:3166",
                                            Code = "SWZ",
                                            Display = "Eswatini"
                                        }
                                    }
                                }
                            }
                        },
                        Url = "http://hl7.org/fhir/StructureDefinition/patient-nationality"
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
                                    System = "http://192.168.10.200:3447/fhir/CodeSystem/SzTinkhundlaCS",
                                    Code = patient.Inkhundla,
                                    Display = patient.Inkhundla,
                                }
                            },
                            Text = patient.Inkhundla
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
                                    Code = patient.Chiefdom,
                                    Display = patient.Chiefdom,
                                }
                            },
                            Text = patient.Chiefdom
                        }
                    }
                },
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        Use = Identifier.IdentifierUse.Official,
                        System = "http://homeaffairs.sys",
                        Value = patient.PIN,
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
                        System = "http://mfl.sys/m001",
                        Value = patient.AlternateId,
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

            return fhirPatient;
        }

        public static Models.Patient FromFHIR(Hl7.Fhir.Model.Patient patient)
        {
            var entity = new Models.Patient
            {
                PatientId = Guid.Parse(patient.Id),
                FirstName = patient.Name.FirstOrDefault().Given.FirstOrDefault(),
                MiddleName = patient.Name.FirstOrDefault().Given.LastOrDefault(),
                LastName = patient.Name.FirstOrDefault().Family,
                Sex = patient.Gender.Value.Equals(AdministrativeGender.Female) ? Sex.female : patient.Gender.Value.Equals(AdministrativeGender.Male) ? Sex.male : Sex.other,
                DOB = DateTime.Parse(patient.BirthDate),
                PIN = patient.Identifier.FirstOrDefault(a => a.System == "http://homeaffairs.sys")?.Value,
                AlternateId = patient.Identifier.FirstOrDefault(a => a.System == "http://mfl.sys/m001")?.Value,
            };

            return entity;
        }

        public static AdministrativeGender setGender(Sex sex)
        {
            if (sex == Sex.male)
                return AdministrativeGender.Male;
            else if (sex == Sex.female)
                return AdministrativeGender.Female;
            else return AdministrativeGender.Other;
        }
    }
}
