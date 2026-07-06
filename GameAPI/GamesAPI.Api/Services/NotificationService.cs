
using GamesAPI.Api.Data;
using Microsoft.EntityFrameworkCore;
using GamesAPI.Api.Exceptions;
using GamesAPI.Api.Models;
public class NotificationService
    : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationResponse>>
    GetMyNotificationsAsync(
        Guid userId)
    {
        return await _context.Notifications
            .Where(x =>
                x.UserId == userId)
            .OrderByDescending(x =>
                x.CreatedAt)
            .Select(x =>
                new NotificationResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                })
            .ToListAsync();
    }

    public async Task<int>
        GetUnreadCountAsync(
            Guid userId)
    {
        return await _context.Notifications
            .CountAsync(x =>
                x.UserId == userId &&
                !x.IsRead);
    }

    public async Task MarkAsReadAsync(
        Guid notificationId,
        Guid userId)
    {
        var notification =
            await _context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.Id == notificationId &&
                    x.UserId == userId);

        if (notification == null)
        {
            throw new NotFoundException(
                "Notification not found.");
        }

        notification.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(
        Guid userId)
    {
        var notifications =
            await _context.Notifications
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsRead)
                .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }


    public async Task CreateNotificationAsync(
    Guid userId,
    string title,
    string message,
    string type,
    Guid? createdByUserId = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.Now,
            CreatedByUserId = createdByUserId
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();
    }
}