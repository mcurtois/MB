using System;

namespace Moneybox.App
{
    public struct AccountId
    {
        private AccountId(Guid value)
        {
            Value = value;
        }

        public readonly Guid Value;

        public static AccountId New(Guid value) =>
            new AccountId(value);

        public static AccountId Create() =>
            New(Guid.NewGuid());
    }
}
