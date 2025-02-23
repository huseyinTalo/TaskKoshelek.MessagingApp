using Microsoft.AspNetCore.SignalR;

namespace TaskKoshelek.MessagingApp.UI.Hubs
{
    /// <summary>
    /// Represents a SignalR hub for real-time messaging functionality.
    /// This hub allows clients to send and receive messages in real time.
    /// </summary>
    public class MessageHub : Hub
    {
        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="message">The message to be sent to all clients.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
