using API.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        public NotificationController(IMapper mapper, INotificationRepository notificationRepository)
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto createNotificationDto)
        {
            var notification = _mapper.Map<Notification>(createNotificationDto);
            await _notificationRepository.AddNotificationAsync(notification);
            return CreatedAtAction(nameof(GetNotificationById), new { id = notification.Id }, notification);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _notificationRepository.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] ModifyNotificationDto dto)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(id);

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            if (dto.Message != null)
            {
                notification.Message = dto.Message;
            }

            if (dto.Recipient != null)
            {
                notification.Recipient = dto.Recipient;
            }

            if (dto.ScheduledAt.HasValue)
            {
                var newScheduledAt = dto.ScheduledAt.Value;

                if (newScheduledAt < DateTime.UtcNow)
                {
                    return BadRequest("Scheduled time cannot be in the past.");
                }

                

                notification.ScheduledAt = newScheduledAt;
            }

            if (dto.Priority.HasValue)
            {
                notification.Priority = dto.Priority.Value;
            }

            await _notificationRepository.UpdateNotificationAsync(notification);

            return NoContent(); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(id);

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            await _notificationRepository.DeleteNotificationAsync(notification.Id);

            return NoContent();
        }
    }
}
