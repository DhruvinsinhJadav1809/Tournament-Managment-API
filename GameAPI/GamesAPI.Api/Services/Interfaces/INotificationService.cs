public interface INotificationService
{
    Task<List<NotificationResponse>>
        GetMyNotificationsAsync(Guid userId);

    Task<int>
        GetUnreadCountAsync(Guid userId);

    Task MarkAsReadAsync(
        Guid notificationId,
        Guid userId);

    Task MarkAllAsReadAsync(Guid userId);
    Task CreateNotificationAsync(
    Guid userId,
    string title,
    string message,
    string type,
    Guid? createdByUserId = null);
}