using Dataverse_api.Util;
using Microsoft.Xrm.Sdk;

namespace Dataverse_api.View;

/// <summary>
/// This class contains methods to validate user input and handle user interactions.
/// It asks for user input and validates it, before returning it to the caller (application) to be sent to the service.
/// </summary>
public static class InputManager
{
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
                        EntityType = null,
                        Id = null
                    };
                
                Logger.Log(Constants.Messages.InvalidAction);
                continue;
            }

            var actionInput = parts[0].ToLower();
            var entityInput = parts[1].ToLower();
            Guid? id = parts.Length == 3 && Guid.TryParse(parts[2], out var parsedId) ? parsedId : null;
            
            // Map the action and entity inputs to constants, then return early if they are valid
            if (Constants.Actions.All.Contains(actionInput) 
                && Constants.EntityNames.EntityMapping.TryGetValue(entityInput, out var entityType))
            {
                return new Command
                {
                    Action = actionInput,
                    EntityType = entityType,
                    Id = id
                };
            }
            
            Logger.Log(Constants.Messages.InvalidEntity);
        }
    }
    
    /*
     * TODO this iterates over every single property from the schema, even if it's not required. It should iterate over
     * the required properties only (there is a dictionary in the Constants for this)
     */
    public static T PromptEntityProperties<T>(T entity, bool isUpdate = false) where T : Entity
    {
        var properties = typeof(T).GetProperties();
        var requiredAttributes = Constants.EntityNames.EntityAttributes[entity.GetType()];

        foreach (var property in properties)
        {
            // Skip properties that aren't required
            if (!requiredAttributes.Contains(property.Name)) continue;
            
            // Skip properties that aren't writable or are navigation properties
            if (!property.CanWrite || property.PropertyType == typeof(EntityReference)) continue;

            var currentValue = isUpdate ? property.GetValue(entity) : null;
            Logger.Log($"{property.Name}: {(currentValue != null ? $"Current Value = {currentValue}" : "Required")}");
            Logger.Log("Enter new value or press Enter to skip:");

            // Handle relational properties
            if (property.PropertyType == typeof(EntityReference))
            {
                Logger.Log($"Please provide the ID for {property.Name}:");
                var relatedEntityIdInput = Console.ReadLine()?.Trim();
                if (!Guid.TryParse(relatedEntityIdInput, out var relatedEntityId))
                {
                    Logger.Log($"Invalid ID for {property.Name}. Please try again.");
                    return PromptEntityProperties(entity, isUpdate);
                }
                var entityReference = new EntityReference
                {
                    Id = relatedEntityId,
                    LogicalName = property.Name.Replace("Id", "").ToLower() // Assumes naming convention
                };
                property.SetValue(entity, entityReference);
            }
            
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                if (isUpdate) continue; // Skip updating this property if blank
                if (currentValue == null) throw new InvalidOperationException($"Property {property.Name} is required.");
            }
            else
            {
                try
                {
                    // Convert input to the correct type and set the value
                    var value = Convert.ChangeType(input, property.PropertyType);
                    property.SetValue(entity, value);
                }
                catch
                {
                    Logger.Log($"Invalid value for {property.Name}. Please try again.");
                    return PromptEntityProperties(entity, isUpdate); // Retry for the same entity
                }
            }
        }

        return entity;
    }
}