using Domain.Enums;

namespace API.DTOs
{
    public class CreateNotificationDto
    {
        public string Recipient { get; set; }
        public string Message { get; set; }
        public DateTimeOffset ScheduledAt { get; set; }
        public SendPriority Priority { get; set; }
    }
}
