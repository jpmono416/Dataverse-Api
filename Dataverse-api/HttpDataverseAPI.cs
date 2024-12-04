using System.Net.Http.Headers;

namespace Dataverse_api;
/// <summary>
/// Class to interact with the Dataverse API for managing Accounts, Contacts, and Cases.
/// Implemented using HttpClient - not currently in use, it was an initial version of the app.
/// As a result, ot all functionality is implemented.
/// </summary>
public class HttpDataverseAPI
{
    private readonly string _apiUrl;
    private readonly string _authToken;

    /// <summary>
    /// Creates a new Account entity in Dataverse.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for the API call.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="email">The email address associated with the account.</param>
    /// <param name="phone">The phone number associated with the account.</param>
    /// <returns>The ID of the created account.</returns>
    private async Task<string> CreateAccount(HttpClient httpClient, string name, string email, string phone)
    {
        var url = $"{_apiUrl}accounts";
        var payload = new
        {
            name = name,
            emailaddress1 = email,
            telephone1 = phone
        };
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = Utils.CreateJsonContent(payload)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken); // Ensure the token is added
        var response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        // Extract ID from response headers
        return response.Headers.Location?.ToString().Split('(')[1].TrimEnd(')') ?? string.Empty;
    }

    /// <summary>
    /// Retrieves an Account entity from Dataverse by its ID.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for the API call.</param>
    /// <param name="accountId">The ID of the account to retrieve.</param>
    /// <returns>The JSON string representing the retrieved account.</returns>
    private async Task<string> GetAccountById(HttpClient httpClient, string accountId)
    {
        var url = $"{_apiUrl}accounts({accountId})";

        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            // Log details if there's an error
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
            throw new HttpRequestException($"Failed to retrieve account. Status code: {response.StatusCode}");
        }

        var responseBody = response.Content.ReadAsStringAsync().Result;
        return responseBody;
    }

    /// <summary>
    /// Creates a new Contact entity in Dataverse.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for the API call.</param>
    /// <param name="firstName">The first name of the contact.</param>
    /// <param name="lastName">The last name of the contact.</param>
    /// <param name="email">The email address associated with the contact.</param>
    /// <param name="accountId">The ID of the associated account.</param>
    /// <returns>The ID of the created contact.</returns>
    private async Task<string> CreateContact(HttpClient httpClient, string firstName, string lastName, string email, string accountId)
    {
        var url = $"{_apiUrl}contacts";
        var payload = new Dictionary<string, object>
        {
            { "firstname", firstName },
            { "lastname", lastName },
            { "emailaddress1", email },
            { "parentcustomerid_account@odata.bind", $"/accounts({accountId})" }
        };
        var response = await httpClient.PostAsync(url, Utils.CreateJsonContent(payload));
        response.EnsureSuccessStatusCode();

        // Extract ID from response headers
        return response.Headers.Location.ToString().Split('(')[1].TrimEnd(')');
    }

    /// <summary>
    /// Retrieves a Contact entity from Dataverse by its ID.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for the API call.</param>
    /// <param name="contactId">The ID of the contact to retrieve.</param>
    /// <returns>The JSON string representing the retrieved contact.</returns>
    private async Task<string> GetContactById(HttpClient httpClient, string contactId)
    {
        var url = $"{_apiUrl}contacts({contactId})";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }

    /// <summary>
    /// Creates a new Case (Incident) entity in Dataverse.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for the API call.</param>
    /// <param name="title">The title of the case.</param>
    /// <param name="description">The description of the case.</param>
    /// <param name="accountId">The ID of the associated account.</param>
    /// <param name="contactId">The ID of the associated contact.</param>
    /// <returns>The ID of the created case.</returns>
    private async Task<string> CreateCase(HttpClient httpClient, string title, string description, string accountId, string contactId)
    {
        var url = $"{_apiUrl}incidents";
        var payload = new Dictionary<string, object>
        {
            { "title", title },
            { "description", description },
            { "customerid_account@odata.bind", $"/accounts({accountId})" },
            { "responsiblecontactid@odata.bind", $"/contacts({contactId})" }
        };
        var response = await httpClient.PostAsync(url, Utils.CreateJsonContent(payload));
        response.EnsureSuccessStatusCode();

        // Extract ID from response headers
        return response.Headers.Location.ToString().Split('(')[1].TrimEnd(')');
    }

    /// <summary>
    /// Retrieves a Case (Incident) entity from Dataverse by its ID.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for the API call.</param>
    /// <param name="caseId">The ID of the case to retrieve.</param>
    /// <returns>The JSON string representing the retrieved case.</returns>
    private async Task<string> GetCaseById(HttpClient httpClient, string caseId)
    {
        var url = $"{_apiUrl}incidents({caseId})";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}
