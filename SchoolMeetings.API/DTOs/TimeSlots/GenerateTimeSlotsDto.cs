using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.TimeSlots
{
    public class GenerateTimeSlotsDto
    {
        [Required(ErrorMessage = "La data è obbligatoria")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "L'orario di inizio è obbligatorio")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "L'orario di fine è obbligatorio")]
        public TimeOnly EndTime { get; set; }

        [Range(5, 60, ErrorMessage = "La durata dello slot deve essere tra 5 e 60 minuti")]
        public int SlotDurationMinutes { get; set; } = 15;
    }
}