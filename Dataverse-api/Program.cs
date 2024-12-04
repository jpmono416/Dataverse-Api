// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using Microsoft.Xrm.Sdk;

namespace Dataverse_api;

internal class Program
{
    private readonly string _authToken;
    private readonly IOrganizationService _service;
    private Program()
    {
        Utils.LoadEnvVariables();
        
        var connectionString = $@"AuthType=ClientSecret;
                        SkipDiscovery=true;url={Utils.GetFromEnv("SCOPE")};
                        Secret={Utils.GetFromEnv("SECRET_ID")};
                        ClientId={Utils.GetFromEnv("APP_ID")};
                        RequireNewInstance=true";
        
        _authToken = Utils.GetAccessToken(Utils.GetFromEnv("TENANT_ID"),
            Utils.GetFromEnv("APP_ID"),
            Utils.GetFromEnv("SECRET_ID"),
            Utils.GetFromEnv("AUTH_EMAIL"),
            Utils.GetFromEnv("AUTH_PASSWORD"),
            Utils.GetFromEnv("SCOPE")).Result;
        
        // print first 16 characters of auth token
        Console.WriteLine($"Auth Token: {_authToken[..16]}...");
        
        _service = Utils.GetOrganizationService(connectionString);
    }

    private static void Main()
    {
        try
        {
            var program = new Program();

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", program._authToken);

            // Create, Read, Update Account
            var account = new Account
            {
                Name = "Acme Corporation",
                EMailAddress1 = "contact@acme.com",
                Telephone1 = "123-456-7890"
            };
            var accountId = EarlyBoundDataverseApi.CreateEntity(program._service, account);
            var retrievedAccount = EarlyBoundDataverseApi.GetEntityById<Account>(program._service, accountId);
            EarlyBoundDataverseApi.UpdateEntity(program._service, retrievedAccount, EarlyBoundDataverseApi.UpdateAccountAction);

            // Create, Read, Update Contact
            var contact = new Contact
            {
                FirstName = "John",
                LastName = "Doe",
                EMailAddress1 = "john.doe@acme.com",
                ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountId)
            };
            var contactId = EarlyBoundDataverseApi.CreateEntity(program._service, contact);
            var retrievedContact = EarlyBoundDataverseApi.GetEntityById<Contact>(program._service, contactId);
            EarlyBoundDataverseApi.UpdateEntity(program._service, retrievedContact, EarlyBoundDataverseApi.UpdateContactAction);

            // Create, Read, Update Case
            var incident = new Incident
            {
                Title = "Billing Issue",
                Description = "Customer disputes invoice amount.",
                CustomerId = new EntityReference(Account.EntityLogicalName, accountId),
                PrimaryContactId = new EntityReference(Contact.EntityLogicalName, contactId)
            };
            var incidentId = EarlyBoundDataverseApi.CreateEntity(program._service, incident);
            var retrievedIncident = EarlyBoundDataverseApi.GetEntityById<Incident>(program._service, incidentId);
            EarlyBoundDataverseApi.UpdateEntity(program._service, retrievedIncident, EarlyBoundDataverseApi.UpdateCaseAction);

            // Cleanup entities in order considering their relationships
            EarlyBoundDataverseApi.DeleteEntity<Incident>(program._service, incidentId);
            EarlyBoundDataverseApi.DeleteEntity<Contact>(program._service, contactId);
            EarlyBoundDataverseApi.DeleteEntity<Account>(program._service, accountId);

            Console.WriteLine("All entities cleaned up.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
