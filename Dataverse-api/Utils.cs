using System.Text;
using System.Text.Json;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace Dataverse_api;

/// <summary>
/// Utility class containing helper methods for interacting with Dataverse and handling common tasks such as authentication, environment variable loading, and JSON manipulation.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Represents the structure of an OAuth 2.0 authentication response.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// The type of token returned (e.g., "Bearer").
        /// </summary>
        public string TokenType { get; init; }

        /// <summary>
        /// The number of seconds before the token expires.
        /// </summary>
        public int ExpiresIn { get; init; }

        /// <summary>
        /// The number of seconds before the token's extended expiration.
        /// </summary>
        public int ExtExpiresIn { get; init; }

        /// <summary>
        /// The access token to use for authentication.
        /// </summary>
        public string AccessToken { get; init; }
    }

    /// <summary>
    /// Retrieves an environment variable by name, throwing an exception if it is not found.
    /// </summary>
    /// <param name="varName">The name of the environment variable to retrieve.</param>
    /// <returns>The value of the environment variable.</returns>
    public static string GetFromEnv(string varName) =>
        Environment.GetEnvironmentVariable(varName)
        ?? throw new InvalidOperationException($"Missing required environment variable: {varName}");

    /// <summary>
    /// Loads environment variables from a .env file located in the application's base directory.
    /// </summary>
    public static void LoadEnvVariables()
    {
        var envFilePath = Path.Combine(AppContext.BaseDirectory, ".env");

        if (File.Exists(envFilePath))
        {
            DotNetEnv.Env.Load(envFilePath);
            Console.WriteLine("Environment variables loaded successfully!");
        }
        else
        {
            Console.WriteLine($"Error: .env file not found at {envFilePath}");
        }
    }

    /// <summary>
    /// Retrieves an OAuth 2.0 access token using the password grant flow.
    /// </summary>
    /// <param name="tenantId">The tenant ID for the Azure Active Directory.</param>
    /// <param name="clientId">The client (application) ID.</param>
    /// <param name="clientSecret">The client secret for the application.</param>
    /// <param name="username">The username of the account.</param>
    /// <param name="password">The password of the account.</param>
    /// <param name="scope">The scope of access requested.</param>
    /// <returns>The access token as a string.</returns>
    public static async Task<string> GetAccessToken(string tenantId, string clientId, string clientSecret, string username, string password, string scope)
    {
        var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
        var client = new HttpClient();
        var payload = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "username", username },
            { "password", password },
            { "scope", scope }
        };

        var response = await client.PostAsync(url, new FormUrlEncodedContent(payload));
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode}, Response: {errorContent}");
            throw new HttpRequestException("Failed to retrieve access token.");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse) 
                            ?? throw new Exception("Failed to retrieve access token.");
        
        return tokenResponse.AccessToken;
    }

    /// <summary>
    /// Creates a JSON-formatted HTTP content object from a payload.
    /// </summary>
    /// <param name="payload">The object to serialize into JSON.</param>
    /// <returns>An <see cref="StringContent"/> object containing the JSON payload.</returns>
    public static StringContent CreateJsonContent(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Establishes a connection to the Dataverse environment and retrieves an organization service.
    /// </summary>
    /// <param name="connectionString">The connection string used to authenticate and connect to Dataverse.</param>
    /// <returns>An <see cref="IOrganizationService"/> instance for interacting with Dataverse.</returns>
    public static IOrganizationService GetOrganizationService(string connectionString)
    {
        var service = new ServiceClient(connectionString);
        if (!service.IsReady)
        {
            throw new Exception("Failed to connect to Dataverse.");
        }
        return service;
    }
}
