using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskKoshelek.MessagingApp.Core.Entities;

namespace TaskKoshelek.MessagingApp.Core.Interfaces
{
    public interface IMessageRepository
    {
        /// <summary>
        /// Asynchronously adds a new message to the database.
        /// </summary>
        /// <param name="message">The message entity to be added to the database.</param>
        /// <returns>A task representing the asynchronous operation, returning the newly added <see cref="Message"/> entity.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while inserting the message into the database.</exception>
        public Task<Message> AddAsync(Message message);
        /// <summary>
        /// Asynchronously retrieves all messages from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning a collection of <see cref="Message"/> entities.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching messages from the database.</exception>
        public Task<IEnumerable<Message>> GetAllAsync();
        /// <summary>
        /// Asynchronously retrieves all messages which were created within the last 10 minutes from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning a collection of <see cref="Message"/> entities.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching messages from the database.</exception>
        public Task<IEnumerable<Message>> GetAllLast10Min();
    }
}
