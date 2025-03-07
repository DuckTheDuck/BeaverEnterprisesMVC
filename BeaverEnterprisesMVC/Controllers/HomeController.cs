
using System.Diagnostics;
using BeaverEnterprisesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Newtonsoft.Json;

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

        [HttpGet]
        public IActionResult Cart()
        {
            try
            {
                // Retrieve tickets and passenger count from session
                var passengerCount = HttpContext.Session.GetInt32("PassengersCount") ?? 0;
                var ticketsJson = HttpContext.Session.GetString("CartTickets");
                var tickets = string.IsNullOrEmpty(ticketsJson)
                    ? new List<Ticket>()
                    : JsonConvert.DeserializeObject<List<Ticket>>(ticketsJson);

                if (!tickets.Any() || passengerCount == 0)
                {
                    return View(new List<Ticket>());
                }

                // Populate flight schedule navigation properties
                foreach (var ticket in tickets)
                {
                    if (ticket.IdFlightSchedule != 0)
                    {
                        ticket.IdFlightScheduleNavigation = _context.Flightschedules
                            .Include(fs => fs.IdFlightNavigation)
                            .ThenInclude(f => f.IdOriginNavigation)
                            .Include(fs => fs.IdFlightNavigation.IdDestinationNavigation)
                            .Include(fs => fs.IdFlightNavigation.IdAircraftNavigation)
                            .Include(fs => fs.IdFlightNavigation.IdClassNavigation)
                            .FirstOrDefault(fs => fs.Id == ticket.IdFlightSchedule);
                    }
                }

                // Calculate the number of tickets per passenger (assuming equal ida and volta)
                int ticketsPerPassenger = tickets.Count() / passengerCount;
                if (ticketsPerPassenger % 2 != 0)
                {
                    throw new Exception("Number of tickets must be even for pairing departure and arrival.");
                }

                // Group tickets by passenger
                List<List<Ticket>> passengerTickets = new List<List<Ticket>>();
                for (int i = 0; i < passengerCount; i++)
                {
                    int startIndex = i * ticketsPerPassenger;
                    int endIndex = startIndex + ticketsPerPassenger;
                    var passengerGroup = tickets.GetRange(startIndex, ticketsPerPassenger);
                    passengerTickets.Add(passengerGroup);
                }

                // Assign temporary IdPassager to each group
                for (int i = 0; i < passengerCount; i++)
                {
                    foreach (var ticket in passengerTickets[i])
                    {
                        ticket.IdPassager = -(i + 1); // Temporary negative ID (e.g., -1, -2, ...)
                    }
                }

                // Rebuild the tickets list
                tickets.Clear();
                foreach (var group in passengerTickets)
                {
                    tickets.AddRange(group);
                }

                return View(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar os tickets para o carrinho.");
                return StatusCode(500, "Ocorreu um erro ao carregar o carrinho.");
            }
        }

        // Ação POST para processar os tickets adicionados ao carrinho
        [HttpPost]
        public IActionResult Cart([FromBody] List<Ticket> tickets)
        {
            try
            {
                if (tickets == null || !tickets.Any())
                {
                    return Json(new { success = false, message = "Lista de tickets vazia." });
                }

                // Validate the tickets
                foreach (var ticket in tickets)
                {
                    // Ensure the ticket has a valid flight schedule
                    if (ticket.IdFlightSchedule == 0)
                    {
                        return Json(new { success = false, message = "Ticket inválido: ID de voo ausente." });
                    }

                    // Passenger ID should be 0 at this stage (not yet assigned)
                    ticket.IdPassager = 0; // Explicitly set to 0 since passenger info isn’t provided yet
                    ticket.Status = "Por validar"; // Set default status
                }

                // Retrieve existing tickets from session (if any)
                var existingTicketsJson = HttpContext.Session.GetString("CartTickets");
                var existingTickets = string.IsNullOrEmpty(existingTicketsJson)
                    ? new List<Ticket>()
                    : JsonConvert.DeserializeObject<List<Ticket>>(existingTicketsJson);

                // Add new tickets to the existing list
                existingTickets.AddRange(tickets);

                // Save the updated list back to session
                HttpContext.Session.SetString("CartTickets", JsonConvert.SerializeObject(existingTickets));

                return Json(new { success = true, message = "Tickets adicionados ao carrinho com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar os tickets.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Ação POST para remover um ticket do carrinho (session)
        [HttpPost]
        public IActionResult RemoveTicketFromCart([FromBody] RemoveTicketRequest request)
        {
            try
            {
                // Retrieve existing tickets from session
                var ticketsJson = HttpContext.Session.GetString("CartTickets");
                if (string.IsNullOrEmpty(ticketsJson))
                {
                    return Json(new { success = false, message = "Carrinho vazio." });
                }

                var tickets = JsonConvert.DeserializeObject<List<Ticket>>(ticketsJson);

                // Validate the index
                if (request.Index < 0 || request.Index >= tickets.Count)
                {
                    return Json(new { success = false, message = "Índice inválido." });
                }

                // Remove the ticket at the specified index
                tickets.RemoveAt(request.Index);

                // Update the session
                HttpContext.Session.SetString("CartTickets", JsonConvert.SerializeObject(tickets));

                return Json(new { success = true, message = "Ticket removido com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover ticket do carrinho.");
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult SubmitPassengerInfo([FromBody] Passenger passenger)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(passenger.Name) ||
                    string.IsNullOrEmpty(passenger.Surname) ||
                    string.IsNullOrEmpty(passenger.Gender))
                {
                    return Json(new { success = false, message = "All fields are required." });
                }

                // Set SeatNumber to null since it’s not provided yet
                passenger.SeatNumber = null;

                // Save the passenger to the database
                _context.Passengers.Add(passenger);
                _context.SaveChanges();

                // Retrieve tickets from session
                var ticketsJson = HttpContext.Session.GetString("CartTickets");
                var tickets = string.IsNullOrEmpty(ticketsJson)
                    ? new List<Ticket>()
                    : JsonConvert.DeserializeObject<List<Ticket>>(ticketsJson);

                if (!tickets.Any())
                {
                    return Json(new { success = false, message = "No tickets found in cart." });
                }

                // Update all tickets in the session with the passenger ID
                foreach (var ticket in tickets)
                {
                    ticket.IdPassager = passenger.Id;
                }

                // Save the updated tickets back to session
                HttpContext.Session.SetString("CartTickets", JsonConvert.SerializeObject(tickets));

                // Redirect back to the cart page
                return RedirectToAction("Cart", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving passenger information.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper class for the request body
        public class RemoveTicketRequest
        {
            public int Index { get; set; }
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
            if (email == "admin@gmail.com" && password == "admin")
            {
                return RedirectToAction("Create", "Manufacturers");
            }

            var user = _context.Accounts.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("CurrentUserID", user.Id);
                return RedirectToAction("Index", "Home");
            }

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

                HttpContext.Session.SetInt32("PassengersCount", Passengers);

                ViewBag.Origin = Origin;
                ViewBag.Destination = Destination;
                ViewBag.Departure = Departure;
                ViewBag.Arrival = Arrival;
                ViewBag.Passengers = Passengers;

                // Converter para DateOnly (apenas a data, sem horário)
                DateOnly departureDateOnly = DateOnly.FromDateTime(departureDate);

                // Consulta na tabela Flightschedules, retornando FlightSchedule diretamente
                var flightSchedules = await _context.Flightschedules
                    .Include(fs => fs.IdFlightNavigation)
                    .ThenInclude(f => f.IdOriginNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdDestinationNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdAircraftNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdClassNavigation)
                    .Where(fs => fs.FlightDate == departureDateOnly &&
                                 fs.IdFlightNavigation.IdOriginNavigation.Name == Origin &&
                                 fs.IdFlightNavigation.IdDestinationNavigation.Name == Destination)
                    .OrderBy(fs => fs.IdFlightNavigation.DepartureTime) // Ordenar pelo horário de partida
                    .ToListAsync();

                // Tratar os voos sem aeronave
                foreach (var schedule in flightSchedules)
                {
                    if (schedule.IdFlightNavigation.IdAircraftNavigation == null)
                    {
                        schedule.IdFlightNavigation.IdAircraftNavigation = new Aircraft(); // Ou defina um valor padrão
                    }
                }

                // Carregar todas as classes disponíveis na base de dados
                var allClasses = await _context.Classes.ToListAsync();
                ViewBag.Classes = allClasses;

                // Passar os FlightSchedules para a view
                return View(flightSchedules);
            }
            catch (Exception ex)
            {
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
                var flightSchedules = await _context.Flightschedules
                    .Include(fs => fs.IdFlightNavigation)
                        .ThenInclude(f => f.IdOriginNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdDestinationNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdAircraftNavigation)
                    .Include(fs => fs.IdFlightNavigation.IdClassNavigation)
                    .Where(fs => fs.FlightDate == arrivalDateOnly // Garante que pega apenas a data da volta
                                && fs.IdFlightNavigation.IdOriginNavigation.Name == Destination // Origem do voo de volta é o destino da ida
                                && fs.IdFlightNavigation.IdDestinationNavigation.Name == Origin // Destino do voo de volta é a origem da ida
                                && fs.FlightDate >= today) // Apenas voos futuros
                    .OrderBy(fs => fs.IdFlightNavigation.DepartureTime) // Ordenar pelo horário de partida
                    .ToListAsync();

                // Tratar voos sem aeronave
                foreach (var schedule in flightSchedules)
                {
                    if (schedule.IdFlightNavigation.IdAircraftNavigation == null)
                    {
                        schedule.IdFlightNavigation.IdAircraftNavigation = new Aircraft(); // Ou valor padrão
                    }
                }

                // Carregar todas as classes disponíveis na base de dados
                var allClasses = await _context.Classes.ToListAsync();

                // Passar as classes para a view usando ViewBag
                ViewBag.Classes = allClasses;

                // Passar os FlightSchedules para a view
                return View(flightSchedules);
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
