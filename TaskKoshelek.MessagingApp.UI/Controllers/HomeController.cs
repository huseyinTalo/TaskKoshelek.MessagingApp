using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using TaskKoshelek.MessagingApp.UI.Models.MessageVMs;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl = "http://api:9098/api/messages";

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging errors and events.</param>
    /// <param name="httpClientFactory">Factory for creating HTTP client instances.</param>
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Retrieves all messages in order to remember the last order number if exist.
    /// </summary>
    /// <returns>A view for creating a message and viewing all the messages which created in the last 10 minutes.</returns>
    public async Task<IActionResult> Index()
    {
        try
        {
            var result = await _httpClient.GetAsync($"{_apiBaseUrl}/all");
            if(!result.IsSuccessStatusCode)
            {
                return View(new MessageViewModel());
            }
            var data = await result.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var messages = JsonSerializer.Deserialize<List<MessageViewModel>>(data, options);
            if(messages.Count <= 0)
            {
                return View(new MessageViewModel());
            }
            var message = messages.OrderByDescending(x => x.CreatedTime).First();
            return View(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while listing the messages");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Sends a message to the API.
    /// </summary>
    /// <param name="message">The message to be sent.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] MessageViewModel message)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { body = message.Body, orderNumber = message.OrderNumber }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/create", content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Sending message failed");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the message");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Retrieves messages sent in the last 10 minutes.
    /// </summary>
    /// <returns>A list of recent messages in JSON format.</returns>
    [HttpGet]
    public async Task<IActionResult> GetRecentMessages()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/all10minutes");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Messages could not be listed");
            }

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var messages = JsonSerializer.Deserialize<List<MessageViewModel>>(content, options);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while listing the messages");
            return StatusCode(500, "An error occurred");
        }
    }

    /// <summary>
    /// Retrieves the latest message's order number.
    /// </summary>
    /// <returns>The order number of the latest message.</returns>
    [HttpGet]
    public async Task<IActionResult> GetLatestOrderNumber()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/latest-order-number");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Could not retrieve the latest order number");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(new { LatestOrderNumber = content });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the latest order number");
            return StatusCode(500, "An error occurred");
        }
    }

}
