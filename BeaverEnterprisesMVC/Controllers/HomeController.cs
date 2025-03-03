
using System.Diagnostics;
using BeaverEnterprisesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeaverEnterprisesMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly BeaverEnterprisesContext _context;
    
        public HomeController(ILogger<HomeController> logger, BeaverEnterprisesContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult BookingAvailability()
        {
            return View();
        }
        public IActionResult BookingAvailabilityReturn()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult PassengerInformation()
        {
            return View();
        }
        public IActionResult CheckIn()
        {
            return View();
        }

        public IActionResult Regsister()
        {
            return View();
        }
        public IActionResult GetPartialView(string viewName)
        {
            return PartialView(viewName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
        [HttpPost]

        public IActionResult Login(string email, string password)
        {
            //if (email == "admin@gmail.com" && password == "admin")
            //{
            //    return RedirectToAction("Create", "Manufacturers");
            //}

            //var user = _context.Passengers.FirstOrDefault(u => u.Gmail == email && u.Password == password);
            //if (user != null)
            //{
            //    HttpContext.Session.SetInt32("CurrentUserID", user.Id);
            //    return RedirectToAction("Index", "Home");
            //}

            ViewBag.Error = "Invalid username or password.";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BookingAvailability(string Origin, string Destination, string Departure, string Arrival)
        {
            ViewBag.Origin = Origin;
            ViewBag.Destination = Destination;
            ViewBag.Departure = Departure;
            ViewBag.Arrival = Arrival;

            var flights = await _context.Flights
           .Include(f => f.IdOriginNavigation)
           .Include(f => f.IdDestinationNavigation)
           .Include(f => f.IdAircraftNavigation)
           .Include(f => f.IdClassNavigation)
           .ToListAsync();

            return View(flights);
        }

        [HttpGet]
        public async Task<IActionResult> BookingAvailabilityReturn(string Origin, string Destination, string Departure, string Arrival)
        {
            ViewBag.Origin = Origin;
            ViewBag.Destination = Destination;
            ViewBag.Departure = Departure;
            ViewBag.Arrival = Arrival;

            var flights = await _context.Flights
           .Include(f => f.IdOriginNavigation)
           .Include(f => f.IdDestinationNavigation)
           .Include(f => f.IdAircraftNavigation)
           .Include(f => f.IdClassNavigation)
           .ToListAsync();

            return View(flights);
        }
    }
}
