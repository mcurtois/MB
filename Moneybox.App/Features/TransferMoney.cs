using MediatR;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Commands;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository accountRepository;
        private readonly IMediator mediator;

        public TransferMoney(
            IAccountRepository accountRepository,
            IMediator mediator)
        {
            this.accountRepository = accountRepository;
            this.mediator = mediator;
        }

        public void Execute(AccountId fromAccountId, AccountId toAccountId, TransferMonetaryAmount amount)
        {
            var from = accountRepository.GetAccountById(fromAccountId.Value);
            var to = accountRepository.GetAccountById(toAccountId.Value);
            var updatedFrom = from.Withdraw(amount).ThrowIfBalanceIsNegative(new InvalidOperationException("Insufficient funds to make transfer"));
            var updatedTo = to.PayIn(amount).ThrowIfPaidInHasExceededLimit(new InvalidOperationException("Account pay in limit reached"));

            accountRepository.Update(updatedFrom);
            accountRepository.Update(updatedTo);

            mediator.Publish(new MoneyTransferredNotification(from, to, amount, updatedFrom, updatedTo));
        }
    }
}
