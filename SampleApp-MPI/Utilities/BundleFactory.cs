using Hl7.Fhir.Model;

namespace SampleApp_MPI.Utilities;

public static class BundleFactory
{
    public static Bundle CreateBundle<T>(List<T> resourceList) where T : Resource
    {
        var bundle = new Bundle 
        {
            Id = Guid.NewGuid().ToString(),
            Type = Bundle.BundleType.Transaction,
        };

        bundle.Entry = new List<Bundle.EntryComponent>();
        foreach (var item in resourceList) 
        {
            var entry = new Bundle.EntryComponent
            {
                FullUrl = $"{item.TypeName}/{item.Id}",
                Resource = item,
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.PUT,
                    Url = $"{item.TypeName}/{item.Id}"
                }
            };

            bundle.Entry.Add(entry);
        }

        return bundle;
    }
}
