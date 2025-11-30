using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeFit.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Models;

namespace BeFit.Controllers
{
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TrainingSessionsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var sessions = await _context.TrainingSessions
                .Where(ts => ts.UserId == userId)
                .ToListAsync();

            return View(sessions);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            trainingSession.PerformedExercises = await _context.PerformedExercises
                .Include(pe => pe.ExerciseType)
                .Where(pe => pe.TrainingSessionId == trainingSession.Id)
                .ToListAsync();

            return View(trainingSession);
        }

        public IActionResult Create()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartTime,EndTime")] TrainingSession trainingSession)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            if (!trainingSession.IsValid())
            {
                ModelState.AddModelError(string.Empty, "Czas zakończenia musi być późniejszy niż początek.");
            }

            if (ModelState.IsValid)
            {
                trainingSession.UserId = userId;
                _context.Add(trainingSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(trainingSession);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(ts => ts.Id == id && ts.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(trainingSession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime")] TrainingSession trainingSession)
        {
            if (id != trainingSession.Id)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            if (!trainingSession.IsValid())
            {
                ModelState.AddModelError(string.Empty, "Czas zakończenia musi być późniejszy niż początek.");
            }

            if (ModelState.IsValid)
            {
                var existsForUser = await _context.TrainingSessions
                    .AnyAsync(ts => ts.Id == id && ts.UserId == userId);

                if (!existsForUser)
                {
                    return NotFound();
                }

                try
                {
                    trainingSession.UserId = userId;
                    _context.Update(trainingSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingSessionExists(trainingSession.Id))
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
            return View(trainingSession);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(trainingSession);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(ts => ts.Id == id && ts.UserId == userId);

            if (trainingSession != null)
            {
                _context.TrainingSessions.Remove(trainingSession);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainingSessionExists(int id)
        {
            return _context.TrainingSessions.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ForUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await _context.TrainingSessions
                .Where(ts => ts.UserId == id)
                .ToListAsync();

            return View(sessions);
        }
    }
}
