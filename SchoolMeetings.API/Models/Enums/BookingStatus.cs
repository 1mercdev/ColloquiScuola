namespace SchoolMeetings.API.Models.Enums
{
    /// <summary>
    /// Stati possibili per una prenotazione
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Prenotazione confermata e attiva
        /// </summary>
        Confirmed = 0,

        /// <summary>
        /// Prenotazione cancellata (da genitore o docente)
        /// </summary>
        Cancelled = 1,

        /// <summary>
        /// Colloquio completato (passato)
        /// </summary>
        Completed = 2
    }
}