using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SampleApp_MPI.Models;
using SampleApp_MPI.Models.ViewModels;
using SampleApp_MPI.Services;
using SampleApp_MPI.Utilities;
using System.Diagnostics;
using System.Security.Cryptography;

namespace SampleApp_MPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration config, ILogger<HomeController> logger)
        {
            _config = config;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new Search());
        }

        public IActionResult Create()
        {
            return View(new Patient());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Patient patient)
        {

            if (ModelState.IsValid)
            {
                patient.PatientId = Guid.NewGuid();
                patient.AlternateId = $"H001{new Random().Next(101010, 999999)}-{new Random().Next(1, 9)}";

                var response = await PatientService.Create(_config.GetValue<string>("ApiBaseUrl")!, patient);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                _logger.LogDebug("Error creating patient: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
            }

            return View(patient);
        }

        public async Task<IActionResult> Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(ClientSearchViewModel vm)
        {
            if(ModelState.IsValid)
            {
                var baseUrl = _config.GetValue<string>("ApiBaseUrl");
                var searchTerm = vm.SearchTerm.Trim();

                string url;

                if (searchTerm.Any(char.IsDigit))
                {
                    url = $"{baseUrl}/Patient?identifier={searchTerm}";
                }
                else
                {
                    url = $"{baseUrl}/Patient?given={searchTerm}";
                }

                var result = await PatientService.Search(url);

                if (result.Count > 0)
                    vm.SearchResults = result;
                else
                    ModelState.AddModelError(string.Empty, "No records found matching the search criteria.");
            }

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}