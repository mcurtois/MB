using System;

namespace Moneybox.App
{
    public record Account(
        AccountId Id,
        User User,
        MonetaryAmount PayInLimit,
        MonetaryAmount Balance,
        MonetaryAmount Withdrawn,
        MonetaryAmount PaidIn);

    public static class AccountExtensions
    {
        public static Account PayIn(this Account account, TransferMonetaryAmount transferAmount) =>
            account with
            {
                Balance = account.Balance + transferAmount.Value,
                PaidIn = account.PaidIn + transferAmount.Value
            };

        public static Account Withdraw(this Account account, TransferMonetaryAmount transferAmount) =>
            account with
            {
                Balance = account.Balance - transferAmount.Value,
                Withdrawn = account.Withdrawn - transferAmount.Value
            };

        public static Account ThrowIfBalanceIsNegative(this Account account, Exception exception) =>
            account switch
            {
                Account acc when acc.Balance < MonetaryAmount.Zero => throw exception,
                Account acc => acc
            };

        public static Account ThrowIfPaidInHasExceededLimit(this Account account, Exception exception) =>
            account switch
            {
                Account acc when acc.PaidIn > acc.PayInLimit => throw exception,
                Account acc => acc
            };
    }
}
