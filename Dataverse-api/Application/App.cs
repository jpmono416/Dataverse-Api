using Dataverse_api.Entities;
using Dataverse_api.Service;
using Dataverse_api.Util;
using Dataverse_api.View;
using Microsoft.Xrm.Sdk;

namespace Dataverse_api.Application;

public static class App
{
    public static void Run()
    {
        Logger.Log(Constants.Messages.WelcomeMessage);
        try
        {
            // Run demo
            Demo.RunDemo();
            
            if (!InputManager.StartManualInput()) return;
            
            // Run manual input
            MainAppLoop();
        }
        catch (Exception e) { Logger.LogError(e); }
    }

    private static void MainAppLoop()
    {
        Logger.Log(Constants.Messages.HelpMessage);
        while (true)
        {
            var command = InputManager.ListenForCommand();
            ProcessCommand(command);
        }
        // ReSharper disable once FunctionNeverReturns : The 'exit' command will close the app and terminate this loop
    }
    
    private static void ProcessCommand(Command command)
    {
        switch (command.Action)
        {
            case Constants.Actions.Help:
                Logger.Log(Constants.Messages.HelpMessage);
                break;

            case Constants.Actions.Cleanup:
                Logger.Log(Constants.Messages.CleanupMessage);
                CleanupEntities();
                break;

            case Constants.Actions.Exit:
                Logger.Log(Constants.Messages.GoodbyeMessage);
                CleanupEntities();
                Environment.Exit(0);
                break;

            case Constants.Actions.Create:
                if (command.EntityType != null)
                {
                    var entity = (Entity)Activator.CreateInstance(command.EntityType)!;
                    Logger.Log($"Creating a new {command.EntityType.Name}...");
                    var populatedEntity = InputManager.PromptEntityProperties(entity);
                    EarlyBoundDataverseApiService.CreateEntity(populatedEntity);
                    Logger.Log($"{command.EntityType.Name} created successfully.");
                }
                break;

            case Constants.Actions.Get:
                if (command.Id.HasValue) GetEntityById(command); 
                else GetAllEntities(command); 
                break;

            case Constants.Actions.Update:
                if (command is { EntityType: not null, Id: not null })
                {
                    var existingEntity = GetEntityById(command);
                    Logger.Log($"Updating {command.EntityType.Name} with ID {command.Id.Value}...");
                    var updatedEntity = InputManager.PromptEntityProperties(existingEntity, isUpdate: true);
                    EarlyBoundDataverseApiService.UpdateEntity(updatedEntity, e =>
                    {
                        foreach (var property in updatedEntity.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite))
                        {
                            property.SetValue(e, property.GetValue(updatedEntity));
                        }
                    });
                    Logger.Log($"{command.EntityType.Name} updated successfully.");
                }
                else Logger.Log(Constants.Messages.InvalidCommand); 
                break;

            case Constants.Actions.Delete:
                if (command.Id.HasValue) DeleteEntity(command); 
                else Logger.Log(Constants.Messages.CommandRequiresId);
                
                break;

            default:
                Logger.Log(Constants.Messages.InvalidCommand);
                break;
        }
    }

    private static void CleanupEntities()
    {
        foreach (var entityType in Constants.EntityNames.EntityMapping.Values)
        {
            Logger.Log($"Cleaning up all {entityType.Name}s...");
            //EarlyBoundDataverseApiService.DeleteAllEntities(entityType);
            Logger.Log($"All {entityType.Name}s deleted.");
        }
    }

    /*
     * TECH_DEBT: The code below is a 'hacked' solution to bridge between the Service and the App classes.
     * It is a temporary solution to allow the application to run and demonstrate the functionality due to time constraints.
     * A better implementation can be achieved either by refactoring the service to a different design pattern without
     * templating or by refactoring the application to use the templating effectively considering user input.
     */

    private static Entity? GetEntityById(Command command)
    {
        if (command.EntityType == typeof(Account))
        {
            var account = EarlyBoundDataverseApiService.GetEntityById<Account>(command.Id.Value);
            Logger.LogEntity(account);
            return account;
        }
        if (command.EntityType == typeof(Contact))
        {
            var contact = EarlyBoundDataverseApiService.GetEntityById<Contact>(command.Id.Value);
            Logger.LogEntity(contact);
            return contact;
        }
        if (command.EntityType == typeof(Incident))
        {
            var incident = EarlyBoundDataverseApiService.GetEntityById<Incident>(command.Id.Value);
            Logger.LogEntity(incident);
            return incident;
        }
        Logger.Log("Unsupported entity type for GetEntityById.");
        return null;
    }

    private static void DeleteEntity(Command command)
    {
        if (command.EntityType == typeof(Account))
        {
            EarlyBoundDataverseApiService.DeleteEntity<Account>(command.Id.Value);
            Logger.Log("Account deleted successfully.");
        }
        else if (command.EntityType == typeof(Contact))
        {
            EarlyBoundDataverseApiService.DeleteEntity<Contact>(command.Id.Value);
            Logger.Log("Contact deleted successfully.");
        }
        else if (command.EntityType == typeof(Incident))
        {
            EarlyBoundDataverseApiService.DeleteEntity<Incident>(command.Id.Value);
            Logger.Log("Incident deleted successfully.");
        }
        else
        {
            Logger.Log("Unsupported entity type for DeleteEntity.");
        }
    }

    
    private static void GetAllEntities(Command command)
    {
        if (command.EntityType == typeof(Account))
        {
            var accounts = EarlyBoundDataverseApiService.GetAllEntities<Account>();
            accounts.ForEach(Logger.LogEntity);
        }
        else if (command.EntityType == typeof(Contact))
        {
            var contacts = EarlyBoundDataverseApiService.GetAllEntities<Contact>();
            contacts.ForEach(Logger.LogEntity);
        }
        else if (command.EntityType == typeof(Incident))
        {
            var incidents = EarlyBoundDataverseApiService.GetAllEntities<Incident>();
            incidents.ForEach(Logger.LogEntity);
        }
        else
        {
            Logger.Log("Unsupported entity type for GetAllEntities.");
        }
    }

}