using Dataverse_api.Util;

namespace Dataverse_api.View;

/// <summary>
/// This class contains methods to validate user input and handle user interactions.
/// It asks for user input and validates it, before returning it to the caller (application) to be sent to the service.
/// </summary>
public static class InputManager
{
    // Create mappings for valid actions and entities
    private static readonly Dictionary<string, string> ValidActions = new()
    {
        { "create", Constants.Actions.Create },
        { "get", Constants.Actions.Get },
        { "update", Constants.Actions.Update },
        { "delete", Constants.Actions.Delete },
        { "help", Constants.Actions.Help },
        { "exit", Constants.Actions.Exit }
    };

    private static readonly Dictionary<string, string?> ValidEntities = new()
    {
        { "account", Constants.EntityNames.Account },
        { "contact", Constants.EntityNames.Contact },
        { "case", Constants.EntityNames.Case }
    };
    
    public static bool StartManualInput()
    {
        while (true)
        {
            // Ask the user if they want to manually input the data
            Logger.Log("Would you like to use the application manually? (y/n)");
            
            // Validate the user's response
            switch (Console.ReadLine()?.ToLower())
            {
                case "y":
                    return true;
                case "n":
                    return false;
                default:
                    Logger.Log("Invalid input. Please enter 'y' or 'n'.");
                    continue;
            }
        }
    }

    public static Command ListenForCommand()
    {
        while (true)
        {
            Logger.Log(Constants.Messages.WhatToDoNext);
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Logger.Log("Invalid input. Please enter a command.");
                continue;
            }

            // Split the input
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                // Check for help or exit commands
                if(parts[0] is Constants.Actions.Help or Constants.Actions.Exit)
                    return new Command
                    {
                        Action = parts[0], // TODO this logic might be improved
                        Entity = null,
                        Id = null
                    };
                
                Logger.Log(Constants.Messages.InvalidAction);
                continue;
            }

            var actionInput = parts[0].ToLower();
            var entityInput = parts[1].ToLower();
            Guid? id = parts.Length == 3 && Guid.TryParse(parts[2], out var parsedId) ? parsedId : null;

            // Map the action and entity inputs to constants
            // TODO use the new property All of each class to validate the input
            if (!ValidActions.TryGetValue(actionInput, out var action))
            {
                Logger.Log(Constants.Messages.InvalidAction);
                continue;
            }

            // Return if both match
            if (ValidEntities.TryGetValue(entityInput, out var entity))
                return new Command
                {
                    Action = action,
                    Entity = entity,
                    Id = id
                };
            
            Logger.Log(Constants.Messages.InvalidEntity);
            continue;

        }
    }


}