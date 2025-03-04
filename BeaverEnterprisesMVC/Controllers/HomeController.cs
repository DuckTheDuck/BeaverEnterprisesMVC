
using System.Diagnostics;
using BeaverEnterprisesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

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
            // Inicializa o contexto do banco de dados
            using (BeaverEnterprisesContext entities = new BeaverEnterprisesContext())
            {
                int? currentUser = HttpContext.Session.GetInt32("CurrentUserID");

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                List<Cart> listcard = entities.Carts
                    .Where(c => c.IdAccount == currentUser && c.Status == "por comprar")
                    .ToList();

                return View(listcard);
            }
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
        public async Task<IActionResult> BookingAvailability(string Origin, string Destination, string Departure, string Arrival, int Passengers)
        {
            try
            {
                // Validação dos parâmetros de entrada
                if (string.IsNullOrEmpty(Origin) || string.IsNullOrEmpty(Destination) || string.IsNullOrEmpty(Departure) || string.IsNullOrEmpty(Arrival) || Passengers <= 0)
                {
                    return BadRequest("Parâmetros de entrada inválidos.");
                }

                DateTime departureDate;
                DateTime arrivalDate;

                // Tentar fazer o parsing das datas
                if (!DateTime.TryParse(Departure, out departureDate) || !DateTime.TryParse(Arrival, out arrivalDate))
                {
                    return BadRequest("Formato de data inválido.");
                }

                ViewBag.Origin = Origin;
                ViewBag.Destination = Destination;
                ViewBag.Departure = Departure;
                ViewBag.Arrival = Arrival;
                ViewBag.Passengers = Passengers;

                // Converter para DateOnly (apenas a data, sem horário)
                DateOnly departureDateOnly = DateOnly.FromDateTime(departureDate);

                // Consulta na tabela Flightschedules
                var flightsFromScheduleTable = await _context.Flightschedules
                    .Include(fs => fs.IdFlightNavigation)
                    .ThenInclude(f => f.IdOriginNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdDestinationNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdAircraftNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdClassNavigation)
                    .Where(fs => fs.FlightDate == departureDateOnly && // Filtra apenas pelo dia da partida
                                 fs.IdFlightNavigation.IdOriginNavigation.Name == Origin &&
                                 fs.IdFlightNavigation.IdDestinationNavigation.Name == Destination)
                    .Select(fs => fs.IdFlightNavigation)
                    .ToListAsync();

                // Ordenar os voos pelo horário de partida (do mais cedo para o mais tarde)
                var allFlights = flightsFromScheduleTable
                    .OrderBy(f => f.DepartureTime) // Ordenar pelo horário de partida
                    .ToList();

                // Tratar os voos sem aeronave
                foreach (var flight in allFlights)
                {
                    if (flight.IdAircraftNavigation == null)
                    {
                        flight.IdAircraftNavigation = new Aircraft(); // Ou criar uma aeronave padrão
                    }
                }

                return View(allFlights);
            }
            catch (Exception ex)
            {
                // Log da exceção (pode ser substituído por um logger real)
                Console.Error.WriteLine($"Erro: {ex.Message}");
                return StatusCode(500, "Ocorreu um erro interno ao processar a solicitação.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BookingAvailabilityReturn(string Origin, string Destination, string Departure, string Arrival, int Passengers)
        {
            try
            {
                // Validação dos parâmetros de entrada
                if (string.IsNullOrEmpty(Origin) || string.IsNullOrEmpty(Destination) || string.IsNullOrEmpty(Departure) || string.IsNullOrEmpty(Arrival) || Passengers <= 0)
                {
                    return BadRequest("Parâmetros de entrada inválidos.");
                }

                // Tentar converter as datas
                if (!DateTime.TryParse(Departure, out DateTime departureDate) || !DateTime.TryParse(Arrival, out DateTime arrivalDate))
                {
                    return BadRequest("Formato de data inválido.");
                }

                ViewBag.Origin = Origin;
                ViewBag.Destination = Destination;
                ViewBag.Departure = Departure;
                ViewBag.Arrival = Arrival;
                ViewBag.Passengers = Passengers;

                // Converter para DateOnly (apenas a data, sem horário)
                DateOnly departureDateOnly = DateOnly.FromDateTime(departureDate);
                DateOnly arrivalDateOnly = DateOnly.FromDateTime(arrivalDate); // Data da volta
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                // Debug para verificar as datas utilizadas na consulta
                Console.WriteLine($"[DEBUG] DepartureDateOnly: {departureDateOnly}, ArrivalDateOnly: {arrivalDateOnly}, Today: {today}");

                // Buscar voos de VOLTA (Destino -> Origem) na data correta da volta
                var flightsFromScheduleTableVolta = await _context.Flightschedules
                    .Include(fs => fs.IdFlightNavigation)
                        .ThenInclude(f => f.IdOriginNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdDestinationNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdAircraftNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdClassNavigation)
                    .Where(fs => fs.FlightDate == arrivalDateOnly // Garante que pega apenas a data da volta
                                && fs.IdFlightNavigation.IdOriginNavigation.Name == Destination // Origem do voo de volta é o destino da ida
                                && fs.IdFlightNavigation.IdDestinationNavigation.Name == Origin // Destino do voo de volta é a origem da ida
                                && fs.FlightDate >= today) // Apenas voos futuros
                    .Select(fs => fs.IdFlightNavigation)
                    .ToListAsync();

                // Ordenar os voos pelo horário de partida
                var allFlightsVolta = flightsFromScheduleTableVolta
                    .OrderBy(f => f.DepartureTime)
                    .ToList();

                return View(allFlightsVolta);
            }
            catch (Exception ex)
            {
                // Log detalhado para capturar erros
                Console.Error.WriteLine($"[ERRO] {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, "Ocorreu um erro interno ao processar a solicitação.");
            }
        }
    }
}
