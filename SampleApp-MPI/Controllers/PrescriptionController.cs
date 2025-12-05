using Hl7.Fhir.Model;
using Hl7.Fhir.Model.CdsHooks;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SampleApp_MPI.DAL;
using SampleApp_MPI.Models;
using SampleApp_MPI.Models.ViewModels;
using SampleApp_MPI.Translators;
using SampleApp_MPI.Utilities;
using System.Reflection.Metadata;

namespace SampleApp_MPI.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly string API_BASE_URL;
        private readonly DataContext _context;

        public PrescriptionController(IConfiguration config, DataContext context)
        {
            API_BASE_URL = config.GetSection("ApiBaseUrl").Value;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Prescribe()
        {
            return View(await new PrescriptionViewModel().Init(_context, "1af5f264-52cc-4fcc-8a01-353fe94bd43a"));
        }

        [HttpPost]
        public async Task<IActionResult> Prescribe(PrescriptionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //Save Prescription Data Locally
                vm.Prescription.Id = Guid.NewGuid();
                var result = SaveData(vm);

                if (result > 0)
                {
                    //Create and Push Fhir Bundle to HIE
                    var bundle = CreateFhirBundle(vm.Prescription.Id);

                    var httpResult = await HttpClientHelper.PostAsync($"{API_BASE_URL}/fhir", bundle);

                    if (httpResult.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Details), new { patientId = vm.Prescription.PatientId });
                    }
                }
            }

            return View(await vm.Init(_context, vm.Patient.PatientId.ToString()));
        }

        public async Task<IActionResult> Dispense(Guid patientId)
        {
            return View(await new PrescriptionDispenseViewModel().Init(_context, API_BASE_URL, patientId));
        }

        [HttpPost]
        public async Task<IActionResult> Dispense(PrescriptionDispenseViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var location = _context.Location.FirstOrDefault();

                var fhirBundle = new Bundle
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = Bundle.BundleType.Transaction
                };

                foreach (var item in vm.MedicationDispenseList)
                {
                    item.Id = Guid.NewGuid();
                    var bundleEntry = new Bundle.EntryComponent
                    {
                        FullUrl = $"MedicationDispense/{item.Id}",
                        Resource = MedicationDispenseTranslator.ToFhir(item, location.Id),
                        Request = new Bundle.RequestComponent
                        {
                            Method = Bundle.HTTPVerb.PUT,
                            Url = $"MedicationDispense/{item.Id}"
                        }
                    };
                    fhirBundle.Entry.Add(bundleEntry);
                }

                var httpResult = await HttpClientHelper.PostAsync($"{API_BASE_URL}/fhir", fhirBundle);

                if (httpResult.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Prescribe)); //TO DO: Redirect to index
                }
            }

            return View(await vm.Init(_context, API_BASE_URL, vm.Patient.PatientId));
        }

        public async Task<IActionResult> Details(Guid patientId)
        {
            var patient = await FHIRParser.GetPatientResource(patientId.ToString());

            var response = await HttpClientHelper.GetAsync($"{API_BASE_URL}/fhir/MedicationRequest?subject=Patient/{patientId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();

                FhirJsonParser parser = new FhirJsonParser();

                var fhirBundle = parser.Parse<Hl7.Fhir.Model.Bundle>(jsonResult);

                var medicationList = new List<Models.Medication>();
                foreach (var entry in fhirBundle.Entry)
                {
                    if (entry.Resource.TypeName == "MedicationRequest")
                    {
                        var medication = MedicationRequestTranslator.FromFhir(entry.Resource as MedicationRequest);
                        medicationList.Add(medication);
                    }
                }

                return View(new PrescriptionDetailsViewModel
                {
                    Patient = patient,
                    Medications = medicationList
                });
            }

            return RedirectToAction(nameof(Index));
        }

        private int SaveData(PrescriptionViewModel vm)
        {
            vm.Encounter.Id = Guid.NewGuid();

            vm.Prescription.EncounterId = vm.Encounter.Id;

            vm.Medication.Id = Guid.NewGuid();
            vm.Medication.PrescriptionId = vm.Prescription.Id;

            _context.Prescription.Add(vm.Prescription);
            _context.Encounters.Add(vm.Encounter);
            _context.Medication.Add(vm.Medication);

            return _context.SaveChanges();
        }

        private Bundle CreateFhirBundle(Guid prescriptionId)
        {
            var medications = _context.Medication
                    .Include(x => x.Prescription.Practitioner)
                    .Include(x => x.Prescription.Patient)
                    .Include(x => x.Prescription.Encounter.ServicePoint)
                    .Include(x => x.Prescription.Encounter.Department)
                    .Include(x => x.Product)
                    .Where(m => m.PrescriptionId == prescriptionId)
                    .ToList();

            var prescription = medications.FirstOrDefault()?.Prescription;

            var location = _context.Location.FirstOrDefault();

            var fhirBundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Type = Bundle.BundleType.Transaction,
                Entry = new List<Bundle.EntryComponent>
                {
                    new Bundle.EntryComponent
                    {
                        FullUrl = $"Patient/{prescription.PatientId}",
                        Resource = FHIRParser.ToFHIR(prescription.Patient),
                        Request = new Bundle.RequestComponent
                        {
                            Method = Bundle.HTTPVerb.PUT,
                            Url = $"{nameof(Hl7.Fhir.Model.Patient)}/{prescription.PatientId}"
                        }
                    },
                    new Bundle.EntryComponent
                    {
                        FullUrl = $"{nameof(Hl7.Fhir.Model.Practitioner)}/{prescription?.PractitionerId}",
                        Resource = PractitionerTranslator.ToFhir(prescription.Practitioner),
                        Request = new Bundle.RequestComponent
                        {
                            Method = Bundle.HTTPVerb.PUT,
                            Url = $"{nameof(Hl7.Fhir.Model.Practitioner)}/{prescription.PractitionerId}"
                        }
                    },
                    new Bundle.EntryComponent
                    {
                        FullUrl = $"{nameof(Hl7.Fhir.Model.Location)}/{location.Id}",
                        Resource = LocationTranslator.ToFhir(location),
                        Request = new Bundle.RequestComponent
                        {
                            Method = Bundle.HTTPVerb.PUT,
                            Url = $"{nameof(Hl7.Fhir.Model.Location)}/{location.Id}"
                        }
                    },
                    new Bundle.EntryComponent
                    {
                        FullUrl = $"{nameof(Hl7.Fhir.Model.Encounter)}/{prescription.EncounterId}",
                        Resource = EncounterTranslator.ToFhir(prescription.Encounter,location.Id),
                        Request = new Bundle.RequestComponent
                        {
                            Method = Bundle.HTTPVerb.PUT,
                            Url = $"{nameof(Hl7.Fhir.Model.Encounter)}/{prescription.EncounterId}"
                        }
                    }
                }
            };

            foreach (var medication in medications)
            {
                var entry = new Bundle.EntryComponent
                {
                    FullUrl = $"{nameof(Hl7.Fhir.Model.MedicationRequest)}/{medication.Id}",
                    Resource = MedicationRequestTranslator.ToFhir(medication),
                    Request = new Bundle.RequestComponent
                    {
                        Method = Bundle.HTTPVerb.PUT,
                        Url = $"{nameof(Hl7.Fhir.Model.MedicationRequest)}/{medication.Id}",
                    }
                };

                fhirBundle.Entry.Add(entry);
            }

            return fhirBundle;
        }
    }
}
