namespace Server.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(int userId, string email, string name);
}