using API.DTOs;
using AutoMapper;
using Domain.Entities;

namespace API.Mapping
{
    public class NotificationMapping : Profile
    {
        public NotificationMapping() 
        {
            CreateMap<CreateNotificationDto, Notification>();
            CreateMap<ModifyNotificationDto, Notification>();
        }
    }
}
