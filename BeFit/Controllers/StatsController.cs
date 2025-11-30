using System;
using System.Linq;
using System.Threading.Tasks;
using BeFit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class StatsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StatsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var dateFrom = DateTime.Today.AddDays(-28);

        var query =
            from pe in _context.PerformedExercises
                .Include(p => p.ExerciseType)
                .Include(p => p.TrainingSession)
            where pe.TrainingSession.StartTime >= dateFrom
            group pe by pe.ExerciseType into g
            select new ExerciseStatsViewModel
            {
                ExerciseTypeId = g.Key.Id,
                ExerciseName = g.Key.Name,
                ExecutionsCount = g.Count(),
                TotalRepetitions = g.Sum(x => x.Sets * x.RepsPerSet),
                AverageLoad = g.Average(x => x.Weight),
                MaxLoad = g.Max(x => x.Weight)
            };

        var model = await query
            .OrderBy(m => m.ExerciseName)
            .ToListAsync();

        return View(model);
    }
}
