using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Podaj czas rozpoczęcia.")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Podaj czas zakończenia.")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ICollection<PerformedExercise>? PerformedExercises { get; set; }

        public bool IsValid()
        {
            return EndTime > StartTime;
        }
    }
}
