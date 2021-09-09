using System;

namespace Web.Infrastructure.Model
{
    public class TokenModel
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}