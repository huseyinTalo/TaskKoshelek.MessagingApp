namespace TaskKoshelek.MessagingApp.API.APIDtos.MessageDTOs
{
    /// <summary>
    /// Data transfer object representing a message
    /// </summary>
    public class MessageDTO
    {
        /// <summary>
        /// Unique identifier of the message
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; set; }

        /// <summary>
        /// The content of the message
        /// </summary>
        /// <example>Hello, this is a test message</example>
        public string Body { get; set; }

        /// <summary>
        /// The order number associated with the message
        /// </summary>
        /// <example>12345</example>
        public int OrderNumber { get; set; }

        /// <summary>
        /// The timestamp when the message was created
        /// </summary>
        /// <example>2025-02-22T17:43:58.570Z</example>
        public DateTime CreatedTime { get; set; }
    }
}
