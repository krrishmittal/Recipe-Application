namespace Recipe.Infrastructure.Handlers
{
    /// <summary>
    /// Provides helpers for hashing and verifying passwords.
    /// </summary>
    public class PasswordHashing
    {
        /// <summary>
        /// Executes the hash p operation.
        /// </summary>
        public static string HashP(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        /// <summary>
        /// Executes the verify p operation.
        /// </summary>
        public static bool VerifyP(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
