namespace Web.Infrastructure.Model
{
    public class JwtSetting
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecurityKey { get; set; }
        public int Expires { get; set; }
    }
}