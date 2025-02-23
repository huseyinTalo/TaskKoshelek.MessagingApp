using AutoMapper;
using TaskKoshelek.MessagingApp.API.APIDtos.MessageDTOs;
using TaskKoshelek.MessagingApp.API.APIServicves.Abstracts;
using TaskKoshelek.MessagingApp.Core.Entities;
using TaskKoshelek.MessagingApp.Core.Interfaces;

namespace TaskKoshelek.MessagingApp.API.APIServicves.Concretes
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _autoMapper;

        /// <summary>
        /// MessageService class constructor. Initializes the message repository and auto-mapper.
        /// </summary>
        /// <param name="messageRepository">Repository for working with messages.</param>
        /// <param name="autoMapper">Auto-mapper for object transformation.</param>
        public MessageService(IMessageRepository messageRepository, IMapper autoMapper)
        {
            _messageRepository = messageRepository;
            _autoMapper = autoMapper;
        }

        /// <summary>
        /// Method for creating a new message. Converts DTO to entity and saves it in the repository.
        /// Returns DTO of the created message.
        /// </summary>
        /// <param name="messageCreateDTO">DTO for creating a new message.</param>
        /// <returns>DTO of the created message.</returns>
        public async Task<MessageDTO> MessageCreate(MessageCreateDTO messageCreateDTO)
        {
            var result = await _messageRepository.AddAsync(_autoMapper.Map<MessageCreateDTO, Message>(messageCreateDTO));
            if (result is null)
            {
                return _autoMapper.Map<Message, MessageDTO>(result);
            }
            return _autoMapper.Map<Message, MessageDTO>(result);
        }

        /// <summary>
        /// Method for getting all messages. Converts entities to DTOs and returns a list of messages.
        /// If there are no messages, returns an empty list.
        /// </summary>
        /// <returns>List of DTOs for all messages.</returns>
        public async Task<IEnumerable<MessageDTO>> MessageGetAll()
        {
            var result = await _messageRepository.GetAllAsync();
            if (result.Count() <= 0)
            {
                return _autoMapper.Map<IEnumerable<Message>, IEnumerable<MessageDTO>>(result);
            }
            return _autoMapper.Map<IEnumerable<Message>, IEnumerable<MessageDTO>>(result);
        }
        /// <summary>
        /// Method for getting all messages which were created within the last 10 minutes. Converts entities to DTOs and returns a list of messages.
        /// If there are no messages, returns an empty list.
        /// </summary>
        /// <returns>List of DTOs for all messages.</returns>
        public async Task<IEnumerable<MessageDTO>> MessageGetLast10Min()
        {
            var result = await _messageRepository.GetAllLast10Min();
            if (result.Count() <= 0)
            {
                return _autoMapper.Map<IEnumerable<Message>, IEnumerable<MessageDTO>>(result);
            }
            return _autoMapper.Map<IEnumerable<Message>, IEnumerable<MessageDTO>>(result);
        }
    }
}
