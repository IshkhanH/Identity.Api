namespace Identity.Core.Configurations.Options
{
    public class JWTConfigurationOptions
    {
        public string ValidIssuer {  get; set; }
        public string ValidAudience { get; set; }
        public string Key { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public int LifeTime { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public bool EnableRefreshToken { get; set; } = true;

    }
}
