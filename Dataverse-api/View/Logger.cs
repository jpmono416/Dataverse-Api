using System.Reflection;
using Dataverse_api.Entities;
using Microsoft.Xrm.Sdk;

namespace Dataverse_api.View;

/// <summary>
/// This class contains methods to log messages to the console with various levels of severity and different formats.
/// </summary>
public static class Logger
{
    private static readonly Dictionary<Type, List<string>> EntityAttributes = new()
    {
        { typeof(Incident), ["CaseTitle", "CaseNumber", "Priority", "Origin", "Customer", "StatusReason", "CreatedOn"] },
        { typeof(Account), ["AccountName", "MainPhone", "Address1_City", "PrimaryContact"] },
        { typeof(Contact), ["FullName", "Email", "CompanyName", "BusinessPhone"] }
    };
    
    public static void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }
    
    /// <summary>
    /// Prints an error message to the console in red and includes the exception stack trace.
    /// </summary>
    /// <param name="e">The exception thrown</param>
    public static void LogError(Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {e.Message}");
        
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"Exception stack trace: {e.StackTrace}");
    }
    
    /// <summary>
    /// Logs an entity's relevant attributes to the console.
    /// </summary>
    /// <param name="entity">The Dataverse Entity object to print</param>
    /// <typeparam name="T">
    /// The type of the entity. Supported types:
    /// <list type="bullet">
    /// <item>
    /// <description>Case: Logs attributes such as Case Title, Case Number, Priority, etc.</description>
    /// </item>
    /// <item>
    /// <description>Account: Logs attributes such as Account Name, Main Phone, Address1: City, etc.</description>
    /// </item>
    /// <item>
    /// <description>Contact: Logs attributes such as Full Name, Email, Company Name, etc.</description>
    /// </item>
    /// </list>
    /// </typeparam>
    public static void LogEntity<T>(T entity) where T : Entity
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityType = entity.GetType();

        if (!EntityAttributes.TryGetValue(entityType, out var relevantAttributes))
        {
            Console.WriteLine($"No logging configuration found for entity type: {entityType.Name}");
            return;
        }

        Console.WriteLine($"Logging entity: {entityType.Name}");
        
        relevantAttributes
            .Select(attribute => new { attribute, property = entityType.GetProperty(attribute) })
            .ToList()
            .ForEach(item => Console.WriteLine(item.property != null
                ? $"{item.attribute}: {item.property.GetValue(entity) ?? "null"}"
                : $"{item.attribute}: [Property not found]"));
    }
}