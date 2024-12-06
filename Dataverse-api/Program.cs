using Dataverse_api.Application;
using Dataverse_api.Util;

namespace Dataverse_api;

internal static class Program
{
    private static void Main()
    {
        Utils.LoadEnvVariables();
        App.Run();
    }
}
