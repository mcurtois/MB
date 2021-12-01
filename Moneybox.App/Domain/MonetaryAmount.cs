namespace Moneybox.App
{
    public struct MonetaryAmount
    {
        private MonetaryAmount(decimal value)
        {
            Value = value;
        }

        public readonly decimal Value;

        public static MonetaryAmount New(decimal value) =>
            new MonetaryAmount(value);

        public static readonly MonetaryAmount Zero =
            New(0m);

        public static bool operator !=(MonetaryAmount left, MonetaryAmount right) =>
            left.Value != right.Value;

        public static bool operator ==(MonetaryAmount left, MonetaryAmount right) =>
            left.Value == right.Value;

        public static bool operator >(MonetaryAmount left, MonetaryAmount right) =>
            left.Value > right.Value;

        public static bool operator <(MonetaryAmount left, MonetaryAmount right) =>
            left.Value < right.Value;

        public static bool operator >=(MonetaryAmount left, MonetaryAmount right) =>
            left.Value >= right.Value;

        public static bool operator <=(MonetaryAmount left, MonetaryAmount right) =>
            left.Value <= right.Value;

        public static MonetaryAmount operator +(MonetaryAmount left, MonetaryAmount right) =>
            New(left.Value + right.Value);

        public static MonetaryAmount operator -(MonetaryAmount left, MonetaryAmount right) =>
            New(left.Value - right.Value);

        public override bool Equals(object obj) =>
            (obj is MonetaryAmount ma) && ma == this;

        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
