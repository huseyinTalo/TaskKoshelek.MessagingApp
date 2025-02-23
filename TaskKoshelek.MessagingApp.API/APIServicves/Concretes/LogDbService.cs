using TaskKoshelek.MessagingApp.API.APIServicves.Abstracts;
using TaskKoshelek.MessagingApp.Core.Interfaces;

namespace TaskKoshelek.MessagingApp.API.APIServicves.Concretes
{
    public class LogDbService : ILogDbService
    {
        private readonly ILogToDatabaseRepository _logToDatabaseRepository;

        /// <summary>
        /// Constructor for LogDbService class. Initializes the database logging repository.
        /// </summary>
        /// <param name="logToDatabaseRepository">Repository for database logging operations.</param>

        public LogDbService(ILogToDatabaseRepository logToDatabaseRepository)
        {
            _logToDatabaseRepository = logToDatabaseRepository;
        }

        /// <summary>
        /// Starts a timer for tracking the duration of a specified operation.
        /// </summary>
        /// <param name="operationName">Name of the operation to be timed.</param>
        public void StartOperationTimer(string operationName)
        {
            _logToDatabaseRepository.StartOperationTimer(operationName);
        }

        /// <summary>
        /// Logs operation details to the database asynchronously.
        /// </summary>
        /// <param name="operation">Name of the operation being logged.</param>
        /// <param name="status">Current status of the operation.</param>
        /// <param name="details">Additional details or information about the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LogOperationToDB(string operation, string status, string details)
        {
            await _logToDatabaseRepository.LogOperationToDatabase(operation, status, details);
        }

    }
}
