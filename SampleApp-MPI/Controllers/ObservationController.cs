using Microsoft.AspNetCore.Mvc;
using SampleApp_MPI.Models;

namespace SampleApp_MPI.Controllers
{
    public class ObservationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateLabResult()
        {

            return View();
        }
        [HttpPost]
        public IActionResult CreateLabResult(Vital vital)
        {
            if (ModelState.IsValid)
            {
                vital.Id = Guid.NewGuid();
            }
            return View();
        }
    }
}
