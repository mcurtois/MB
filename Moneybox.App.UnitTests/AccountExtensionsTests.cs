using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;
using static Moneybox.App.UnitTests.TestDefaults;

namespace Moneybox.App.UnitTests
{
    public class AccountExtensionsTests
    {
        public static IEnumerable<object[]> PayInValues =>
            new List<object[]>
            {
                new object[] { 0m, 10m, 10m },
                new object[] { -10m, 10m, 0m },
                new object[] { 10m, 0m, 10m },
                new object[] { 10m, 10m, 20m },
            };

        public static IEnumerable<object[]> WithdrawValues =>
            new List<object[]>
            {
                new object[] { 0m, 10m, -10m },
                new object[] { -10m, 10m, -20m },
                new object[] { 10m, 0m, 10m },
                new object[] { 10m, 10m, 0m },
            };

        [Theory]
        [MemberData(nameof(PayInValues))]
        public void PayInModifiesBalance(decimal initialBalance, decimal amount, decimal expectedBalance)
        {
            new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.New(100),
                Balance: MonetaryAmount.New(initialBalance),
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.Zero).
                PayIn(TransferMonetaryAmount.TryNew(MonetaryAmount.New(amount))).
                Balance.
                ShouldBe(MonetaryAmount.New(expectedBalance));
        }

        [Theory]
        [MemberData(nameof(PayInValues))]
        public void PayInModifiesPaidIn(decimal initialPaidIn, decimal amount, decimal expectedPaidIn)
        {
            new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.New(100),
                Balance: MonetaryAmount.Zero,
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.New(initialPaidIn)).
                PayIn(TransferMonetaryAmount.TryNew(MonetaryAmount.New(amount))).
                PaidIn.
                ShouldBe(MonetaryAmount.New(expectedPaidIn));
        }

        [Theory]
        [MemberData(nameof(PayInValues))]
        public void PayInDoesNotModifyWithdrawn(decimal initialWithdrawn, decimal amount, decimal _)
        {
            new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.New(100),
                Balance: MonetaryAmount.Zero,
                Withdrawn: MonetaryAmount.New(initialWithdrawn),
                PaidIn: MonetaryAmount.Zero).
                PayIn(TransferMonetaryAmount.TryNew(MonetaryAmount.New(amount))).
                Withdrawn.
                ShouldBe(MonetaryAmount.New(initialWithdrawn));
        }

        [Theory]
        [MemberData(nameof(WithdrawValues))]
        public void WithdrawModifiesBalance(decimal initialBalance, decimal amount, decimal expectedBalance)
        {
            new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.New(100),
                Balance: MonetaryAmount.New(initialBalance),
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.Zero).
                Withdraw(TransferMonetaryAmount.TryNew(MonetaryAmount.New(amount))).
                Balance.
                ShouldBe(MonetaryAmount.New(expectedBalance));
        }

        [Theory]
        [MemberData(nameof(WithdrawValues))]
        public void WithdrawModifiesWithdrawn(decimal initialWithdrawn, decimal amount, decimal expectedWithdrawn)
        {
            new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.New(100),
                Balance: MonetaryAmount.Zero,
                Withdrawn: MonetaryAmount.New(initialWithdrawn),
                PaidIn: MonetaryAmount.Zero).
                Withdraw(TransferMonetaryAmount.TryNew(MonetaryAmount.New(amount))).
                Withdrawn.
                ShouldBe(MonetaryAmount.New(expectedWithdrawn));
        }

        [Theory]
        [MemberData(nameof(WithdrawValues))]
        public void WithdrawDoesNotModifyPaidIn(decimal initialPaidIn, decimal amount, decimal _)
        {
            new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.New(100),
                Balance: MonetaryAmount.Zero,
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.New(initialPaidIn)).
                Withdraw(TransferMonetaryAmount.TryNew(MonetaryAmount.New(amount))).
                PaidIn.
                ShouldBe(MonetaryAmount.New(initialPaidIn));
        }

        [Fact]
        public void ThrowIfBalanceNegative_ThrowsWhenNegative()
        {
            Should.Throw<Exception>(() => new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.Zero,
                Balance: MonetaryAmount.New(-1m),
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.Zero).
                ThrowIfBalanceIsNegative(new Exception("")));
        }

        [Fact]
        public void ThrowIfBalanceNegative_DoesNotThrowWhenNotNegative()
        {
            Should.NotThrow(() => new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.Zero,
                Balance: MonetaryAmount.New(1m),
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.Zero).
                ThrowIfBalanceIsNegative(new Exception("")));
        }

        [Fact]
        public void ThrowIfPaidInHasExceededLimit_ThrowsWhenExceeded()
        {
            Should.Throw<Exception>(() => new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.New(100m),
                Balance: MonetaryAmount.New(101m),
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.New(101m)).
                ThrowIfPaidInHasExceededLimit(new Exception("")));
        }

        [Fact]
        public void ThrowIfPaidInHasExceededLimit_DoesNotThrowWhenNotExceeded()
        {
            Should.NotThrow(() => new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.New(100m),
                Balance: MonetaryAmount.New(100m),
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.New(100m)).
                ThrowIfPaidInHasExceededLimit(new Exception("")));
        }
    }
}
