namespace Moneybox.App
{
    public struct Email
    {
        private Email(string value)
        {
            Value = value;
        }

        public readonly string Value;

        public static Email New(string value) =>
            new Email(value); // add validation
    }
}
