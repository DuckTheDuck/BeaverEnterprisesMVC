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
    public class FlightschedulesController : Controller
    {
        private readonly BeaverEnterprisesContext _context;

        public FlightschedulesController(BeaverEnterprisesContext context)
        {
            _context = context;
        }

        // GET: Flightschedules
        public async Task<IActionResult> Index()
        {
            var beaverEnterprisesContext = _context.Flightschedules.Include(f => f.IdFlightNavigation);
            return View(await beaverEnterprisesContext.ToListAsync());
        }

        // GET: Flightschedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightschedule = await _context.Flightschedules
                .Include(f => f.IdFlightNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flightschedule == null)
            {
                return NotFound();
            }

            return View(flightschedule);
        }

        // GET: Flightschedules/Create
        public IActionResult Create()
        {
            ViewData["IdFlight"] = new SelectList(_context.Flights, "Id", "Id");
            return View();
        }

        // POST: Flightschedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdFlight,FlightDate")] Flightschedule flightschedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flightschedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdFlight"] = new SelectList(_context.Flights, "Id", "Id", flightschedule.IdFlight);
            return View(flightschedule);
        }

        // GET: Flightschedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightschedule = await _context.Flightschedules.FindAsync(id);
            if (flightschedule == null)
            {
                return NotFound();
            }
            ViewData["IdFlight"] = new SelectList(_context.Flights, "Id", "Id", flightschedule.IdFlight);
            return View(flightschedule);
        }

        // POST: Flightschedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdFlight,FlightDate")] Flightschedule flightschedule)
        {
            if (id != flightschedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flightschedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightscheduleExists(flightschedule.Id))
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
            ViewData["IdFlight"] = new SelectList(_context.Flights, "Id", "Id", flightschedule.IdFlight);
            return View(flightschedule);
        }

        // GET: Flightschedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightschedule = await _context.Flightschedules
                .Include(f => f.IdFlightNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flightschedule == null)
            {
                return NotFound();
            }

            return View(flightschedule);
        }

        // POST: Flightschedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flightschedule = await _context.Flightschedules.FindAsync(id);
            if (flightschedule != null)
            {
                _context.Flightschedules.Remove(flightschedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightscheduleExists(int id)
        {
            return _context.Flightschedules.Any(e => e.Id == id);
        }
    }
}
