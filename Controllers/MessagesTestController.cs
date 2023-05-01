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
    public class MessagesTestController : Controller
    {
        private readonly MainDb _context;

        public MessagesTestController(MainDb context)
        {
            _context = context;
        }

        // GET: MessagesTest
        public async Task<IActionResult> Index()
        {
            var mainDb = _context.MessagesDB.Include(m => m.Recipient).Include(m => m.Sender);
            return View(await mainDb.ToListAsync());
        }

        // GET: MessagesTest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MessagesDB == null)
            {
                return NotFound();
            }

            var messageModel = await _context.MessagesDB
                .Include(m => m.Recipient)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (messageModel == null)
            {
                return NotFound();
            }

            return View(messageModel);
        }

        // GET: MessagesTest/Create
        public IActionResult Create()
        {
            ViewData["RecipientForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id");
            ViewData["SenderForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id");
            return View();
        }

        // POST: MessagesTest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Message,SenderForeignKey,RecipientForeignKey,MessageSended,Messagereaded,IsGetted,IsReaded")] MessageModel messageModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(messageModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipientForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", messageModel.RecipientForeignKey);
            ViewData["SenderForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", messageModel.SenderForeignKey);
            return View(messageModel);
        }

        // GET: MessagesTest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MessagesDB == null)
            {
                return NotFound();
            }

            var messageModel = await _context.MessagesDB.FindAsync(id);
            if (messageModel == null)
            {
                return NotFound();
            }
            ViewData["RecipientForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", messageModel.RecipientForeignKey);
            ViewData["SenderForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", messageModel.SenderForeignKey);
            return View(messageModel);
        }

        // POST: MessagesTest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,SenderForeignKey,RecipientForeignKey,MessageSended,Messagereaded,IsGetted,IsReaded")] MessageModel messageModel)
        {
            if (id != messageModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageModelExists(messageModel.Id))
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
            ViewData["RecipientForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", messageModel.RecipientForeignKey);
            ViewData["SenderForeignKey"] = new SelectList(_context.UsersDB, "Id", "Id", messageModel.SenderForeignKey);
            return View(messageModel);
        }

        // GET: MessagesTest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MessagesDB == null)
            {
                return NotFound();
            }

            var messageModel = await _context.MessagesDB
                .Include(m => m.Recipient)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (messageModel == null)
            {
                return NotFound();
            }

            return View(messageModel);
        }

        // POST: MessagesTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MessagesDB == null)
            {
                return Problem("Entity set 'MainDb.MessagesDB'  is null.");
            }
            var messageModel = await _context.MessagesDB.FindAsync(id);
            if (messageModel != null)
            {
                _context.MessagesDB.Remove(messageModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageModelExists(int id)
        {
          return (_context.MessagesDB?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
