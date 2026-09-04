
using ApiSalonyar.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(
            IServiceProvider services,
            ILogger<NotificationBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCreateNotifications();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in NotificationService");
                }

                // هر ۵ دقیقه چک کن
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task CheckAndCreateNotifications()
        {
            _logger.LogInformation("🔔 Checking notifications at {time}", DateTime.Now);

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();

            var now = DateTime.Now;
            var targetTime = now.AddMinutes(15);

            // پیدا کردن رزروهایی که ۱۵ دقیقه دیگه شروع میشن
            var reservations = await context.RoomReservations
                .Include(x => x.Patient)
                .Include(x => x.Staff)
                .Include(x => x.Treatment)
                .Where(x =>
                    !x.IsDeleted &&
                    x.ReservationStatusId == 2 && // فقط تایید شده
                    x.ReservationDate.Year == now.Year &&
                    x.ReservationDate.Month == now.Month &&
                    x.ReservationDate.Day == now.Day)
                .ToListAsync();

            foreach (var res in reservations)
            {
                // تبدیل StartTime به DateTime
                var startDateTime = new DateTime(
                    res.ReservationDate.Year,
                    res.ReservationDate.Month,
                    res.ReservationDate.Day,
                    res.StartTime.Hours,
                    res.StartTime.Minutes,
                    0);

                var diff = (startDateTime - now).TotalMinutes;

                // فقط بین ۱۳ تا ۱۶ دقیقه مانده
                  if (diff < 1 || diff > 16) continue;
              //  if (diff < -30 || diff > 60) continue;
                // چک کن اعلان قبلاً ساخته نشده باشه
                var exists = await context.Notifications.AnyAsync(n =>
                    n.ReservationId == res.ReservationId &&
                    n.Type == "REMINDER_15MIN");

                if (exists) continue;

                // ساخت اعلان
                var patientName = res.Patient != null
                    ? $"{res.Patient.FirstName} {res.Patient.LastName}"
                    : "بیمار";

                context.Notifications.Add(new Notification
                {
                    StaffId = res.StaffId,
                    ReservationId = res.ReservationId,
                    Title = $"رزرو {patientName} ۱۵ دقیقه دیگر",
                    Body = $"بیمار {patientName} ساعت {res.StartTime:hh\\:mm} رزرو دارد. آیا حاضر است؟",
                    Type = "REMINDER_15MIN",
                    Priority = "HIGH",
                    ActionRequired = true,
                    ActionType = "CONFIRM_ARRIVAL",
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = startDateTime.AddMinutes(30),
                });
            }

            await context.SaveChangesAsync();
        }
    }
}