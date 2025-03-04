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
    public class OrderbuysController : Controller
    {
        private readonly BeaverEnterprisesContext _context;

        public OrderbuysController(BeaverEnterprisesContext context)
        {
            _context = context;
        }

        // GET: Orderbuys
        public async Task<IActionResult> Index()
        {
            var beaverEnterprisesContext = _context.Orderbuys.Include(o => o.IdOrderNavigation).Include(o => o.IdTicketNavigation);
            return View(await beaverEnterprisesContext.ToListAsync());
        }

        // GET: Orderbuys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderbuy = await _context.Orderbuys
                .Include(o => o.IdOrderNavigation)
                .Include(o => o.IdTicketNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderbuy == null)
            {
                return NotFound();
            }

            return View(orderbuy);
        }

        // GET: Orderbuys/Create
        public IActionResult Create()
        {
            ViewData["IdOrder"] = new SelectList(_context.Orders, "Id", "Id");
            ViewData["IdTicket"] = new SelectList(_context.Tickets, "Id", "Id");
            return View();
        }

        // POST: Orderbuys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdTicket,IdOrder")] Orderbuy orderbuy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderbuy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdOrder"] = new SelectList(_context.Orders, "Id", "Id", orderbuy.IdOrder);
            ViewData["IdTicket"] = new SelectList(_context.Tickets, "Id", "Id", orderbuy.IdTicket);
            return View(orderbuy);
        }

        // GET: Orderbuys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderbuy = await _context.Orderbuys.FindAsync(id);
            if (orderbuy == null)
            {
                return NotFound();
            }
            ViewData["IdOrder"] = new SelectList(_context.Orders, "Id", "Id", orderbuy.IdOrder);
            ViewData["IdTicket"] = new SelectList(_context.Tickets, "Id", "Id", orderbuy.IdTicket);
            return View(orderbuy);
        }

        // POST: Orderbuys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdTicket,IdOrder")] Orderbuy orderbuy)
        {
            if (id != orderbuy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderbuy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderbuyExists(orderbuy.Id))
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
            ViewData["IdOrder"] = new SelectList(_context.Orders, "Id", "Id", orderbuy.IdOrder);
            ViewData["IdTicket"] = new SelectList(_context.Tickets, "Id", "Id", orderbuy.IdTicket);
            return View(orderbuy);
        }

        // GET: Orderbuys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderbuy = await _context.Orderbuys
                .Include(o => o.IdOrderNavigation)
                .Include(o => o.IdTicketNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderbuy == null)
            {
                return NotFound();
            }

            return View(orderbuy);
        }

        // POST: Orderbuys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderbuy = await _context.Orderbuys.FindAsync(id);
            if (orderbuy != null)
            {
                _context.Orderbuys.Remove(orderbuy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderbuyExists(int id)
        {
            return _context.Orderbuys.Any(e => e.Id == id);
        }
    }
}
