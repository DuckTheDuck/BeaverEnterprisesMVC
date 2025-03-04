using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeaverEnterprisesMVC.Models;

namespace BeaverEnterprisesMVC.Controllers
{
    public class FlightsController : Controller
    {
        private readonly BeaverEnterprisesContext _context;

        public FlightsController(BeaverEnterprisesContext context)
        {
            _context = context;
        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            var beaverEnterprisesContext = _context.Flights
                .Include(f => f.IdAircraftNavigation)
                .Include(f => f.IdClassNavigation)
                .Include(f => f.IdDestinationNavigation)
                .Include(f => f.IdOriginNavigation);
            return View(await beaverEnterprisesContext.ToListAsync());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.IdAircraftNavigation)
                .Include(f => f.IdClassNavigation)
                .Include(f => f.IdDestinationNavigation)
                .Include(f => f.IdOriginNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }
        public void GenerateFlightSchedules()
        {
            DateTime today = DateTime.Today;
            DateTime maxDate = today.AddDays(30); // Pode ser ajustado para um intervalo maior, como 30 dias, ou conforme sua necessidade.

            var flights = _context.Flights.ToList(); // Pega todos os voos

            foreach (var flight in flights)
            {
                DateTime currentDate = flight.StartDate ?? today; // Se não houver data de início, usa o dia de hoje

                // Gerar agendamentos até a data final
                while (currentDate <= maxDate)
                {
                    // Convertendo a data de 'currentDate' para 'DateOnly' e verificando se já existe um agendamento para esse voo nesta data
                    DateOnly flightDateOnly = DateOnly.FromDateTime(currentDate); // Converte para DateOnly

                    bool exists = _context.Flightschedules
                        .Any(fs => fs.IdFlight == flight.Id && fs.FlightDate == flightDateOnly); // Compara com DateOnly

                    if (!exists)
                    {
                        // Adicionar o novo agendamento para o voo
                        _context.Flightschedules.Add(new Flightschedule
                        {
                            IdFlight = flight.Id,
                            FlightDate = flightDateOnly, // Armazena a data com DateOnly
                            IdFlightNavigation = flight, // Relacionamento com o voo
                        });
                    }

                    // Atualizar a data conforme a periodicidade
                    switch (flight.Periocity)
                    {
                        case 0:
                            currentDate = currentDate.AddDays(1); // Para voos diários
                            break;
                        case 7:
                            currentDate = currentDate.AddDays(7); // Para voos semanais
                            break;
                        case 30:
                            currentDate = currentDate.AddMonths(1); // Para voos mensais
                            break;
                        default:
                            currentDate = currentDate.AddDays(1); // Caso a periodicidade não seja identificada
                            break;
                    }
                }
            }
            _context.SaveChanges(); // Salva todas as alterações no banco de dados
        }


        // GET: Flights/Create
        public IActionResult Create()
        {
            // Popula as listas para os campos de seleção
            ViewData["IdAircraft"] = new SelectList(_context.Aircraft, "Id", "Id");
            ViewData["IdClass"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["IdDestination"] = new SelectList(_context.Locations, "Id", "Id");
            ViewData["IdOrigin"] = new SelectList(_context.Locations, "Id", "Id");
            return View();
        }

        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FlightNumber,IdOrigin,IdDestination,DepartureTime,ArrivalTime,IdAircraft,IdClass,Periocity,StartDate,EndDate")] Flight flight)
        {
            ModelState.Remove("IdClassNavigation");
            ModelState.Remove("IdOriginNavigation");
            ModelState.Remove("IdDestinationNavigation");
            ModelState.Remove("IdAircraftNavigation");
            if (ModelState.IsValid)
            {
                // Adiciona o voo à base de dados
                _context.Add(flight);
                await _context.SaveChangesAsync();
                GenerateFlightSchedules();
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                // Exibe os erros de validação
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(flight);
            }


            // Se o modelo não for válido, recarrega as listas
            ViewData["IdAircraft"] = new SelectList(_context.Aircraft, "Id", "Id", flight.IdAircraft);
            ViewData["IdClass"] = new SelectList(_context.Classes, "Id", "Id", flight.IdClass);
            ViewData["IdDestination"] = new SelectList(_context.Locations, "Id", "Id", flight.IdDestination);
            ViewData["IdOrigin"] = new SelectList(_context.Locations, "Id", "Id", flight.IdOrigin);
            return View(flight);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            // Preenche as listas de seleção para o formulário de edição
            ViewData["IdAircraft"] = new SelectList(_context.Aircraft, "Id", "Id", flight.IdAircraft);
            ViewData["IdClass"] = new SelectList(_context.Classes, "Id", "Id", flight.IdClass);
            ViewData["IdDestination"] = new SelectList(_context.Locations, "Id", "Id", flight.IdDestination);
            ViewData["IdOrigin"] = new SelectList(_context.Locations, "Id", "Id", flight.IdOrigin);
            return View(flight);
        }

        // POST: Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlightNumber,IdOrigin,IdDestination,DepartureTime,ArrivalTime,IdAircraft,IdClass,Periocity,StartDate,EndDate")] Flight flight)
        {
            if (id != flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza o voo com os novos dados
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo não for válido, recarrega as listas de seleção
            ViewData["IdAircraft"] = new SelectList(_context.Aircraft, "Id", "Id", flight.IdAircraft);
            ViewData["IdClass"] = new SelectList(_context.Classes, "Id", "Id", flight.IdClass);
            ViewData["IdDestination"] = new SelectList(_context.Locations, "Id", "Id", flight.IdDestination);
            ViewData["IdOrigin"] = new SelectList(_context.Locations, "Id", "Id", flight.IdOrigin);
            return View(flight);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.IdAircraftNavigation)
                .Include(f => f.IdClassNavigation)
                .Include(f => f.IdDestinationNavigation)
                .Include(f => f.IdOriginNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
