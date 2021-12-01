using Shouldly;
using System;
using Xunit;

namespace Moneybox.App.UnitTests
{
    public class TransferMonetaryAmountTests
    {
        [Fact]
        public void TryNewAmountZero() =>
            TransferMonetaryAmount.TryNew(MonetaryAmount.Zero).Value.ShouldBe(MonetaryAmount.Zero);

        [Fact]
        public void TryNewAmountPositive() =>
            TransferMonetaryAmount.TryNew(MonetaryAmount.New(1m)).Value.ShouldBe(MonetaryAmount.New(1m));

        [Fact]
        public void TryNewAmountNegative() =>
            Should.Throw<Exception>(() => TransferMonetaryAmount.TryNew(MonetaryAmount.New(-1m)));
    }
}
