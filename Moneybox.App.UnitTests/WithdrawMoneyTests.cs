using MediatR;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Commands;
using Moneybox.App.Features;
using Moq;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Moneybox.App.UnitTests.TestDefaults;

namespace Moneybox.App.UnitTests
{
    public class WithdrawMoneyTests
    {
        [Fact]
        public void AmountWithdrawnFromAccount()
        {
            var account = AccountDefault with { Balance = MonetaryAmount.New(100m), Withdrawn = MonetaryAmount.New(100m) };
            var repository = new Mock<IAccountRepository>();
            repository.Setup(x => x.GetAccountById(account.Id.Value)).Returns(account);

            var sut = new WithdrawMoney(repository.Object, new Mock<IMediator>().Object);

            sut.Execute(account.Id, TransferMonetaryAmount.TryNew(MonetaryAmount.New(10m)));

            repository.Verify(x => x.Update(It.Is<Account>(a => a.Id.Value == account.Id.Value && a.Balance == MonetaryAmount.New(90m) && a.Withdrawn == MonetaryAmount.New(90m))), Times.Once);
        }

        [Fact]
        public void WithdrawalLeavesAccountBalanceNegative()
        {
            var account = AccountDefault with { Balance = MonetaryAmount.New(100m) };
            var repository = new Mock<IAccountRepository>();
            repository.Setup(x => x.GetAccountById(account.Id.Value)).Returns(account);

            var sut = new WithdrawMoney(repository.Object, new Mock<IMediator>().Object);

            Should.Throw<InvalidOperationException>(() => sut.Execute(account.Id, TransferMonetaryAmount.TryNew(MonetaryAmount.New(110m))));
            repository.Verify(x => x.Update(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void NotificationPublished()
        {
            var account = AccountDefault with { Balance = MonetaryAmount.New(100m) };
            var repository = new Mock<IAccountRepository>();
            repository.Setup(x => x.GetAccountById(account.Id.Value)).Returns(account);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Publish(It.IsAny<MoneyWithdrawnNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var sut = new WithdrawMoney(repository.Object, mediator.Object);

            sut.Execute(account.Id, TransferMonetaryAmount.TryNew(MonetaryAmount.New(10m)));

            mediator.Verify(x => x.Publish(It.Is<MoneyWithdrawnNotification>(n =>
                n.BeforeWithdrawal.Balance == MonetaryAmount.New(100m) &&
                n.AmountWithdrawn.Value == MonetaryAmount.New(10m) &&
                n.AfterWithdrawal.Balance == MonetaryAmount.New(90m)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
