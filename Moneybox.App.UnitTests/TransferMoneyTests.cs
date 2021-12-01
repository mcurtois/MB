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
    public class TransferMoneyTests
    {
        [Fact]
        public void AmountWithdrawnFromCorrectAccount()
        {
            var fromAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(100m) }; 
            var toAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(70m) };
            var repository = new Mock<IAccountRepository>();
            repository.Setup(x => x.GetAccountById(fromAccount.Id.Value)).Returns(fromAccount);
            repository.Setup(x => x.GetAccountById(toAccount.Id.Value)).Returns(toAccount);

            var sut = new TransferMoney(repository.Object, new Mock<IMediator>().Object);

            sut.Execute(fromAccount.Id, toAccount.Id, TransferMonetaryAmount.TryNew(MonetaryAmount.New(10m)));

            repository.Verify(x => x.Update(It.Is<Account>(a => a.Id.Value == fromAccount.Id.Value && a.Balance == MonetaryAmount.New(90m))), Times.Once);
        }

        [Fact]
        public void AmountPaidInToCorrectAccount()
        {
            var fromAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(100m) };
            var toAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(70m) };
            var repository = new Mock<IAccountRepository>();
            repository.Setup(x => x.GetAccountById(fromAccount.Id.Value)).Returns(fromAccount);
            repository.Setup(x => x.GetAccountById(toAccount.Id.Value)).Returns(toAccount);

            var sut = new TransferMoney(repository.Object, new Mock<IMediator>().Object);

            sut.Execute(fromAccount.Id, toAccount.Id, TransferMonetaryAmount.TryNew(MonetaryAmount.New(10m)));

            repository.Verify(x => x.Update(It.Is<Account>(a => a.Id.Value == toAccount.Id.Value && a.Balance == MonetaryAmount.New(80m))), Times.Once);
        }

        [Fact]
        public void TransferLeavesFromAccountBalanceNegative()
        {
            var fromAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(100m) };
            var toAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(70m) };
            var repository = new Mock<IAccountRepository>();
            repository.Setup(x => x.GetAccountById(fromAccount.Id.Value)).Returns(fromAccount);
            repository.Setup(x => x.GetAccountById(toAccount.Id.Value)).Returns(toAccount);

            var sut = new TransferMoney(repository.Object, new Mock<IMediator>().Object);

            Should.Throw<InvalidOperationException>(() => sut.Execute(fromAccount.Id, toAccount.Id, TransferMonetaryAmount.TryNew(MonetaryAmount.New(110m))));
            repository.Verify(x => x.Update(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void TransferLeavesToAccountPaidInLimitExceeded()
        {
            var fromAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(100m) };
            var toAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(90m), Balance = MonetaryAmount.New(70m) };
            var repository = new Mock<IAccountRepository>();
            repository.Setup(x => x.GetAccountById(fromAccount.Id.Value)).Returns(fromAccount);
            repository.Setup(x => x.GetAccountById(toAccount.Id.Value)).Returns(toAccount);

            var sut = new TransferMoney(repository.Object, new Mock<IMediator>().Object);

            Should.Throw<InvalidOperationException>(() => sut.Execute(fromAccount.Id, toAccount.Id, TransferMonetaryAmount.TryNew(MonetaryAmount.New(100m))));
            repository.Verify(x => x.Update(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void NotificationPublished()
        {
            var fromAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(100m) };
            var toAccount = AccountDefault with { PayInLimit = MonetaryAmount.New(100m), Balance = MonetaryAmount.New(70m) };
            var repository = new Mock<IAccountRepository>();
            repository.Setup(x => x.GetAccountById(fromAccount.Id.Value)).Returns(fromAccount);
            repository.Setup(x => x.GetAccountById(toAccount.Id.Value)).Returns(toAccount);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Publish(It.IsAny<MoneyTransferredNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var sut = new TransferMoney(repository.Object, mediator.Object);

            sut.Execute(fromAccount.Id, toAccount.Id, TransferMonetaryAmount.TryNew(MonetaryAmount.New(10m)));

            mediator.Verify(x => x.Publish(It.Is<MoneyTransferredNotification>(n =>
                n.FromBeforeTransfer.Balance == MonetaryAmount.New(100m) &&
                n.ToBeforeTransfer.Balance == MonetaryAmount.New(70m) &&
                n.AmountTransferred.Value == MonetaryAmount.New(10m) &&
                n.FromAfterTransfer.Balance == MonetaryAmount.New(90m) &&
                n.ToAfterTransfer.Balance == MonetaryAmount.New(80m)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
