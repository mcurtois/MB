using MediatR;

namespace Moneybox.App.Domain.Commands
{
    public record MoneyTransferredNotification(
        Account FromBeforeTransfer,
        Account ToBeforeTransfer,
        TransferMonetaryAmount AmountTransferred,
        Account FromAfterTransfer,
        Account ToAfterTransfer) : INotification;
}
