using System;

namespace Moneybox.App
{
    public struct TransferMonetaryAmount
    {
        private TransferMonetaryAmount(MonetaryAmount value)
        {
            Value = value;
        }

        public readonly MonetaryAmount Value;

        public static TransferMonetaryAmount TryNew(MonetaryAmount amount) =>
            amount switch
            {
                MonetaryAmount ma when ma < MonetaryAmount.Zero => throw new Exception("Transfer monetary amount can not be negative"),
                MonetaryAmount ma => new TransferMonetaryAmount(ma)
            };

        public static TransferMonetaryAmount Zero =>
            new TransferMonetaryAmount(MonetaryAmount.Zero);
    }
}
