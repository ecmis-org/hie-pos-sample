using Hl7.Fhir.Model;
using SampleApp_MPI.Models;
using static SampleApp_MPI.Utilities.Constants;

namespace SampleApp_MPI.Utilities
{
    public static class Extensions
    {
        public static AdministrativeGender ToFhirGender(this Sex sex)
        {
            switch (sex)
            {
                case Sex.male: return AdministrativeGender.Male;
                case Sex.female: return AdministrativeGender.Female;
                default: return AdministrativeGender.Other;
            }
        }
    }
}
