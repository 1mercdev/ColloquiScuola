namespace SchoolMeetings.API.Models.Enums
{
    /// <summary>
    /// Stati possibili per uno slot temporale
    /// </summary>
    public enum TimeSlotStatus
    {
        /// <summary>
        /// Slot disponibile per prenotazioni
        /// </summary>
        Available = 0,

        /// <summary>
        /// Slot prenotato da un genitore
        /// </summary>
        Booked = 1,

        /// <summary>
        /// Slot bloccato dal docente (non disponibile)
        /// </summary>
        Blocked = 2
    }
}