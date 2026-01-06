namespace Wsdc.Submissions.Apps.DocBuilder;

///////////////////////////////////////////////////////////////////////////////
// PROGRAM ENTRY POINT
///////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Entry point for the POCO documentation generator.
/// </summary>
public class Program
{
    private const string UsageMessage = "Usage: dotnet run -- <relative-folder-path>";
    private const string ExampleMessage = "Example: dotnet run -- \"Wsdc.Submissions.Models\\Dtos\"";

    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args">Command line arguments. First argument should be the relative folder path.</param>
    /// <returns>Exit code (0 for success, 1 for error).</returns>
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Error: No folder path provided.");
            Console.Error.WriteLine(UsageMessage);
            Console.Error.WriteLine(ExampleMessage);
            return 1;
        }

        var relativeFolderPath = args[0];
        var solutionRoot = FindSolutionRoot();

        if (solutionRoot == null)
        {
            Console.Error.WriteLine("Error: Could not find solution root. Ensure you are running from within the solution directory.");
            return 1;
        }

        var absoluteFolderPath = Path.Combine(solutionRoot, relativeFolderPath);
        if (!Directory.Exists(absoluteFolderPath))
        {
            Console.Error.WriteLine($"Error: Folder not found: {absoluteFolderPath}");
            return 1;
        }

        try
        {
            var generator = new DocumentationGenerator(solutionRoot);
            generator.GenerateDocumentation(relativeFolderPath);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    /// <summary>
    /// Finds the solution root by searching upward for the .sln file.
    /// </summary>
    private static string? FindSolutionRoot()
    {
        var directory = Directory.GetCurrentDirectory();

        while (directory != null)
        {
            var slnFiles = Directory.GetFiles(directory, "*.sln");
            if (slnFiles.Length > 0)
            {
                return directory;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        return null;
    }
}

