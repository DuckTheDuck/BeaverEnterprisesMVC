
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
                var p = HttpContext.Session.GetInt32("PassengersCount");
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

                int ticketsLength = tickets.Count();

                // Verifica se algum IdPassager é igual a 0
                if (tickets.Any(t => t.IdPassager == 0))
                {
                    List<Ticket> departure = tickets.Take(ticketsLength / 2).ToList(); // Primeira metade para ida
                    List<Ticket> arrival = tickets.Skip(ticketsLength / 2).Take(ticketsLength - (ticketsLength / 2)).ToList(); // Segunda metade para volta

                    tickets.Clear();
                    for (int i = 0; i < p; i++)
                    {
                        departure[i].IdPassager = -(i+1);
                        departure[i].Type = "One-way";
                        departure[i].Id = i + 1;
                        arrival[i].IdPassager = -(i + 1);
                        arrival[i].Type = "return";
                        arrival[i].Id = p.Value + i + 1;
                        tickets.Add(departure[i]);
                        tickets.Add(arrival[i]);
                    }
                }

                HttpContext.Session.SetString("CartTickets", JsonConvert.SerializeObject(tickets, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));

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
        public IActionResult SubmitPassengerInfo([FromBody] Passenger passenger, [FromQuery] int id)
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
                    if (ticket.IdPassager == id) { ticket.IdPassager = passenger.Id; }
                }

                // Save the updated tickets back to session
                HttpContext.Session.SetString("CartTickets", JsonConvert.SerializeObject(tickets));

                // Redirect back to the cart page
                return Json(new { success = true, redirectUrl = Url.Action("Cart", "Home") });
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

        public IActionResult PassengerInformation(int passengerid)
        {
            ViewBag.PassengerId = passengerid;

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
        public IActionResult SelectSeat(int passengerId, int ticketId)
        {
            // Log para depuração
            System.Diagnostics.Debug.WriteLine($"Received: passengerId={passengerId}, ticketId={ticketId}");

            var passengerCount = HttpContext.Session.GetInt32("PassengersCount") ?? 0;
            var ticketsJson = HttpContext.Session.GetString("CartTickets");
            var tickets = string.IsNullOrEmpty(ticketsJson)
                ? new List<Ticket>()
                : JsonConvert.DeserializeObject<List<Ticket>>(ticketsJson);

            if (!tickets.Any() || passengerCount == 0)
            {
                return View(new List<Ticket>());
            }

            var ticket = tickets.FirstOrDefault(t => t.Id == ticketId && t.IdPassager == passengerId);
            if (ticket == null)
            {
                return NotFound($"Bilhete ou passageiro não encontrado. ticketId: {ticketId}, passengerId: {passengerId}");
            }

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

            if (ticket.IdFlightScheduleNavigation == null)
            {
                return NotFound("Voo programado não encontrado.");
            }

            var capacity = ticket.IdFlightScheduleNavigation.IdFlightNavigation.IdAircraftNavigation.Capacity;
            int seatsPerRow = 3;
            int rows = (int)Math.Ceiling((double)capacity / seatsPerRow);

            var occupiedSeatsInDb = _context.Tickets
                .Where(t => t.IdFlightSchedule == ticket.IdFlightSchedule && t.SeatNumber.HasValue)
                .Select(t => t.SeatNumber.Value)
                .ToList();

            var occupiedSeatsInCart = tickets
                .Where(t => t.IdFlightSchedule == ticket.IdFlightSchedule && t.SeatNumber.HasValue)
                .Select(t => t.SeatNumber.Value)
                .ToList();

            var occupiedSeats = occupiedSeatsInDb.Union(occupiedSeatsInCart).ToList();

            ViewBag.PassengerId = passengerId;
            ViewBag.TicketId = ticketId;
            ViewBag.Rows = rows;
            ViewBag.SeatsPerRow = seatsPerRow;
            ViewBag.OccupiedSeats = occupiedSeats;
            ViewBag.TicketType = ticket.Type;

            return View(tickets);
        }

        [HttpPost]
        public IActionResult SetSeat(int ticketId, int seatNumber)
        {
            System.Diagnostics.Debug.WriteLine($"SetSeat Received: ticketId={ticketId}, seatNumber={seatNumber}");

            var ticketsJson = HttpContext.Session.GetString("CartTickets");
            var tickets = string.IsNullOrEmpty(ticketsJson)
                ? new List<Ticket>()
                : JsonConvert.DeserializeObject<List<Ticket>>(ticketsJson);

            var ticket = tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null)
            {
                TempData["Error"] = "Bilhete não encontrado.";
                return RedirectToAction("SelectSeat", new { ticketId });
            }

            var occupiedSeatsInCart = tickets
                .Where(t => t.IdFlightSchedule == ticket.IdFlightSchedule && t.SeatNumber.HasValue && t.Id != ticketId)
                .Select(t => t.SeatNumber.Value)
                .ToList();

            var occupiedSeatsInDb = _context.Tickets
                .Where(t => t.IdFlightSchedule == ticket.IdFlightSchedule && t.SeatNumber.HasValue)
                .Select(t => t.SeatNumber.Value)
                .ToList();

            var allOccupiedSeats = occupiedSeatsInDb.Union(occupiedSeatsInCart).ToList();

            if (allOccupiedSeats.Contains(seatNumber))
            {
                TempData["Error"] = "O assento selecionado já está ocupado.";
                return RedirectToAction("SelectSeat", new { passengerId = ticket.IdPassager, ticketId });
            }

            // Atribuir o assento ao bilhete
            ticket.SeatNumber = seatNumber;
            HttpContext.Session.SetString("CartTickets", JsonConvert.SerializeObject(tickets));

            // Verificar se existe um bilhete de volta para o mesmo passageiro
            var returnTicket = tickets.FirstOrDefault(t => t.IdPassager == ticket.IdPassager && t.Type == "return" && t.Id != ticketId);
            if (returnTicket != null && ticket.Type == "One-way")
            {
                TempData["Success"] = $"Assento {seatNumber} atribuído ao bilhete de ida. Agora selecione o assento de volta.";
                return RedirectToAction("SelectSeat", new { passengerId = ticket.IdPassager, ticketId = returnTicket.Id });
            }

            TempData["Success"] = $"Assento {seatNumber} atribuído ao bilhete com sucesso!";
            return RedirectToAction("Cart", "Home"); // Ou outra página final, como um resumo
        }
    }
}
