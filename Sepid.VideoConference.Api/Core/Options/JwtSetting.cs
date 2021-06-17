namespace Sepid.VideoConference.Api.Core.Options
{
    public class JwtSetting
    {
        public string SecretKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
    }
}