using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Models;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    public class MyTaskController : Controller
    {
        private readonly TaskStoreContext _context;

        public MyTaskController(TaskStoreContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.MyTasks.ToListAsync());
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myTask = await _context.MyTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myTask == null)
            {
                return NotFound();
            }

            return View(myTask);
        }
        
        public IActionResult Create()
        {
            ViewData["Priorities"] = new SelectList(Enum.GetValues(typeof(Priority)).Cast<Priority>());
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Priority")] MyTask myTask)
        {
            if (ModelState.IsValid)
            {
                myTask.Status = Status.Новая;
                myTask.CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow);

                await _context.AddAsync(myTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Priorities"] = new SelectList(Enum.GetValues(typeof(Priority)).Cast<Priority>());
            return View(myTask);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask == null)
            {
                return NotFound();
            }
            return View(myTask);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MyTask myTask)
        {
            if (id != myTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(myTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyTaskExists(myTask.Id))
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
            return View(myTask);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myTask = await _context.MyTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myTask == null)
            {
                return NotFound();
            }

            return View(myTask);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask != null && myTask.Status == Status.Закрыта) 
            {
                _context.MyTasks.Remove(myTask);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //---------------------------------------------------
        public async Task<IActionResult> Open(int id)
        {
            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask != null || myTask.Status != Status.Новая)
            {
                return NotFound();
            }

            myTask.Status = Status.Открыта;
            myTask.OpenDate = DateOnly.FromDateTime(DateTime.Now);
            _context.Update(myTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Close(int id)
        {
            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask != null || myTask.Status != Status.Открыта)
            {
                return NotFound();
            }

            myTask.Status = Status.Закрыта;
            myTask.CloseDate = DateOnly.FromDateTime(DateTime.Now);
            _context.Update(myTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        //---------------------------------------------------

        private bool MyTaskExists(int id)
        {
            return _context.MyTasks.Any(e => e.Id == id);
        }
    }
}
