namespace AngularApp1.Server.Entities
{
    public class ProfileCredentials
    {
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public ProfileType ProfileType { get; set; }
    }
}
