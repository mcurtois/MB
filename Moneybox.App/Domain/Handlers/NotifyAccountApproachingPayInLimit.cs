using MediatR;
using Moneybox.App.Domain.Commands;
using Moneybox.App.Domain.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Moneybox.App.Domain.Handlers
{
    public class NotifyAccountApproachingPayInLimit : INotificationHandler<MoneyTransferredNotification>
    {
        private readonly INotificationService notificationService;
        private readonly MonetaryAmount threshold;

        public NotifyAccountApproachingPayInLimit(
            INotificationService notificationService,
            MonetaryAmount threshold)
        {
            this.notificationService = notificationService;
            this.threshold = threshold;
        }

        public Task Handle(MoneyTransferredNotification notification, CancellationToken cancellationToken) =>
            CheckAccount(notification.ToAfterTransfer);

        private Task<Unit> CheckAccount(Account account) =>
            account switch
            {
                Account acc when acc.PayInLimit - acc.PaidIn < threshold => NotifyApproachingPayInLimit(acc.User.Email),
                _ => Unit.Task
            };

        private Task<Unit> NotifyApproachingPayInLimit(Email email)
        {
            notificationService.NotifyApproachingPayInLimit(email.Value);
            return Unit.Task;
        }
    }
}
