namespace EntityModel
{
    public class UserAuthModel
    {
        public string? Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

    }
}
