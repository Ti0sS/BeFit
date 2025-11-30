using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class PerformedExercise
    {
        public int Id { get; set; }

        [Display(Name = "Typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }
        public ExerciseType ExerciseType { get; set; }

        [Display(Name = "Sesja treningowa")]
        public int TrainingSessionId { get; set; }
        public TrainingSession TrainingSession { get; set; }

        [Display(Name = "Obciążenie [kg]")]
        public int Weight { get; set; }

        [Display(Name = "Liczba serii")]
        public int Sets { get; set; }

        [Display(Name = "Powtórzenia w serii")]
        public int RepsPerSet { get; set; }

        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
    }
}
