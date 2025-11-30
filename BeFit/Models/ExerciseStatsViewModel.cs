using System.ComponentModel.DataAnnotations;

public class ExerciseStatsViewModel
{
    public int ExerciseTypeId { get; set; }

    [Display(Name = "Ćwiczenie")]
    public string ExerciseName { get; set; }

    [Display(Name = "Ile razy wykonywane")]
    public int ExecutionsCount { get; set; }

    [Display(Name = "Łączna liczba powtórzeń")]
    public int TotalRepetitions { get; set; }

    [Display(Name = "Średnie obciążenie [kg]")]
    public double AverageLoad { get; set; }

    [Display(Name = "Maksymalne obciążenie [kg]")]
    public int MaxLoad { get; set; }
}
