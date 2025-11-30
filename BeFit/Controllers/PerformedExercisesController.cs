using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    public class PerformedExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PerformedExercisesController(ApplicationDbContext context, UserManager<AppUser> userManager)
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
                return Challenge();

            var exercises = _context.PerformedExercises
                .Include(p => p.ExerciseType)
                .Include(p => p.TrainingSession)
                .Where(p => p.UserId == userId);

            return View(await exercises.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();

            var exercise = await _context.PerformedExercises
                .Include(p => p.ExerciseType)
                .Include(p => p.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (exercise == null)
                return NotFound();

            return View(exercise);
        }

        public IActionResult Create(int? trainingSessionId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();

            var sessions = _context.TrainingSessions
                .Where(ts => ts.UserId == userId)
                .ToList();

            if (!sessions.Any())
            {
                TempData["Error"] = "Najpierw utwórz sesję treningową.";
                return RedirectToAction("Index", "TrainingSessions");
            }

            ViewData["ExerciseTypeId"] = new SelectList(
                _context.ExerciseTypes,
                "Id",
                "Name"
            );

            ViewData["TrainingSessionId"] = new SelectList(
                sessions,
                "Id",
                "StartTime",
                trainingSessionId
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,ExerciseTypeId,TrainingSessionId,Weight,Sets,RepsPerSet")]
            PerformedExercise performedExercise)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();

            if (performedExercise.TrainingSessionId == 0)
            {
                ModelState.AddModelError("TrainingSessionId", "Wybierz sesję treningową.");
            }

            if (ModelState.IsValid)
            {
                performedExercise.UserId = userId;

                _context.Add(performedExercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ExerciseTypeId"] = new SelectList(
                _context.ExerciseTypes,
                "Id",
                "Name",
                performedExercise.ExerciseTypeId
            );

            ViewData["TrainingSessionId"] = new SelectList(
                _context.TrainingSessions.Where(ts => ts.UserId == userId),
                "Id",
                "StartTime",
                performedExercise.TrainingSessionId
            );

            return View(performedExercise);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();

            var exercise = await _context.PerformedExercises
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (exercise == null)
                return NotFound();

            ViewData["ExerciseTypeId"] = new SelectList(
                _context.ExerciseTypes, "Id", "Name", exercise.ExerciseTypeId);

            ViewData["TrainingSessionId"] = new SelectList(
                _context.TrainingSessions.Where(ts => ts.UserId == userId),
                "Id", "StartTime", exercise.TrainingSessionId);

            return View(exercise);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,ExerciseTypeId,TrainingSessionId,Weight,Sets,RepsPerSet")]
            PerformedExercise performedExercise)
        {
            if (id != performedExercise.Id)
                return NotFound();

            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();

            if (ModelState.IsValid)
            {
                performedExercise.UserId = userId;

                _context.Update(performedExercise);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["ExerciseTypeId"] = new SelectList(
                _context.ExerciseTypes, "Id", "Name", performedExercise.ExerciseTypeId);

            ViewData["TrainingSessionId"] = new SelectList(
                _context.TrainingSessions.Where(ts => ts.UserId == userId),
                "Id", "StartTime", performedExercise.TrainingSessionId);

            return View(performedExercise);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();

            var exercise = await _context.PerformedExercises
                .Include(p => p.ExerciseType)
                .Include(p => p.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (exercise == null)
                return NotFound();

            return View(exercise);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();

            var exercise = await _context.PerformedExercises
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (exercise != null)
            {
                _context.PerformedExercises.Remove(exercise);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
