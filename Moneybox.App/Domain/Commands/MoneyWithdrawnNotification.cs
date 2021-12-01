using MediatR;

namespace Moneybox.App.Domain.Commands
{
    public record MoneyWithdrawnNotification(
        Account BeforeWithdrawal,
        TransferMonetaryAmount AmountWithdrawn,
        Account AfterWithdrawal) : INotification;
}
