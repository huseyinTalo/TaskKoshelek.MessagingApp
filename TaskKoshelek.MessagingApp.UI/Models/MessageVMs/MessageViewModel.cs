using System.Security.Principal;

namespace TaskKoshelek.MessagingApp.UI.Models.MessageVMs
{
    public class MessageViewModel
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public int OrderNumber { get; set; } = 0;
        public DateTime CreatedTime { get; set; }
    }
}
