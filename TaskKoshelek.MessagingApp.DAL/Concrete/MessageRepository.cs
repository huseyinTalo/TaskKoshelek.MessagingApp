using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskKoshelek.MessagingApp.Core.Entities;
using TaskKoshelek.MessagingApp.Core.Interfaces;
using TaskKoshelek.MessagingApp.Core.Utilities;

namespace TaskKoshelek.MessagingApp.DAL.Concrete
{
    /// <summary>
    /// Repository class responsible for handling database operations related to messages.
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRepository"/> class with the specified database connection string.
        /// </summary>
        /// <param name="options">Configuration parameters containing the database connection string.</param>
        /// <exception cref="InvalidOperationException">Thrown when the connection string is not configured properly.</exception>
        public MessageRepository(IOptions<DatabaseOptions> options)
        {
            _connectionString = options.Value.ConnectionString;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }
        }

        /// <summary>
        /// Asynchronously adds a new message to the database.
        /// </summary>
        /// <param name="message">The message entity to be added to the database.</param>
        /// <returns>A task representing the asynchronous operation, returning the newly added <see cref="Message"/> entity.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while inserting the message into the database.</exception>
        public async Task<Message> AddAsync(Message message)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string sql = @"
                INSERT INTO Messages (Body, OrderNumber)
                OUTPUT 
                    INSERTED.Id,
                    INSERTED.Body,
                    INSERTED.OrderNumber,
                    INSERTED.CreatedTime
                VALUES (@Body, @OrderNumber);";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Body", message.Body);
                    command.Parameters.AddWithValue("@OrderNumber", message.OrderNumber);

                    try
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                message.Id = reader.GetGuid(0);
                                message.Body = reader.GetString(1);
                                message.OrderNumber = reader.GetInt32(2);
                                message.CreatedTime = reader.GetDateTime(3);
                            }
                        }

                        return message;
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"Error inserting message into database. Connection string might be invalid. Error: {ex.Message}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves all messages from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning a collection of <see cref="Message"/> entities.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching messages from the database.</exception>
        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            var messages = new List<Message>();

            using (var connection = new SqlConnection(_connectionString))
            {
                const string sql = @"
                    SELECT [Id], [Body], [OrderNumber], [CreatedTime]
                    FROM Messages
                    ORDER BY [CreatedTime] DESC";

                using (var command = new SqlCommand(sql, connection))
                {
                    try
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var message = new Message
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    Body = reader.GetString(reader.GetOrdinal("Body")),
                                    OrderNumber = reader.GetInt32(reader.GetOrdinal("OrderNumber")),
                                    CreatedTime = reader.GetDateTime(reader.GetOrdinal("CreatedTime"))
                                };
                                messages.Add(message);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"Error retrieving messages from database. Connection string might be invalid. Error: {ex.Message}", ex);
                    }
                }
            }

            return messages;
        }

        /// <summary>
        /// Asynchronously retrieves all messages which were created within the last 10 minutes from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning a collection of <see cref="Message"/> entities.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching messages from the database.</exception>
        public async Task<IEnumerable<Message>> GetAllLast10Min()
        {
            var messages = new List<Message>();

            using (var connection = new SqlConnection(_connectionString))
            {
                const string sql = @"
                    SELECT [Id], [Body], [OrderNumber], [CreatedTime]
                    FROM Messages
                    WHERE [CreatedTime] >= DATEADD(MINUTE, -10, GETUTCDATE())
                    ORDER BY [CreatedTime] DESC";

                using (var command = new SqlCommand(sql, connection))
                {
                    try
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var message = new Message
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    Body = reader.GetString(reader.GetOrdinal("Body")),
                                    OrderNumber = reader.GetInt32(reader.GetOrdinal("OrderNumber")),
                                    CreatedTime = reader.GetDateTime(reader.GetOrdinal("CreatedTime"))
                                };
                                messages.Add(message);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"Error retrieving messages from database. Connection string might be invalid. Error: {ex.Message}", ex);
                    }
                }
            }

            return messages;
        }
    }
}
