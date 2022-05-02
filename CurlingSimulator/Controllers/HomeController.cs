using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CurlingSimulator.Interfaces;
using CurlingSimulator.Models.ViewModels;

namespace CurlingSimulator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISimulator _curlingSimulator;

        public HomeController(ILogger<HomeController> logger, ISimulator curlingSimulator)
        {
            _logger = logger;
            _curlingSimulator = curlingSimulator;
        }

        public IActionResult Index()
        {
            var model = new SimulatorViewModel {
                Input = new SimulatorInput {
                    NumDisks = 1,
                    DiskRadius = 1
                },
                Result = new SimulatorResult()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SimulatorInput input)
        {
            var model = new SimulatorViewModel { Input = input, Result = new SimulatorResult() };
            if (ModelState.IsValid)
            {
                var startPositions = Array.ConvertAll(input.XCoordinates.Split(","), Convert.ToInt32);

                var result = _curlingSimulator.Simulate(input.DiskRadius, startPositions);

                model.Result.Disks = result;
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
