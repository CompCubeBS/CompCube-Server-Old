using System.Security.Cryptography;
using System.Text;
using CompCube_Server.Data.Schema;

namespace CompCube_Server.Data;

public class AuthData(IServiceScope scopeService, ILogger<AuthData> logger)
{
    public void AddOAuthState(string state, string returnTo, AuthState.ResponseModeType responseMode)
    {
        try
        {
            using var scope = scopeService.ServiceProvider.CreateScope();

            var context = scope.ServiceProvider.GetService<DataContext>()!;

            using var transaction = context.Database.BeginTransaction();

            var oauthState = new AuthState()
            {
                StateHash = Hash(state),
                ReturnTo = returnTo,
                ResponseMode = responseMode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedAt = DateTime.UtcNow
            };

            context.AuthStates.Add(oauthState);

            context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to add oauth state {state}");
            throw;
        }
    }

    private static string Hash(string value)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(value);
        byte[] hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}