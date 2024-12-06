using Dataverse_api.Entities;

namespace Dataverse_api.View;

/// <summary>
/// Contains all the constants used in the application for commands, entities, and messages, as well as demo data.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Contains the demo data for the application. It contains static readonly and not constant fields because 
    /// the IDs are assigned as they are created in the Application, the rest remains constant.
    /// </summary>
    public static class DemoData
    {
        public static readonly Account DemoAccount = new()
        {
            Name = "Acme Corporation",
            EMailAddress1 = "contact@acme.com",
            Telephone1 = "123-456-7890"
        };
        
        public static readonly Contact DemoContact = new()
        {
            FirstName = "John",
            LastName = "Doe",
            EMailAddress1 = "john.doe@acme.com",
        };
        
        public static readonly Incident DemoCase = new()
        {
            Title = "Billing Issue",
            Description = "Customer disputes invoice amount.",
        };
    }

    /// <summary>
    /// Contains the valid actions that commands can execute. It contains named constants for explicit use
    /// and a list for iteration and validation
    /// </summary>
    public static class Actions
    {
        public const string Help = "help";
        public const string Create = "create";
        public const string Get = "get";
        public const string Update = "update";
        public const string List = "list";
        public const string Delete = "delete";
        public const string Cleanup = "cleanup";
        public const string Exit = "exit";
        
        public static readonly List<string> All =
        [
            Help,
            Create,
            Get,
            Update,
            List,
            Delete,
            Cleanup,
            Exit
        ];
    }

    /// <summary>
    /// Contains the valid entity options that can be handled by the application. It contains named constants 
    /// for explicit use and a list for iteration and validation
    /// </summary>
    public static class EntityNames
    {
        public const string Account = "account";
        public const string Contact = "contact";
        public const string Case = "case";
        
        public static readonly List<string> All =
        [
            Account,
            Contact,
            Case
        ];
    }

    /// <summary>
    /// Contains the messages logged by the application.
    /// </summary>
    public static class Messages
    {
        public const string HelpMessage = """
                                          To use the application, enter a command and an entity type. For example: 'create account'.
                                              Available commands:
                                                  - help (displays this message)
                                                  - create (creates a new entity)
                                                  - get* (retrieves an entity)
                                                  - update* (updates an entity)
                                                  - list (lists all entities of a type)
                                                  - delete* (deletes an entity)
                                                  - cleanup (deletes all created entities)
                                                  - exit (performs a cleanup and exits the program)
                                  
                                              Available entities:
                                                  - Account
                                                  - Contact
                                                  - Case
                                          * Note: 'get', 'update' and 'delete' commands require an entity GUID as well. For example: 'get case <guid>'
                                          """;
        public static readonly string InvalidAction = $"Invalid action. Supported actions: {string.Join(", ", Actions.All)}.";
        public static readonly string InvalidEntity = $"Invalid entity. Supported entities: {string.Join(", ", EntityNames.All)}";
        public const string CommandRequiresId = "This command requires an entity ID. Please use `command entity <guid>`.";
        public const string WelcomeMessage = "Welcome to the Customer Service Hub Management Console!";
        public const string CleanupMessage = "Cleaning up created entities...";
        public const string WhatToDoNext = "What would you like to do next? enter a command and entity. Alternatively use 'help' or 'exit'";
        public const string GoodbyeMessage = "Thanks for using the app. Goodbye!";
    }
}