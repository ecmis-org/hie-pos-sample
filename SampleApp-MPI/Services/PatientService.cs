using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using SampleApp_MPI.Translators;
using SampleApp_MPI.Utilities;

namespace SampleApp_MPI.Services;

public class PatientService
{
    public static async Task<HttpResponseMessage> Create(string url, Models.Patient patient)
    {
        var entries = new List<Resource> { patient.ToFhir() };

        var response = await HttpClientHelper
            .PostAsync(url, BundleFactory.CreateBundle(entries));

        return response;
    }

    public static async Task<List<Models.Patient>> Search(string url)
    {
        var response = await HttpClientHelper.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var jsonResult = await response.Content.ReadAsStringAsync();

            var bundle = new FhirJsonParser().Parse<Bundle>(jsonResult);

            if (bundle?.Total > 0)
            {
                List<Models.Patient> patients = new List<Models.Patient>();

                foreach (var entry in bundle.Entry)
                {
                    if (entry.Resource is Patient fhirPatient)
                    {
                        patients.Add(fhirPatient.ToEmrType());
                    }
                }

                return patients;
            }
        }

        return null;
    }
}
