using Moneybox.App.Domain.Commands;
using Moneybox.App.Domain.Handlers;
using Moneybox.App.Domain.Services;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Moneybox.App.UnitTests.TestDefaults;

namespace Moneybox.App.UnitTests
{
    public class NotifyAccountApproachingPayInLimitTests
    {
        [Fact]
        public async Task AccountPaidInAboveLimitWarningThreshold()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountApproachingPayInLimit(notif.Object, MonetaryAmount.New(500m));

            await sut.Handle(new MoneyTransferredNotification(
                AccountDefault,
                AccountDefault, 
                TransferMonetaryAmount.Zero,
                AccountDefault,
                AccountDefault with { User = UserDefault with { Email = Email.New("m@c.com") }, PayInLimit = MonetaryAmount.New(1000m), PaidIn = MonetaryAmount.New(501m) }),
                CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyApproachingPayInLimit("m@c.com"), Times.Once);
        }

        [Fact]
        public async Task AccountPaidInBelowLimitWarningThreshold()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountApproachingPayInLimit(notif.Object, MonetaryAmount.New(500m));

            await sut.Handle(new MoneyTransferredNotification(
                AccountDefault,
                AccountDefault,
                TransferMonetaryAmount.Zero,
                AccountDefault,
                AccountDefault with { PayInLimit = MonetaryAmount.New(1000m), PaidIn = MonetaryAmount.New(499m) }), CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AccountPaidInEqualLimitWarningThreshold()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountApproachingPayInLimit(notif.Object, MonetaryAmount.New(500m));

            await sut.Handle(new MoneyTransferredNotification(
                AccountDefault,
                AccountDefault,
                TransferMonetaryAmount.Zero,
                AccountDefault,
                AccountDefault with { PayInLimit = MonetaryAmount.New(1000m), PaidIn = MonetaryAmount.New(500m) }), CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Never);
        }
    }
}
