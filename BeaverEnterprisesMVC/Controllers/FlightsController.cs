using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeaverEnterprisesMVC.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            var beaverEnterprisesContext = _context.Flights.Include(f => f.IdAircraftNavigation).Include(f => f.IdClassNavigation).Include(f => f.IdDestinationNavigation).Include(f => f.IdOriginNavigation);
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

        // GET: Flights/Create
        public IActionResult Create()
        {
            ViewData["IdAircraft"] = new SelectList(_context.Aircraft, "Id", "Id");
            ViewData["IdClass"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["IdDestination"] = new SelectList(_context.Locations, "Id", "Id");
            ViewData["IdOrigin"] = new SelectList(_context.Locations, "Id", "Id");
            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FlightNumber,IdOrigin,IdDestination,DepartureTime,ArrivalTime,IdAircraft,IdClass,Periocity")] Flight flight)
        {

            ModelState.Remove("IdClassNavigation");
            ModelState.Remove("IdOriginNavigation");
            ModelState.Remove("IdAircraftNavigation");
            ModelState.Remove("IdDestinationNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

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
            ViewData["IdAircraft"] = new SelectList(_context.Aircraft, "Id", "Id", flight.IdAircraft);
            ViewData["IdClass"] = new SelectList(_context.Classes, "Id", "Id", flight.IdClass);
            ViewData["IdDestination"] = new SelectList(_context.Locations, "Id", "Id", flight.IdDestination);
            ViewData["IdOrigin"] = new SelectList(_context.Locations, "Id", "Id", flight.IdOrigin);
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlightNumber,IdOrigin,IdDestination,DepartureTime,ArrivalTime,IdAircraft,IdClass,Periocity")] Flight flight)
        {
            if (id != flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
