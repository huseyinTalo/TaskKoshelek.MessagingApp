using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskKoshelek.MessagingApp.Core.Entities;
using TaskKoshelek.MessagingApp.API.APIServicves.Abstracts;
using System.Diagnostics;
using OpenTelemetry.Trace;
using TaskKoshelek.MessagingApp.API.APIDtos.MessageDTOs;

namespace TaskKoshelek.MessagingApp.API.Controllers
{
    /// <summary>
    /// Controller for managing messages in the system
    /// </summary>
    [Route("api/messages")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogDbService _logDbService;
        private static readonly ActivitySource _activitySource = new ActivitySource("TaskKoshelek.MessagingApp.API");

        /// <summary>
        /// Constructor for the message controller
        /// </summary>
        /// <param name="mapper">Service for object mapping</param>
        /// <param name="messageService">Service for working with messages</param>
        /// <param name="logDbService">Service for logging to the database</param>
        public MessageController(IMessageService messageService, ILogDbService logDbService)
        {
            _messageService = messageService;
            _logDbService = logDbService;
        }

        /// <summary>
        /// Handles the creation of a new message.
        /// </summary>
        /// <param name="messageCreateDTO">The message data transfer object containing message details.</param>
        /// <returns>Returns a success response if the message is created successfully, otherwise returns an error response.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> AddMessage([FromBody] MessageCreateDTO messageCreateDTO)
        {
            using var activity = _activitySource.StartActivity("AddMessage", ActivityKind.Server);
            activity?.SetTag("endpoint", "POST api/messages/create");
            activity?.SetTag("order_number", messageCreateDTO.OrderNumber);

            // Start timing the operation
            _logDbService.StartOperationTimer("AddMessage");

            // Validate the model if needed (ModelState validation happens automatically with [ApiController])
            if (!ModelState.IsValid)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Validation error");
                activity?.SetTag("validation_failed", true);

                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Log validation errors
                await _logDbService.LogOperationToDB("AddMessage", "ValidationError",
                    $"Validation failed: {string.Join(", ", errors)}");

                return BadRequest(new { Status = "Error", Errors = errors });
            }

            try
            {
                activity?.AddEvent(new ActivityEvent("Starting to create message"));
                activity?.SetTag("message_length", messageCreateDTO.Body?.Length ?? 0);

                // Use message service to create message
                await _messageService.MessageCreate(messageCreateDTO);

                activity?.AddEvent(new ActivityEvent("Message created successfully"));

                // Log the operation to SQL Server with truncated body content
                string truncatedBody = messageCreateDTO.Body;
                if (!string.IsNullOrEmpty(truncatedBody) && truncatedBody.Length > 50)
                {
                    truncatedBody = truncatedBody.Substring(0, 50) + "...";
                }

                await _logDbService.LogOperationToDB("AddMessage", "Success",
                    $"Body: {truncatedBody}, OrderNumber: {messageCreateDTO.OrderNumber}");

                return Ok(new { Status = "Success", Message = "Message created successfully" });
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.RecordException(ex);

                await _logDbService.LogOperationToDB("AddMessage", "Error", ex.Message);

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a list of messages from the last 10 minutes relative to the most recent message
        /// </summary>
        /// <returns>List of messages from the last 10 minutes</returns>
        /// <response code="200">Returns the list of messages</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("all10minutes")]
        public async Task<IActionResult> GetMessagesLast10Minutes()
        {
            using var activity = _activitySource.StartActivity("GetMessagesLast10Minutes", ActivityKind.Server);
            activity?.SetTag("endpoint", "GET api/messages/all10minutes");

            // Start timing the operation
            _logDbService.StartOperationTimer("GetMessagesLast10Minutes");

            try
            {
                activity?.AddEvent(new ActivityEvent("Starting to retrieve messages"));

                // Use message service to get all messages
                var messages = await _messageService.MessageGetLast10Min();

                activity?.AddEvent(new ActivityEvent("Retrieved all messages from service"));

                // Check if messages exist
                if (!messages.Any())
                {
                    activity?.SetTag("message_count", 0);
                    await _logDbService.LogOperationToDB("GetMessagesLast10Minutes", "Success", "No messages found");
                    return Ok(new List<Message>());
                }                

                // Log the operation to SQL Server
                await _logDbService.LogOperationToDB("GetMessagesLast10Minutes", "Success", messages.Count().ToString());

                return Ok(messages);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.RecordException(ex);

                await _logDbService.LogOperationToDB("GetMessagesLast10Minutes", "Error", ex.Message);

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a list of messages
        /// </summary>
        /// <returns>List of messages</returns>
        /// <response code="200">Returns the list of messages</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("all")]
        public async Task<IActionResult> GetMessages()
        {
            using var activity = _activitySource.StartActivity("GetMessages", ActivityKind.Server);
            activity?.SetTag("endpoint", "GET api/messages/all");

            // Start timing the operation
            _logDbService.StartOperationTimer("GetMessages");

            try
            {
                activity?.AddEvent(new ActivityEvent("Starting to retrieve messages"));

                // Use message service to get all messages
                var messages = await _messageService.MessageGetAll();

                activity?.AddEvent(new ActivityEvent("Retrieved all messages from service"));

                // Check if messages exist
                if (!messages.Any())
                {
                    activity?.SetTag("message_count", 0);
                    await _logDbService.LogOperationToDB("GetMessages", "Success", "No messages found");
                    return Ok(new List<Message>());
                }

                // Find latest message
                var latestMessage = messages.OrderByDescending(m => m.CreatedTime).First();

                activity?.SetTag("latest_message_time", latestMessage.CreatedTime.ToString("o"));
                activity?.SetTag("message_count", messages.Count());
                activity?.AddEvent(new ActivityEvent("Messages retrieved successfully"));

                // Log the operation to SQL Server
                await _logDbService.LogOperationToDB("GetMessages", "Success", messages.Count().ToString());

                return Ok(messages);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.RecordException(ex);

                await _logDbService.LogOperationToDB("GetMessages", "Error", ex.Message);

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the order number of the latest message
        /// </summary>
        /// <returns>Order number of the latest message</returns>
        /// <response code="200">Returns the order number of the latest message</response>
        /// <response code="404">No messages found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("latest-order-number")]
        public async Task<IActionResult> GetLatestMessageOrderNumber()
        {
            using var activity = _activitySource.StartActivity("GetLatestMessageOrderNumber", ActivityKind.Server);
            activity?.SetTag("endpoint", "GET api/messages/latest-order-number");

            // Start timing the operation
            _logDbService.StartOperationTimer("GetLatestMessageOrderNumber");

            try
            {
                activity?.AddEvent(new ActivityEvent("Starting to retrieve messages"));

                // Use message service to get all messages
                var messages = await _messageService.MessageGetAll();

                activity?.AddEvent(new ActivityEvent("Retrieved all messages from service"));

                // Check if messages exist
                if (!messages.Any())
                {
                    activity?.SetTag("message_count", 0);
                    await _logDbService.LogOperationToDB("GetLatestMessageOrderNumber", "Success", "No messages found returned 0 as default");
                    return Ok(0);
                }

                // Find latest message
                var latestMessage = messages.OrderByDescending(m => m.CreatedTime).First();

                activity?.SetTag("latest_message_time", latestMessage.CreatedTime.ToString("o"));
                activity?.SetTag("latest_order_number", latestMessage.OrderNumber);
                activity?.AddEvent(new ActivityEvent("Latest message order number retrieved successfully"));

                // Log the operation to SQL Server
                await _logDbService.LogOperationToDB("GetLatestMessageOrderNumber", "Success", latestMessage.OrderNumber.ToString());

                return Ok(latestMessage.OrderNumber);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.RecordException(ex);

                await _logDbService.LogOperationToDB("GetLatestMessageOrderNumber", "Error", ex.Message);

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
