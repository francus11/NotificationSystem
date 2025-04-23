using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTimeOffset ScheduledAt { get; set; }
        public SendPriority Priority { get; set; }
        public SendStatus Status { get; set; }
    }
}
