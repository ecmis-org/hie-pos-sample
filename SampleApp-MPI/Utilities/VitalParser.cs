using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath.Sprache;
using System;


namespace SampleApp_MPI.Utilities
{
    public class VitalParser
    {
        public static async Task<string> AddVitalResource(Models.Vital vital)
        {
            var response = await HttpClientHelper.PostAsync("http://20.164.61.81:5001/fhir/Patient", ToFHIR(vital));

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<Models.Vital> GetVitalResource(string searchTerm)
        {
            var response = await HttpClientHelper.GetAsync("$http://20.164.61.81:5001/fhir/Patient/{searchTerm}");

            if (response.IsSuccessStatusCode)
            {
                var vitalJson = await response.Content.ReadAsStringAsync();
                FhirJsonParser parser = new FhirJsonParser();
                var fhirVital = parser.Parse<Hl7.Fhir.Model.Observation>(vitalJson);
                return FromFHIR(fhirVital);
            }
            return null;
        }
        public static Hl7.Fhir.Model.Observation ToFHIR(Models.Vital vital)
        {
            var entity = new Hl7.Fhir.Model.Observation
            {
                Id = vital.Id.ToString(),
                //Meta =
                //{
                //    Profile = "http://192.168.10.200:3447/fhir/StructureDefinition/SzVitalSigns"
                //},
                Status = ObservationStatus.Final,

                Category = new List<CodeableConcept>
                {
                    new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                           new Coding
                           {
                               System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "vital-signs"
                           }
                        }
                    }
                },
                Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://hl7.org/fhir/ValueSet/observation-vitalsignresult",
                            Code = "8302-2",
                            Display ="Body height"
                        }
                    },
                    Text = "Body height"
                },
                Subject = new ResourceReference
                {
                    Reference = "Patient/SampleSzPatient"
                },
                Effective = new FhirDateTime(""),
                Value = new Quantity
                {
                    Value = 175,
                    Unit = "cm"
                }

            };
            return entity;
        }


        public static Models.Vital FromFHIR(Hl7.Fhir.Model.Observation vital)
        {
            var entity = new Models.Vital
            {
                //Meta = vital.Meta?.FirstOrDefault(),
            };

            return entity;
        }



    }
}

