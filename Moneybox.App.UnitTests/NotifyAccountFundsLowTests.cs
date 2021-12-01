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
    public class NotifyAccountFundsLowTests
    {
        [Fact]
        public async Task AccountBalanceBelowThresholdAfterTransfer()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountFundsLow(notif.Object, MonetaryAmount.New(100m));

            await sut.Handle(new MoneyTransferredNotification(
                AccountDefault,
                AccountDefault,
                TransferMonetaryAmount.Zero,
                AccountDefault with { User = UserDefault with { Email = Email.New("m@c.com") }, Balance = MonetaryAmount.New(99m) },
                AccountDefault), CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyFundsLow("m@c.com"), Times.Once);
        }

        [Fact]
        public async Task AccountBalanceBelowThresholdAfterWithdrawal()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountFundsLow(notif.Object, MonetaryAmount.New(100m));

            await sut.Handle(new MoneyWithdrawnNotification(
                AccountDefault,
                TransferMonetaryAmount.Zero,
                AccountDefault with { User = UserDefault with { Email = Email.New("m@c.com") }, Balance = MonetaryAmount.New(99m) }), CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyFundsLow("m@c.com"), Times.Once);
        }

        [Fact]
        public async Task AccountBalanceAboveThresholdAfterTransfer()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountFundsLow(notif.Object, MonetaryAmount.New(100m));

            await sut.Handle(new MoneyTransferredNotification(
                AccountDefault,
                AccountDefault,
                TransferMonetaryAmount.Zero,
                AccountDefault with { Balance = MonetaryAmount.New(101m) },
                AccountDefault), CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyFundsLow(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AccountBalanceAboveThresholdAfterWithdrawal()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountFundsLow(notif.Object, MonetaryAmount.New(100m));

            await sut.Handle(new MoneyWithdrawnNotification(
                AccountDefault,
                TransferMonetaryAmount.Zero,
                AccountDefault with { Balance = MonetaryAmount.New(101m) }), CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyFundsLow(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AccountBalanceEqualThresholdAfterTransfer()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountFundsLow(notif.Object, MonetaryAmount.New(100m));

            await sut.Handle(new MoneyTransferredNotification(
                AccountDefault,
                AccountDefault,
                TransferMonetaryAmount.Zero,
                AccountDefault with { Balance = MonetaryAmount.New(100m) },
                AccountDefault), CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyFundsLow(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AccountBalanceEqualThresholdAfterWithdrawal()
        {
            var notif = new Mock<INotificationService>();

            var sut = new NotifyAccountFundsLow(notif.Object, MonetaryAmount.New(100m));

            await sut.Handle(new MoneyWithdrawnNotification(
                AccountDefault,
                TransferMonetaryAmount.Zero,
                AccountDefault with { Balance = MonetaryAmount.New(100m) }), CancellationToken.None).ConfigureAwait(false);

            notif.Verify(n => n.NotifyFundsLow(It.IsAny<string>()), Times.Never);
        }
    }
}
