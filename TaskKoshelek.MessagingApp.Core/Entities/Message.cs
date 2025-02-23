using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskKoshelek.MessagingApp.Core.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public int OrderNumber { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
