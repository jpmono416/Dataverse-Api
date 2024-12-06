using Dataverse_api.Entities;
using Dataverse_api.Service;
using Microsoft.Xrm.Sdk;

namespace Dataverse_api.Application;

public static class App
{
    public static void Run()
    {
        try
        {
            // TODO log IDs as they are created
            // Create, Read, Update Account, Contact, Case
            var accountId = PerformDemoAccountOperations();
            var contactId = PerformDemoContactOperations(accountId);
            var incidentId = PerformDemoCaseOperations(accountId, contactId);

            PerformDemoCleanup(incidentId, contactId, accountId);
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    private static Guid PerformDemoAccountOperations()
    {
        var account = new Account
        {
            Name = "Acme Corporation",
            EMailAddress1 = "contact@acme.com",
            Telephone1 = "123-456-7890"
        };
        // Create
        var accountId = EarlyBoundDataverseApiService.CreateEntity(account);
        
        // Retrieve
        var retrievedAccount = EarlyBoundDataverseApiService.GetEntityById<Account>(accountId);
        
        // Update
        EarlyBoundDataverseApiService.UpdateEntity( retrievedAccount,
            EarlyBoundDataverseApiService.UpdateAccountAction);
        
        return accountId;
    }

    private static Guid PerformDemoContactOperations(Guid accountId)
    {
        var contact = new Contact
        {
            FirstName = "John",
            LastName = "Doe",
            EMailAddress1 = "john.doe@acme.com",
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountId)
        };
        var contactId = EarlyBoundDataverseApiService.CreateEntity(contact);
        var retrievedContact = EarlyBoundDataverseApiService.GetEntityById<Contact>(contactId);
        EarlyBoundDataverseApiService.UpdateEntity( retrievedContact, EarlyBoundDataverseApiService.UpdateContactAction);
        
        return contactId;
    }

    private static Guid PerformDemoCaseOperations(Guid accountId, Guid contactId)
    {
        var incident = new Incident
        {
            Title = "Billing Issue",
            Description = "Customer disputes invoice amount.",
            CustomerId = new EntityReference(Account.EntityLogicalName, accountId),
            PrimaryContactId = new EntityReference(Contact.EntityLogicalName, contactId)
        };
        var incidentId = EarlyBoundDataverseApiService.CreateEntity(incident);
        var retrievedIncident = EarlyBoundDataverseApiService.GetEntityById<Incident>(incidentId);
        EarlyBoundDataverseApiService.UpdateEntity( retrievedIncident, EarlyBoundDataverseApiService.UpdateCaseAction);

        return incidentId;
    }
    
    private static void PerformDemoCleanup(Guid incidentId, Guid contactId, Guid accountId)
    {
        EarlyBoundDataverseApiService.DeleteEntity<Incident>(incidentId);
        EarlyBoundDataverseApiService.DeleteEntity<Contact>(contactId);
        EarlyBoundDataverseApiService.DeleteEntity<Account>(accountId);
        
        Console.WriteLine("All entities cleaned up.");
    }
}