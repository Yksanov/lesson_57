using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    [Authorize]
    public class MyTaskController : Controller
    {
        private readonly TaskStoreContext _context;

        public MyTaskController(TaskStoreContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index(Priority? priority, Status? status, string? taskname, DateOnly? dateFrom, DateOnly? dateTo, string? description, SortTaskState? sortOrder = SortTaskState.NameAsc, int page = 1)
        {
            IEnumerable<MyTask> task = await _context.MyTasks.ToListAsync();
            ViewBag.NameSort = sortOrder == SortTaskState.NameAsc ? SortTaskState.NameDesc : SortTaskState.NameAsc;
            ViewBag.PrioritySort = sortOrder == SortTaskState.PriorityAsc ? SortTaskState.PriorityDesc : SortTaskState.PriorityAsc;
            ViewBag.StatusSort = sortOrder == SortTaskState.StatusAsc ? SortTaskState.StatusDesc : SortTaskState.StatusAsc;
            ViewBag.CreateDate = sortOrder == SortTaskState.CreatedDateAsc ? SortTaskState.CreatedDateDesc : SortTaskState.CreatedDateAsc;
            switch (sortOrder)
            {
                case SortTaskState.NameAsc:
                    task = task.OrderBy(t => t.Name);
                    break;
                case SortTaskState.NameDesc:
                    task = task.OrderByDescending(t => t.Name);
                    break;
                case SortTaskState.PriorityAsc:
                    task = task.OrderBy(t => t.Priority);
                    break;
                case SortTaskState.PriorityDesc:
                    task = task.OrderByDescending(t => t.Priority);
                    break;
                case SortTaskState.StatusAsc:
                    task = task.OrderBy(t => t.Status);
                    break;
                case SortTaskState.StatusDesc:
                    task = task.OrderByDescending(t => t.Status);
                    break;
                case SortTaskState.CreatedDateAsc:
                    task = task.OrderBy(t => t.CreatedDate);
                    break;
                case SortTaskState.CreatedDateDesc:
                    task = task.OrderByDescending(t => t.CreatedDate);
                    break;
            }

            if (taskname != null)
                task = task.Where(t => t.Name == taskname);
            
            ViewData["Priorities"] = new SelectList(Enum.GetValues(typeof(Priority)).Cast<Priority>(), "Priority");
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)).Cast<Status>(), "Status");
            if (priority.HasValue)
                task = task.Where(t => t.Priority == priority.Value);
            if (status.HasValue)
                task = task.Where(t => t.Status == status.Value);

            if (dateFrom != null)
                task = task.Where(t => t.CreatedDate >= dateFrom);
            if (dateTo != null)
                task = task.Where(t => t.CreatedDate <= dateTo);
            if (description != null)
                task = task.Where(t => t.Description.Contains(description));
            
            int pageSize = 3;
            var items = task.Skip((page - 1) * pageSize).Take(pageSize);
            PageViewModel pvm = new PageViewModel(task.Count(), page, pageSize);

            var vm = new TaskModels
            {
                Tasks = items.ToList(),
                PageViewModel = pvm
            };
            
            return View(vm);
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
        public async Task<IActionResult> Create([Bind("Name,Description,Priority,UserName")] MyTask myTask)
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
            ViewData["Priorities"] = new SelectList(Enum.GetValues(typeof(Priority)).Cast<Priority>());
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
                    myTask.CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow);
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
            ViewData["Priorities"] = new SelectList(Enum.GetValues(typeof(Priority)).Cast<Priority>());
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
            if (myTask == null)
            {
                return NotFound();
            }

            myTask.Status = Status.Закрыта;
            _context.MyTasks.Remove(myTask);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        //---------------------------------------------------
        public async Task<IActionResult> Open(int id)
        {
            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask == null || myTask.Status != Status.Новая)
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
            
            if (myTask == null || myTask.Status != Status.Открыта)
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

        public async Task<IActionResult> TakeTask(int id)
        {
            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask == null || myTask.ExecutorId != null)
            {
                return NotFound();
            }

            myTask.ExecutorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            myTask.Status = Status.Открыта;
            myTask.OpenDate = DateOnly.FromDateTime(DateTime.Now);
            
            _context.Update(myTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyTaskExists(int id)
        {
            return _context.MyTasks.Any(e => e.Id == id);
        }
    }
}
