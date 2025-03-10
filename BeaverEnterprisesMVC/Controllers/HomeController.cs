
using System.Diagnostics;
using System.Text;
using BeaverEnterprisesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

//gabriel

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
        // POST: Processar o formulário de check-in
        [HttpPost]
        public async Task<IActionResult> Index(string TicketCode, string PassengerCode)
        {
            // Converter os valores para inteiros
            if (!int.TryParse(TicketCode, out int ticketId) || !int.TryParse(PassengerCode, out int passengerId))
            {
                TempData["ErrorMessage"] = "Os códigos de ticket e passageiro devem ser números válidos.";
                return RedirectToAction("Index");
            }

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserID");
            if (currentUserId == null)
            {
                return RedirectToAction("Register", "Home");
            }

            // 1. Verificar se a conta existe
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == currentUserId);

            if (account == null)
            {
                TempData["ErrorMessageAccount"] = "Conta não encontrada.";
                return RedirectToAction("Index");
            }

            // 2. Verificar se o TicketId e o PassengerId estão associados à conta
            // Consulta explícita usando Joins para seguir a cadeia Account → Order → OrderBuy → Ticket → Passenger
            var ticket = await (from t in _context.Tickets
                                join ob in _context.Orderbuys on t.Id equals ob.IdTicket
                                join o in _context.Orders on ob.IdOrder equals o.Id
                                join a in _context.Accounts on o.IdAccount equals a.Id
                                where a.Id == currentUserId && t.Id == ticketId && t.IdPassager == passengerId
                                select t)
                              .FirstOrDefaultAsync();

            if (ticket == null)
            {
                TempData["ErrorMessageTicket"] = "Ticket não encontrado ou não associado ao passageiro informado para esta conta.";
                return RedirectToAction("Index");
            }

            // 3. Realizar o check-in
            if (ticket.Status == "Validado")
            {
                TempData["ErrorMessage"] = "O ticket já foi validado anteriormente.";
                return RedirectToAction("Index");
            }

            ticket.Status = "Validado";
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Check-in realizado com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserID");

            var user = _context.Accounts.FirstOrDefault(u => u.Id == currentUserId);
         
            ViewBag.User = user;

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

        public IActionResult ClearSession()
        {
            HttpContext.Session.Remove("CurrentUserID");
            TempData["Logout"] = "LoggedOut";
            return RedirectToAction("Index");  // Ou qualquer outra ação desejada após limpar a sessão
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

                    ticket.IdPassager = 0; // Explicitly set to 0 since passenger info isn’t provided yet
                    ticket.Status = "Por validar"; // Set default status
                    ticket.IdFlightScheduleNavigation = null; // Limpar navegação para evitar problemas na sessão
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
        public IActionResult Register()
        {

            if (HttpContext.Session.GetInt32("CurrentUserID") != null)
            {
               return RedirectToAction("Profile", "Home");
            }
            
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

            string passHash = HashPassword(password);

            var user = _context.Accounts.FirstOrDefault(u => u.Email == email && u.Password == passHash);
            if (user != null)
            {           
                TempData["Logged"] = "Logged with: " + email;
                HttpContext.Session.SetInt32("CurrentUserID", user.Id);
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessageLogin"] = "Invalid username or password!";
            return View("Register", "Home");
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

                // Não criar novos objetos; apenas pular se nulo (evitar new Aircraft())
                foreach (var schedule in flightSchedules)
                {
                    if (schedule.IdFlightNavigation.IdAircraftNavigation == null)
                    {
                        // Não atribuir new Aircraft(); deixar como null
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
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            if (!HttpContext.Session.GetInt32("CurrentUserID").HasValue) 
            {
                return View("Register", "Home");
            }
            _logger.LogInformation("Accessing Checkout GET...");
            return View();
        }

       


        [HttpPost]
        public async Task<IActionResult> ProcessCheckout()
        {
            try
            {
                _logger.LogInformation("Starting ProcessCheckout POST...");

                var currentUserId = HttpContext.Session.GetInt32("CurrentUserID");
                if (currentUserId == null)
                {
                    _logger.LogWarning("CurrentUserID is null. User must be logged in.");
                    return Json(new { success = false, message = "User must be logged in to complete the checkout." });
                }

                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Id == currentUserId);
                if (account == null)
                {
                    _logger.LogWarning("Account not found for UserId: {UserId}", currentUserId);
                    return Json(new { success = false, message = "Account not found." });
                }

                var ticketsJson = HttpContext.Session.GetString("CartTickets");
                if (string.IsNullOrEmpty(ticketsJson))
                {
                    _logger.LogWarning("CartTickets is empty in session.");
                    return Json(new { success = false, message = "Cart is empty." });
                }

                var tickets = JsonConvert.DeserializeObject<List<Ticket>>(ticketsJson);
                if (!tickets.Any())
                {
                    _logger.LogWarning("No tickets found after deserialization.");
                    return Json(new { success = false, message = "No tickets found in the cart." });
                }

                // Validação e tratamento de tickets
                foreach (var ticket in tickets)
                {
                    // Validar IdFlightSchedule
                    if (ticket.IdFlightSchedule <= 0)
                    {
                        _logger.LogWarning("Invalid IdFlightSchedule ({IdFlightSchedule}) for ticket.", ticket.IdFlightSchedule);
                        return Json(new { success = false, message = "All tickets must have a valid flight schedule." });
                    }

                    var flightScheduleExists = await _context.Flightschedules.AnyAsync(fs => fs.Id == ticket.IdFlightSchedule);
                    if (!flightScheduleExists)
                    {
                        _logger.LogWarning("FlightSchedule with Id {IdFlightSchedule} does not exist.", ticket.IdFlightSchedule);
                        return Json(new { success = false, message = $"Flight schedule with ID {ticket.IdFlightSchedule} does not exist." });
                    }

                    // Tratar IdPassager
                    if (!ticket.IdPassager.HasValue || ticket.IdPassager <= 0)
                    {
                        ticket.IdPassager = null;
                        _logger.LogInformation("IdPassager set to null for ticket with IdFlightSchedule: {IdFlightSchedule}", ticket.IdFlightSchedule);
                    }
                    else
                    {
                        var passengerExists = await _context.Passengers.AnyAsync(p => p.Id == ticket.IdPassager);
                        if (!passengerExists)
                        {
                            _logger.LogWarning("Passenger with Id {IdPassager} does not exist for ticket.", ticket.IdPassager);
                            return Json(new { success = false, message = $"Passenger with ID {ticket.IdPassager} does not exist." });
                        }
                        _logger.LogInformation("Passenger with Id {IdPassager} validated for ticket.", ticket.IdPassager);
                    }

                    // Definir valores padrão para propriedades obrigatórias ou anuláveis
                    if (string.IsNullOrEmpty(ticket.Status))
                    {
                        ticket.Status = "Por validar";
                        _logger.LogInformation("Set default Status for ticket: {Status}", ticket.Status);
                    }

                    if (!ticket.Price.HasValue)
                    {
                        ticket.Price = 100.00; // Valor padrão, ajuste conforme necessário
                        _logger.LogInformation("Set default Price for ticket: {Price}", ticket.Price);
                    }

                    if (!ticket.SeatNumber.HasValue)
                    {
                        ticket.SeatNumber = 0; // Valor padrão
                        _logger.LogInformation("Set default SeatNumber for ticket: {SeatNumber}", ticket.SeatNumber);
                    }

                    // Limpar propriedade de navegação para evitar salvar entidades relacionadas
                    ticket.IdFlightScheduleNavigation = null;
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var order = new Order
                        {
                            IdAccount = currentUserId.Value,
                            Date = DateOnly.FromDateTime(DateTime.Now),
                            Status = "Completed"
                        };
                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();

                        foreach (var ticket in tickets)
                        {
                            // Garantir que o Id do ticket seja tratado como novo registro
                            ticket.Id = 0; // O EF gerará um novo valor para a coluna IDENTITY
                            _context.Tickets.Add(ticket);
                            await _context.SaveChangesAsync();

                            var orderBuy = new Orderbuy
                            {
                                IdOrder = order.Id,
                                IdTicket = ticket.Id
                            };
                            _context.Orderbuys.Add(orderBuy);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error during transaction in ProcessCheckout. StackTrace: {StackTrace}", ex.StackTrace);
                        return Json(new { success = false, message = "An error occurred during transaction: " + ex.Message });
                    }
                }

                HttpContext.Session.Remove("CartTickets");
                HttpContext.Session.Remove("PassengersCount");

                _logger.LogInformation("Checkout completed successfully!");
                return Json(new { success = true, message = "Checkout completed successfully!", redirectUrl = Url.Action("Index", "Home") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout. StackTrace: {StackTrace}", ex.StackTrace);
                return Json(new { success = false, message = "An error occurred during checkout: " + ex.Message });
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
