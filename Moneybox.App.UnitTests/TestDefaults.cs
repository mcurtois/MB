namespace Moneybox.App.UnitTests
{
    public static class TestDefaults
    {
        public static User UserDefault =>
            new User(UserId.Create(), UsersName.New(""), Email.New(""));

        public static Account AccountDefault =>
            new Account(
                Id: AccountId.Create(),
                User: UserDefault,
                PayInLimit: MonetaryAmount.Zero,
                Balance: MonetaryAmount.Zero,
                Withdrawn: MonetaryAmount.Zero,
                PaidIn: MonetaryAmount.Zero);
    }
}
