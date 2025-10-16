using MediatR;

namespace AntiFraud.System.Application.Services
{
    public interface INotificationService
    {
        #region Public Methods/Operators   
        Task PublishAsync<TNotification>(TNotification notification)
            where TNotification : INotification;
        #endregion
    }
}