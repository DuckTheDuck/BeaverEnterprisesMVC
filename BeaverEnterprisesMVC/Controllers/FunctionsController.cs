using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeaverEnterprisesMVC.Models;

namespace BeaverEnterprisesMVC.Controllers
{
    public class FunctionsController : Controller
    {
        private readonly BeaverEnterprisesContext _context;

        public FunctionsController(BeaverEnterprisesContext context)
        {
            _context = context;
        }

        // GET: Functions
        public async Task<IActionResult> Index()
        {
            var beaverEnterprisesContext = _context.Functions.Include(f => f.IdEmployeeNavigation).Include(f => f.IdFlightNavigation);
            return View(await beaverEnterprisesContext.ToListAsync());
        }

        // GET: Functions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var function = await _context.Functions
                .Include(f => f.IdEmployeeNavigation)
                .Include(f => f.IdFlightNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (function == null)
            {
                return NotFound();
            }

            return View(function);
        }

        // GET: Functions/Create
        public IActionResult Create()
        {
            ViewData["IdEmployee"] = new SelectList(_context.Employees, "Id", "Id");
            ViewData["IdFlight"] = new SelectList(_context.Flights, "Id", "Id");
            return View();
        }

        // POST: Functions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdEmployee,IdFlight,FunctionDescription")] Function function)
        {
            ModelState.Remove("IdEmployeeNavigation");
            ModelState.Remove("IdFlightNavigation");


            var newFlight = await _context.Flights.FindAsync(function.IdFlight);
    if (newFlight == null)
    {
        ModelState.AddModelError("", "The flight doesnt exist.");
    }
    else
    {
        var employeeFlights = _context.Functions
            .Where(f => f.IdEmployee == function.IdEmployee)
            .Select(f => f.IdFlight)
            .ToList();

        var overlappingFlights = _context.Flights
            .Where(f => employeeFlights.Contains(f.Id) &&
                        f.StartDate.Value.Date == newFlight.StartDate.Value.Date) 
            .ToList();

        if (overlappingFlights.Any())
        {
            ModelState.AddModelError("", "This employee is already scheduled for a flight on this day.");
        }
    }


            if (ModelState.IsValid)
            {
                _context.Add(function);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            
            
            ViewData["IdEmployee"] = new SelectList(_context.Employees, "Id", "Id", function.IdEmployee);
            ViewData["IdFlight"] = new SelectList(_context.Flights, "Id", "Id", function.IdFlight);
            return View(function);
        }

        // GET: Functions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var function = await _context.Functions.FindAsync(id);
            if (function == null)
            {
                return NotFound();
            }
            ViewData["IdEmployee"] = new SelectList(_context.Employees, "Id", "Id", function.IdEmployee);
            ViewData["IdFlight"] = new SelectList(_context.Flights, "Id", "Id", function.IdFlight);
            return View(function);
        }

        // POST: Functions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdEmployee,IdFlight,FunctionDescription")] Function function)
        {
            if (id != function.Id)
            {
                return NotFound();
            }

            ModelState.Remove("IdEmployeeNavigation");
            ModelState.Remove("IdFlightNavigation");

            // Buscar o voo atualizado
            var newFlight = await _context.Flights.FindAsync(function.IdFlight);
            if (newFlight == null)
            {
                ModelState.AddModelError("", "The flight doesnt exist.");
            }
            else
            {
                var overlappingFlights = _context.Functions
                    .Where(f => f.IdEmployee == function.IdEmployee && f.Id != function.Id) 
                    .Join(_context.Flights,
                        func => func.IdFlight,
                        flight => flight.Id,
                        (func, flight) => flight)
                    .Where(f => f.StartDate.Value.Date == newFlight.StartDate.Value.Date) 
                    .ToList();

                if (overlappingFlights.Any())
                {
                    ModelState.AddModelError("", "This employee is already scheduled for a flight on this day.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(function);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Functions.Any(e => e.Id == function.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["IdEmployee"] = new SelectList(_context.Employees, "Id", "Name", function.IdEmployee);
            ViewData["IdFlight"] = new SelectList(_context.Flights, "Id", "FlightNumber", function.IdFlight);
            return View(function);
        }


        // GET: Functions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var function = await _context.Functions
                .Include(f => f.IdEmployeeNavigation)
                .Include(f => f.IdFlightNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (function == null)
            {
                return NotFound();
            }

            return View(function);
        }

        // POST: Functions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var function = await _context.Functions.FindAsync(id);
            if (function != null)
            {
                _context.Functions.Remove(function);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunctionExists(int id)
        {
            return _context.Functions.Any(e => e.Id == id);
        }
    }
}
