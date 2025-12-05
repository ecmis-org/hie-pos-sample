using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SampleApp_MPI.Utilities
{
    public static class HttpClientHelper
    {
        public static async Task<HttpResponseMessage> PostAsync(string url, Resource resource)
        {
            using (var httpClient = CreateHttpClient())
            {
                var content = CreateContent(resource);

                return await httpClient.PostAsync(url, content);
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(string url)
        {
            using (var httpClient = CreateHttpClient())
            {
                return await httpClient.GetAsync(url);
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Custom", "test");

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        private static StringContent CreateContent(Resource resource)
        {
            var options = new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector).Pretty();
            var patientResourceJson = JsonSerializer.Serialize(resource, options);
            var stringContent = new StringContent(patientResourceJson);

            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/fhir+json");
            stringContent.Headers.ContentLength = Encoding.UTF8.GetByteCount(patientResourceJson);

            return stringContent;
        }
    }
}
