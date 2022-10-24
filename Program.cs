using CarsAndTanks.UX.Forms.MainUI;

namespace CarsAndTanks;

internal static class Program
{
    internal static string applicationSpecificFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CarsAndTanks");

    internal static string applicationUserSettings = "";
    internal static string applicationUserTracks;
    internal static string applicationUserAIModels;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // The folder for the roaming current user 
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        // Combine the base folder with your specific folder....

        // CreateDirectory will check if every folder in path exists and, if not, create them.
        // If all folders exist then CreateDirectory will do nothing.
        Directory.CreateDirectory(applicationSpecificFolder);

        applicationUserAIModels = Path.Combine(applicationSpecificFolder, "UserAIModels");
        applicationUserTracks = Path.Combine(applicationSpecificFolder, "UserTracks");
        applicationUserSettings = Path.Combine(applicationSpecificFolder, "UserSettings");

        Directory.CreateDirectory(applicationUserSettings);
        Directory.CreateDirectory(applicationUserTracks);
        Directory.CreateDirectory(applicationUserAIModels);

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new FormMain());
    }
}