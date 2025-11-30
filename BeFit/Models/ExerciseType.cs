using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nazwa ćwiczenia")]
        public string Name { get; set; }
    }
}
