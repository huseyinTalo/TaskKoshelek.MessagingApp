using System.ComponentModel.DataAnnotations;

namespace TaskKoshelek.MessagingApp.API.APIDtos.MessageDTOs
{
    /// <summary>
    /// Data transfer object for creating a new message
    /// </summary>
    public class MessageCreateDTO
    {
        /// <summary>
        /// The content of the message
        /// </summary>
        /// <example>Hello, this is a test message</example>
        [Required(ErrorMessage = "Message content is required.")]
        [StringLength(128, ErrorMessage = "Message cannot exceed 128 characters.")]
        public string Body { get; set; }

        /// <summary>
        /// The order number associated with the message
        /// </summary>
        /// <example>12345</example>
        [Required(ErrorMessage = "Order number is required.")]
        public int OrderNumber { get; set; }
    }
}