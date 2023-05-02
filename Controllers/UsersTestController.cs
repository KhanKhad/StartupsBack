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
    public class UsersTestController : Controller
    {
        private readonly MainDb _context;

        public UsersTestController(MainDb context)
        {
            _context = context;
        }

        // GET: UsersTest
        public async Task<IActionResult> Index()
        {
              return _context.UsersDB != null ? 
                          View(await _context.UsersDB.ToListAsync()) :
                          Problem("Entity set 'MainDb.UsersDB'  is null.");
        }

        // GET: UsersTest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsersDB == null)
            {
                return NotFound();
            }

            var userModel = await _context.UsersDB
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userModel == null)
            {
                return NotFound();
            }

            return View(userModel);
        }

        // GET: UsersTest/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UsersTest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PasswordHash,AccountCreated,ProfilePicFileName,Delta")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userModel);
        }

        // GET: UsersTest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsersDB == null)
            {
                return NotFound();
            }

            var userModel = await _context.UsersDB.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }
            return View(userModel);
        }

        // POST: UsersTest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Token,Name,PasswordHash,AccountCreated,ProfilePic,ProfilePicFileName,Delta")] UserModel userModel)
        {
            if (id != userModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserModelExists(userModel.Id))
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
            return View(userModel);
        }

        // GET: UsersTest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsersDB == null)
            {
                return NotFound();
            }

            var userModel = await _context.UsersDB
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userModel == null)
            {
                return NotFound();
            }

            return View(userModel);
        }

        // POST: UsersTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsersDB == null)
            {
                return Problem("Entity set 'MainDb.UsersDB'  is null.");
            }
            var userModel = await _context.UsersDB.FindAsync(id);
            if (userModel != null)
            {
                _context.UsersDB.Remove(userModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserModelExists(int id)
        {
          return (_context.UsersDB?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
