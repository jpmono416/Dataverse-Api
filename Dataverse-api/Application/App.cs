using Dataverse_api.Util;
using Dataverse_api.View;

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
        // 
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
            case Constants.Actions.Create:
                Logger.Log($"Creating {command.Entity}...");
                // TODO Call EarlyBoundDataverseApiService.CreateEntity<T> based on entity
                break;

            case Constants.Actions.Get:
                if (!command.Id.HasValue)
                {
                    Logger.Log(Constants.Messages.CommandRequiresId);
                    return;
                }
                Logger.Log($"Retrieving {command.Entity} with ID {command.Id}...");
                // TODO Call EarlyBoundDataverseApiService.GetEntityById<T>

                break;
            
            case Constants.Actions.List:
                Logger.Log($"Retrieving all {command.Entity}s...");
                // TODO Implement logic to retrieve all entities
                break;


            case Constants.Actions.Update:
                if (!command.Id.HasValue)
                {
                    Logger.Log(Constants.Messages.CommandRequiresId);
                    return;
                }
                Logger.Log($"Updating {command.Entity} with ID {command.Id}...");
                // TODO Call EarlyBoundDataverseApiService.UpdateEntity<T> with ID and action
                break;

            case Constants.Actions.Delete:
                if (!command.Id.HasValue)
                {
                    Logger.Log(Constants.Messages.CommandRequiresId);
                    return;
                }
                Logger.Log($"Deleting {command.Entity} with ID {command.Id}...");
                // TODO Call EarlyBoundDataverseApiService.DeleteEntity<T>
                break;

            case Constants.Actions.Help:
                Logger.Log(Constants.Messages.HelpMessage);
                break;

            case Constants.Actions.Cleanup:
                Logger.Log(Constants.Messages.CleanupMessage);
                // TODO Implement logic to delete all created entities
                break;
            
            case Constants.Actions.Exit:
                Logger.Log(Constants.Messages.GoodbyeMessage);
                // TODO call cleanup method
                Environment.Exit(0);
                break;

            default:
                Logger.Log(Constants.Messages.InvalidAction);
                break;
        }
    }
}