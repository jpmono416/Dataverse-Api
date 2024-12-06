using Dataverse_api.Entities;
using Dataverse_api.Service;
using Dataverse_api.View;
using Microsoft.Xrm.Sdk;

namespace Dataverse_api.Application;

public static class Demo
{

    /// <summary>
    /// Runs a demo of creating, reading, updating, and deleting Account, Contact, and Case entities, with logging and cleanup.
    /// </summary>
    public static void RunDemo()
    {
        // Create, Read, Update Account, Contact, Case
        var accountId = PerformDemoAccountOperations();
        Logger.LogEntity(EarlyBoundDataverseApiService.GetEntityById<Account>(accountId));
        
        var contactId = PerformDemoContactOperations(accountId);
        Logger.LogEntity(EarlyBoundDataverseApiService.GetEntityById<Contact>(contactId));
        
        var incidentId = PerformDemoCaseOperations(accountId, contactId);
        Logger.LogEntity(EarlyBoundDataverseApiService.GetEntityById<Incident>(incidentId));

        // Delete all entities
        PerformDemoCleanup(incidentId, contactId, accountId);
        Logger.Log(Constants.Messages.CleanupMessage);
    }

    /// <summary>
    /// Creates, retrieves, and updates a demo Account entity.
    /// </summary>
    /// <returns>The Guid of the Account created</returns>
    private static Guid PerformDemoAccountOperations()
    {
        // Create
        var accountId = EarlyBoundDataverseApiService.CreateEntity(Constants.DemoData.DemoAccount);
        
        // Retrieve
        var retrievedAccount = EarlyBoundDataverseApiService.GetEntityById<Account>(accountId);
        
        // Update
        EarlyBoundDataverseApiService.UpdateEntity( retrievedAccount,
            EarlyBoundDataverseApiService.UpdateAccountAction);
        
        return accountId;
    }

    /// <summary>
    /// Creates, retrieves, and updates a demo Contact entity.
    /// </summary>
    /// <param name="accountId">The Guid of the account associated with the Contact</param>
    /// <returns>The Guid of the Contact created</returns>
    private static Guid PerformDemoContactOperations(Guid accountId)
    {
        // Set ParentCustomerId to the new contact
        Constants.DemoData.DemoContact.ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountId);
        
        var contactId = EarlyBoundDataverseApiService.CreateEntity(Constants.DemoData.DemoContact);
        var retrievedContact = EarlyBoundDataverseApiService.GetEntityById<Contact>(contactId);
        EarlyBoundDataverseApiService.UpdateEntity( retrievedContact, EarlyBoundDataverseApiService.UpdateContactAction);
        
        return contactId;
    }

    /// <summary>
    /// Creates, retrieves, and updates a demo Case entity.
    /// </summary>
    /// <param name="accountId">The Guid of the Account associated with the case</param>
    /// <param name="contactId">The Guid of the Contact associated with the case</param>
    /// <returns>The Guid of the Case created</returns>
    private static Guid PerformDemoCaseOperations(Guid accountId, Guid contactId)
    {
        // Set CustomerId and PrimaryContactId to the new case
        Constants.DemoData.DemoCase.CustomerId = new EntityReference(Account.EntityLogicalName, accountId);
        Constants.DemoData.DemoCase.PrimaryContactId = new EntityReference(Contact.EntityLogicalName, contactId);
        
        var incidentId = EarlyBoundDataverseApiService.CreateEntity(Constants.DemoData.DemoCase);
        var retrievedIncident = EarlyBoundDataverseApiService.GetEntityById<Incident>(incidentId);
        EarlyBoundDataverseApiService.UpdateEntity( retrievedIncident, EarlyBoundDataverseApiService.UpdateCaseAction);

        return incidentId;
    }

    /// <summary>
    /// Deletes the demo entities in the reverse order of their creation to avoid issues with relationships.
    /// </summary>
    /// <param name="incidentId">The Guid of the Case to be deleted</param>
    /// <param name="contactId">The Guid of the Contact to be deleted</param>
    /// <param name="accountId">The Guid of the Account to be deleted</param>
    private static void PerformDemoCleanup(Guid incidentId, Guid contactId, Guid accountId)
    {
        EarlyBoundDataverseApiService.DeleteEntity<Incident>(incidentId);
        EarlyBoundDataverseApiService.DeleteEntity<Contact>(contactId);
        EarlyBoundDataverseApiService.DeleteEntity<Account>(accountId);
    }
}