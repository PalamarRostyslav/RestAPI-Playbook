namespace Identity.Api
{
    public class JwtSettings
    {
        public string TokenSecret { get; set; } = string.Empty;
        public int TokenLifetimeHours { get; set; } = 8;
    }
}
