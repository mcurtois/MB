using System;

namespace Moneybox.App
{
    public struct UserId
    {
        private UserId(Guid value)
        {
            Value = value;
        }

        public readonly Guid Value;

        public static UserId New(Guid value) =>
            new UserId(value);

        public static UserId Create() =>
            New(Guid.NewGuid());
    }
}
