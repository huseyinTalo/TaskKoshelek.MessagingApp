namespace TaskKoshelek.MessagingApp.API.APIServicves.Abstracts
{
    public interface ILogDbService
    {
        /// <summary>
        /// Starts a timer for tracking the duration of a specified operation.
        /// </summary>
        /// <param name="operationName">Name of the operation to be timed.</param>
        Task LogOperationToDB(string operation, string status, string details);

        /// <summary>
        /// Logs operation details to the database asynchronously.
        /// </summary>
        /// <param name="operation">Name of the operation being logged.</param>
        /// <param name="status">Current status of the operation.</param>
        /// <param name="details">Additional details or information about the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        void StartOperationTimer(string operationName);
    }
}
