using MediatR;
using Moneybox.App.Domain.Commands;
using Moneybox.App.Domain.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Moneybox.App.Domain.Handlers
{
    public class NotifyAccountFundsLow : INotificationHandler<MoneyTransferredNotification>, INotificationHandler<MoneyWithdrawnNotification>
    {
        private readonly INotificationService notificationService;
        private readonly MonetaryAmount threshold;

        public NotifyAccountFundsLow(
            INotificationService notificationService,
            MonetaryAmount threshold)
        {
            this.notificationService = notificationService;
            this.threshold = threshold;
        }

        public Task Handle(MoneyWithdrawnNotification notification, CancellationToken cancellationToken) =>
            CheckAccount(notification.AfterWithdrawal);

        public Task Handle(MoneyTransferredNotification notification, CancellationToken cancellationToken) =>
            CheckAccount(notification.FromAfterTransfer);

        private Task<Unit> CheckAccount(Account account) =>
            account switch
            {
                Account acc when acc.Balance < threshold => NotifyFundsLow(acc.User.Email),
                _ => Unit.Task
            };

        private Task<Unit> NotifyFundsLow(Email email)
        {
            notificationService.NotifyFundsLow(email.Value);
            return Unit.Task;
        }
    }
}
