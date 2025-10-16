using AntiFraud.System.Application.Services;
using Humanizer;
using MassTransit;
using MediatR;

namespace AntiFraud.System.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        #region Variables
        private readonly ISendEndpointProvider _sendEndpoint;
        #endregion

        #region Constructors
        public NotificationService(ISendEndpointProvider sendEndpoint)
        {
            _sendEndpoint = sendEndpoint ?? throw new ArgumentNullException(nameof(sendEndpoint));
        }
        #endregion

        #region Public Methods/Operators
        public async Task PublishAsync<TNotification>(TNotification notification)
            where TNotification : INotification
        {
            ArgumentNullException.ThrowIfNull(notification, nameof(notification));

            var endpoint = await _sendEndpoint.GetSendEndpoint(new Uri($"queue:{notification.GetType().Name.Underscore()}"));
            await endpoint.Send(notification).ConfigureAwait(false);
        }
        #endregion
    }
}