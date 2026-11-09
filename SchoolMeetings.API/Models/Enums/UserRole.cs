namespace SchoolMeetings.API.Models.Enums
{
    /// <summary>
    /// Ruoli disponibili per gli utenti del sistema
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Amministratore della scuola - può gestire docenti, slot e pulizia dati
        /// </summary>
        Admin = 0,

        /// <summary>
        /// Docente - può gestire le proprie disponibilità e prenotazioni
        /// </summary>
        Teacher = 1,

        /// <summary>
        /// Genitore - può prenotare colloqui per i propri figli
        /// </summary>
        Parent = 2
    }
}