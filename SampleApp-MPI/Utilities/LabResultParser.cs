using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath.Sprache;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.Xml;

namespace SampleApp_MPI.Utilities
{
    public class LabResultParser
    {
        public static async Task<string> AddVitalResource(Models.LabResult labResult)
        {
            var response = await HttpClientHelper.PostAsync("http://20.164.61.81:5001/fhir/Patient", ToFHIR(labResult));

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<Models.LabResult> GetVitalResource(string searchTerm)
        {
            var response = await HttpClientHelper.GetAsync($"http://20.164.61.81:5001/fhir/Patient/{searchTerm}");

            if (response.IsSuccessStatusCode)
            {
                var labResultJson = await response.Content.ReadAsStringAsync();
                FhirJsonParser parser = new FhirJsonParser();
                var fhirLabResult = parser.Parse<Hl7.Fhir.Model.Observation>(labResultJson);
                return FromFHIR(fhirLabResult);
            }
            return null;
        }

        public static Hl7.Fhir.Model.Observation ToFHIR(Models.LabResult labResult)
        {
            var fhirLabResult = new Hl7.Fhir.Model.Observation
            {
                Id = labResult.Id.ToString(),
                Text = new Narrative
                {
                    Status = Narrative.NarrativeStatus.Generated,
                    Div = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><p class=\"res-header-id\"><b>Generated Narrative: Observation SampleSzLabResult1</b></p><a name=\"SampleSzLabResult1\"> </a><a name=\"hcSampleSzLabResult1\"> </a><a name=\"SampleSzLabResult1-en-US\"> </a><p><b>Extention: Eswatini Lab Result Authorizer</b>: <a href=\"Practitioner-SampleSzPractitioner.html\">Practitioner Thabile Celiwe Dlamini </a></p><p><b>Extention: Eswatini Testing Laboratory</b>: <a href=\"Location-SampleSzLabLocation.html\">Location Mbabane Central Laboratory</a></p><p><b>status</b>: Final</p><p><b>code</b>: <span title=\"Codes:{http://localhost:3447/fhir/CodeSystem/SzTestParameterCodeCS PSCD}\">HIV Viral Load</span></p><p><b>subject</b>: <a href=\"Patient-SampleSzPatient.html\">Celucolo Celani Sacolo  Male, DoB: 2000-01-01 ( Medical Record Number: M001010101-1\u00a0(use:\u00a0usual,\u00a0))</a></p><p><b>encounter</b>: <a href=\"Encounter-SampleSzEncounter.html\">Encounter: status = finished; class = Out Patient Department (SzEncounterClassificationCS#OPD); reasonCode = Antiretrovial Therapy</a></p><p><b>performer</b>: <a href=\"Practitioner-SampleSzPractitioner.html\">Practitioner Thabile Celiwe Dlamini </a></p><p><b>value</b>: 16.5 Copies/ml</p><p><b>interpretation</b>: <span title=\"Codes:{http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation L}\">Low</span></p><p><b>specimen</b>: <a href=\"Specimen-SampleSzSpecimen.html\">Specimen: status = available; type = Blood</a></p></div>"
                },
                Extension = new List<Extension>
                {
                    new Extension
                    {
                        Url = "http://192.168.10.200:3447/fhir/StructureDefinition/SzAuthorizerExtension",
                        Value = new  ResourceReference
                        {
                            Reference = "Practitioner/SampleSzPractitioner"
                        }
                    }
                },
                Status = ObservationStatus.Final,
                Code = new CodeableConcept
                {

                    Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://localhost:3447/fhir/CodeSystem/SzTestParameterCodeCS",
                                Code ="PSCD",
                                Display ="HIV Viral Load"
                            }
                        },
                    Text = "HIV Viral Load"

                },
                Subject = new ResourceReference
                {
                    Reference = "Patient/SampleSzPatient"
                },
                Encounter = new ResourceReference
                {
                    Reference = "Encounter/SampleSzEncounter"
                },
                Performer = new List<ResourceReference>
               {
                      new ResourceReference
                       {
                           Reference = "Practitioner/SampleSzPractitioner"
                       }

               },
                Value = new Quantity
                {
                    Value = 16, //16.5,
                    Unit = "Copies/ml"
                },
                Interpretation = new List<CodeableConcept>
                {
                    new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System ="http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation",
                                Code ="L",
                                Display = "Low"
                            }
                        },
                        Text = "Low"
                    }

                },
                Specimen = new ResourceReference
                {
                    Reference = "Specimen/SampleSzSpecimen"
                }

            };

            return fhirLabResult;
        }


        public static Models.LabResult FromFHIR(Hl7.Fhir.Model.Observation labResult)
        {
            var entity = new Models.LabResult
            {
                Meta = labResult.AsEnumerable().AsQueryable().ToString(),
                Text = labResult.Text.ToString(),
                Status = labResult.Status.ToString(),
                Subject = labResult.Subject.ToString(),
                Encounter = labResult.Encounter.ToString(),
                Code = labResult.Code.ToString(),
                Interpretation = labResult.Interpretation.ToList().ToString(),
                Specimen = labResult.Specimen.ToString(),
                Performer = labResult.Performer.FirstOrDefault()?.Reference,

            };
            return entity;
        }
    }
}
