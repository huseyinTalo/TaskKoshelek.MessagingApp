using TaskKoshelek.MessagingApp.API.APIDtos.MessageDTOs;

namespace TaskKoshelek.MessagingApp.API.APIServicves.Abstracts
{
    public interface IMessageService
    {
        /// <summary>
        /// Method for creating a new message. Converts DTO to entity and saves it in the repository.
        /// Returns DTO of the created message.
        /// </summary>
        /// <param name="messageCreateDTO">DTO for creating a new message.</param>
        /// <returns>DTO of the created message.</returns>
        public Task<MessageDTO> MessageCreate(MessageCreateDTO messageCreateDTO);
        /// <summary>
        /// Method for getting all messages. Converts entities to DTOs and returns a list of messages.
        /// If there are no messages, returns an empty list.
        /// </summary>
        /// <returns>List of DTOs for all messages.</returns>
        public Task<IEnumerable<MessageDTO>> MessageGetAll();
        /// <summary>
        /// Method for getting all messages which were created within the last 10 minutes. Converts entities to DTOs and returns a list of messages.
        /// If there are no messages, returns an empty list.
        /// </summary>
        /// <returns>List of DTOs for all messages.</returns>
        public Task<IEnumerable<MessageDTO>> MessageGetLast10Min();
    }
}
