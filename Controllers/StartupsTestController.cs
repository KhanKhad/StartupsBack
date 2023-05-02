using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StartupsBack.Database;
using StartupsBack.Models.DbModels;

namespace StartupsBack.Controllers
{
    public class StartupsTestController : Controller
    {
        private readonly MainDb _context;

        public StartupsTestController(MainDb context)
        {
            _context = context;
        }

        // GET: StartupsTest
        public async Task<IActionResult> Index()
        {
            var mainDb = _context.StartupsDB.Include(s => s.Author);
            return View(await mainDb.ToListAsync());
        }

        // GET: StartupsTest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StartupsDB == null)
            {
                return NotFound();
            }

            var startupModel = await _context.StartupsDB
                .Include(s => s.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (startupModel == null)
            {
                return NotFound();
            }

            return View(startupModel);
        }

        // GET: StartupsTest/Create
        public IActionResult Create()
        {
            ViewData["AuthorForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id");
            return View();
        }

        // POST: StartupsTest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartupPublished,AuthorForeignKey,StartupPicFileName,Viewers,LastModify")] StartupModel startupModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(startupModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", startupModel.AuthorForeignKey);
            return View(startupModel);
        }

        // GET: StartupsTest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StartupsDB == null)
            {
                return NotFound();
            }

            var startupModel = await _context.StartupsDB.FindAsync(id);
            if (startupModel == null)
            {
                return NotFound();
            }
            ViewData["AuthorForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", startupModel.AuthorForeignKey);
            return View(startupModel);
        }

        // POST: StartupsTest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartupPublished,AuthorForeignKey,StartupPicFileName,Viewers,LastModify")] StartupModel startupModel)
        {
            if (id != startupModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(startupModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StartupModelExists(startupModel.Id))
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
            ViewData["AuthorForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", startupModel.AuthorForeignKey);
            return View(startupModel);
        }

        // GET: StartupsTest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StartupsDB == null)
            {
                return NotFound();
            }

            var startupModel = await _context.StartupsDB
                .Include(s => s.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (startupModel == null)
            {
                return NotFound();
            }

            return View(startupModel);
        }

        // POST: StartupsTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StartupsDB == null)
            {
                return Problem("Entity set 'MainDb.StartupsDB'  is null.");
            }
            var startupModel = await _context.StartupsDB.FindAsync(id);
            if (startupModel != null)
            {
                _context.StartupsDB.Remove(startupModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StartupModelExists(int id)
        {
          return (_context.StartupsDB?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
