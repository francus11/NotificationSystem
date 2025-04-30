using Domain.Interfaces;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NotificationSchedulerService;
using NotificationSchedulerService.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=notifications.db"));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddHostedService<SchedulerWorker>();

var host = builder.Build();
host.Run();
