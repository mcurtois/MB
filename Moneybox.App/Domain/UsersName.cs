namespace Moneybox.App
{
    public struct UsersName
    {
        private UsersName(string value)
        {
            Value = value;
        }

        public readonly string Value;

        public static UsersName New(string value) =>
            new UsersName(value); // add validation
    }
}
