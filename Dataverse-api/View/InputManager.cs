namespace Dataverse_api.View;

/// <summary>
/// This class contains methods to validate user input and handle user interactions.
/// It asks for user input and validates it, before returning it to the caller (application) to be sent to the service.
/// </summary>
public class InputManager
{
    public static bool StartManualInput()
    {
        while (true)
        {
            // Ask the user if they want to manually input the data
            Logger.Log("Would you like to use the application manually? (y/n)");
            
            switch (Console.ReadLine()?.ToLower())
            {
                // Validate the user's response
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
}