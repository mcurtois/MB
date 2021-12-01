using MediatR;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Commands;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository accountRepository;
        private readonly IMediator mediator;

        public WithdrawMoney(
            IAccountRepository accountRepository,
            IMediator mediator)
        {
            this.accountRepository = accountRepository;
            this.mediator = mediator;
        }

        public void Execute(AccountId fromAccountId, TransferMonetaryAmount amount)
        {
            var account = accountRepository.GetAccountById(fromAccountId.Value);
            var updatedAccount = account.Withdraw(amount).ThrowIfBalanceIsNegative(new InvalidOperationException("Insufficient funds to make withdrawal"));
            accountRepository.Update(updatedAccount);

            mediator.Publish(new MoneyWithdrawnNotification(account, amount, updatedAccount));
        }
    }
}
