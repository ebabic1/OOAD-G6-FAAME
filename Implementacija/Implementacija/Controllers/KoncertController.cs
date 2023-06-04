using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Implementacija.Data;
using Implementacija.Models;

namespace Implementacija.Controllers
{
    public class KoncertController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KoncertController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Koncert
        public async Task<IActionResult> Index()
        {
            return View(await _context.Koncerti.ToListAsync());
        }

        // GET: Koncert/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var koncert = await _context.Koncerti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (koncert == null)
            {
                return NotFound();
            }

            return View(koncert);
        }

        // GET: Koncert/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Koncert/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,naziv,izvodjacId,zanr")] Koncert koncert)
        {
            if (ModelState.IsValid)
            {
                _context.Add(koncert);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(koncert);
        }

        // GET: Koncert/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var koncert = await _context.Koncerti.FindAsync(id);
            if (koncert == null)
            {
                return NotFound();
            }
            return View(koncert);
        }

        // POST: Koncert/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,naziv,izvodjacId,zanr")] Koncert koncert)
        {
            if (id != koncert.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(koncert);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KoncertExists(koncert.Id))
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
            return View(koncert);
        }

        // GET: Koncert/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var koncert = await _context.Koncerti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (koncert == null)
            {
                return NotFound();
            }

            return View(koncert);
        }

        // POST: Koncert/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var koncert = await _context.Koncerti.FindAsync(id);
            _context.Koncerti.Remove(koncert);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KoncertExists(int id)
        {
            return _context.Koncerti.Any(e => e.Id == id);
        }
    }
}
