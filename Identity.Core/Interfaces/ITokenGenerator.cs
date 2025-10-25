namespace Identity.Core.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(int id, string email);
        string GenerateRefreshToken();
    }
}
