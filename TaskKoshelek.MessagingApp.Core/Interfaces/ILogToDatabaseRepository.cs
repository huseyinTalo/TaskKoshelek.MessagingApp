using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskKoshelek.MessagingApp.Core.Interfaces
{
    public interface ILogToDatabaseRepository
    {
        /// <summary>
        /// Logs operation details to the separate telemetry database
        /// </summary>
        Task LogOperationToDatabase(string operation, string status, string details);
        /// <summary>
        /// Starts timing an operation
        /// </summary>
        void StartOperationTimer(string operationName);
    }
}
