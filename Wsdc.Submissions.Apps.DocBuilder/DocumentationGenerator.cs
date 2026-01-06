using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Wsdc.Submissions.Apps.DocBuilder;

///////////////////////////////////////////////////////////////////////////////
// DOCUMENTATION GENERATOR
///////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Generates markdown documentation from compiled assemblies and XML documentation files.
/// </summary>
public class DocumentationGenerator
{
    private readonly string _solutionRoot;

    /// <summary>
    /// Initializes a new instance of the DocumentationGenerator.
    /// </summary>
    /// <param name="solutionRoot">Absolute path to the solution root directory.</param>
    public DocumentationGenerator(string solutionRoot)
    {
        _solutionRoot = solutionRoot;
    }

    private static readonly string[] ExcludedFolders = { "bin", "obj", "Properties" };

    /// <summary>
    /// Generates documentation for all POCOs in the specified relative path.
    /// Supports both project-level paths (with subfolders) and specific folder paths.
    /// </summary>
    /// <param name="relativePath">Relative path from solution root (e.g., "Wsdc.Submissions.Models" or "Wsdc.Submissions.Models\Dtos").</param>
    public void GenerateDocumentation(string relativePath)
    {
        var absolutePath = Path.Combine(_solutionRoot, relativePath);
        var isProjectRoot = IsProjectRoot(absolutePath);

        if (isProjectRoot)
        {
            var projectDoc = ExtractProjectDocumentation(relativePath);
            var markdown = GenerateProjectMarkdown(projectDoc);
            WriteOutput(projectDoc.ProjectName, markdown);
        }
        else
        {
            var folderDoc = ExtractFolderDocumentation(relativePath);
            var markdown = GenerateFolderMarkdown(folderDoc);
            WriteOutput(folderDoc.FolderName, markdown);
        }
    }

    /// <summary>
    /// Determines if the path is a project root (contains a .csproj file).
    /// </summary>
    private static bool IsProjectRoot(string absolutePath)
    {
        return Directory.Exists(absolutePath) &&
               Directory.GetFiles(absolutePath, "*.csproj").Length > 0;
    }

    /// <summary>
    /// Extracts documentation for an entire project, including all subfolders.
    /// </summary>
    private ProjectDocumentation ExtractProjectDocumentation(string relativePath)
    {
        var absolutePath = Path.Combine(_solutionRoot, relativePath);
        var projectName = Path.GetFileName(absolutePath);

        var (assembly, xmlDoc) = LoadAssemblyAndXml(projectName);
        var xmlMembers = ParseXmlDocumentation(xmlDoc);

        var projectDoc = new ProjectDocumentation { ProjectName = projectName };

        // Get all subdirectories (excluding bin, obj, etc.)
        var subDirectories = Directory.GetDirectories(absolutePath)
            .Where(d => !ExcludedFolders.Contains(Path.GetFileName(d)))
            .OrderBy(d => d);

        foreach (var subDir in subDirectories)
        {
            var folderName = Path.GetFileName(subDir);
            var folderDoc = ExtractFolderFromDirectory(subDir, assembly, xmlMembers, projectName, folderName);

            if (folderDoc.Classes.Count > 0 || folderDoc.Enums.Count > 0)
            {
                projectDoc.Folders.Add(folderDoc);
            }
        }

        return projectDoc;
    }

    /// <summary>
    /// Extracts documentation from the assembly and XML docs for a specific folder.
    /// </summary>
    private FolderDocumentation ExtractFolderDocumentation(string relativeFolderPath)
    {
        var absoluteFolderPath = Path.Combine(_solutionRoot, relativeFolderPath);
        var folderName = Path.GetFileName(absoluteFolderPath);
        var projectName = GetProjectNameFromPath(relativeFolderPath);

        var (assembly, xmlDoc) = LoadAssemblyAndXml(projectName);
        var xmlMembers = ParseXmlDocumentation(xmlDoc);

        return ExtractFolderFromDirectory(absoluteFolderPath, assembly, xmlMembers, projectName, folderName);
    }

    /// <summary>
    /// Extracts documentation for all classes and enums in a directory.
    /// </summary>
    private FolderDocumentation ExtractFolderFromDirectory(
        string absoluteFolderPath,
        Assembly assembly,
        Dictionary<string, string> xmlMembers,
        string projectName,
        string folderName)
    {
        var folderDoc = new FolderDocumentation { FolderName = folderName };
        var sourceFiles = Directory.GetFiles(absoluteFolderPath, "*.cs");

        foreach (var sourceFile in sourceFiles.OrderBy(f => f))
        {
            var typeName = Path.GetFileNameWithoutExtension(sourceFile);

            // Try to extract as class first
            var classDoc = ExtractClassDocumentation(assembly, xmlMembers, projectName, folderName, typeName);
            if (classDoc != null)
            {
                folderDoc.Classes.Add(classDoc);
                continue;
            }

            // Try to extract as enum
            var enumDoc = ExtractEnumDocumentation(assembly, xmlMembers, projectName, folderName, typeName);
            if (enumDoc != null)
            {
                folderDoc.Enums.Add(enumDoc);
            }
        }

        return folderDoc;
    }

    /// <summary>
    /// Extracts the project name from the relative folder path.
    /// </summary>
    private static string GetProjectNameFromPath(string relativeFolderPath)
    {
        var parts = relativeFolderPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return parts[0];
    }

    /// <summary>
    /// Loads the compiled assembly and XML documentation file for a project.
    /// </summary>
    private (Assembly assembly, XDocument xmlDoc) LoadAssemblyAndXml(string projectName)
    {
        var projectPath = Path.Combine(_solutionRoot, projectName);
        var binPath = Path.Combine(projectPath, "bin");
        
        // Find the most recent build output
        var dllPath = FindNewestFile(binPath, $"{projectName}.dll");
        var xmlPath = FindNewestFile(binPath, $"{projectName}.xml");

        if (dllPath == null)
            throw new FileNotFoundException($"Could not find compiled assembly for {projectName}. Please build the project first.");

        if (xmlPath == null)
            throw new FileNotFoundException($"Could not find XML documentation for {projectName}. Ensure GenerateDocumentationFile is enabled.");

        var assembly = Assembly.LoadFrom(dllPath);
        var xmlDoc = XDocument.Load(xmlPath);

        return (assembly, xmlDoc);
    }

    /// <summary>
    /// Recursively finds the newest file matching the specified name in a directory.
    /// </summary>
    private static string? FindNewestFile(string directory, string fileName)
    {
        if (!Directory.Exists(directory))
            return null;

        return Directory.GetFiles(directory, fileName, SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTime)
            .FirstOrDefault();
    }

    /// <summary>
    /// Parses XML documentation into a dictionary keyed by member name.
    /// </summary>
    private static Dictionary<string, string> ParseXmlDocumentation(XDocument xmlDoc)
    {
        var members = new Dictionary<string, string>();
        var memberElements = xmlDoc.Descendants("member");

        foreach (var member in memberElements)
        {
            var name = member.Attribute("name")?.Value;
            var summary = member.Element("summary")?.Value?.Trim();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(summary))
            {
                // Normalize whitespace in summary
                summary = string.Join(" ", summary.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries));
                members[name] = summary;
            }
        }

        return members;
    }

    /// <summary>
    /// Extracts documentation for a single class.
    /// </summary>
    private ClassDocumentation? ExtractClassDocumentation(
        Assembly assembly,
        Dictionary<string, string> xmlMembers,
        string projectName,
        string folderName,
        string className)
    {
        // Build the expected namespace
        var namespaceName = $"{projectName}.{folderName}";
        var fullTypeName = $"{namespaceName}.{className}";

        var type = assembly.GetType(fullTypeName);
        if (type == null || !IsPocoClass(type))
            return null;

        var classDoc = new ClassDocumentation
        {
            ClassName = className,
            Summary = xmlMembers.GetValueOrDefault($"T:{fullTypeName}", string.Empty)
        };

        // Get public instance properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            var propDoc = new PropertyDocumentation
            {
                PropertyName = prop.Name,
                TypeName = FormatTypeName(prop.PropertyType),
                Summary = xmlMembers.GetValueOrDefault($"P:{fullTypeName}.{prop.Name}", string.Empty)
            };
            classDoc.Properties.Add(propDoc);
        }

        return classDoc;
    }

    /// <summary>
    /// Extracts documentation for a single enum.
    /// </summary>
    private EnumDocumentation? ExtractEnumDocumentation(
        Assembly assembly,
        Dictionary<string, string> xmlMembers,
        string projectName,
        string folderName,
        string enumName)
    {
        // Build the expected namespace
        var namespaceName = $"{projectName}.{folderName}";
        var fullTypeName = $"{namespaceName}.{enumName}";

        var type = assembly.GetType(fullTypeName);
        if (type == null || !type.IsEnum)
            return null;

        var enumDoc = new EnumDocumentation
        {
            EnumName = enumName,
            Summary = xmlMembers.GetValueOrDefault($"T:{fullTypeName}", string.Empty)
        };

        // Get enum values
        var enumValues = Enum.GetValues(type);
        foreach (var value in enumValues)
        {
            var valueName = value.ToString()!;
            var valueDoc = new EnumValueDocumentation
            {
                Name = valueName,
                Value = Convert.ToInt32(value),
                Summary = xmlMembers.GetValueOrDefault($"F:{fullTypeName}.{valueName}", string.Empty)
            };
            enumDoc.Values.Add(valueDoc);
        }

        return enumDoc;
    }

    /// <summary>
    /// Determines if a type is a POCO class.
    /// </summary>
    private static bool IsPocoClass(Type type)
    {
        return type.IsClass && type.IsPublic && !type.IsAbstract && !type.IsInterface;
    }

    /// <summary>
    /// Formats a type name for display, handling generics.
    /// </summary>
    private static string FormatTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeName = type.Name.Split('`')[0];
            var genericArgs = string.Join(", ", type.GetGenericArguments().Select(FormatTypeName));
            return $"{genericTypeName}<{genericArgs}>";
        }

        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
        {
            return $"{FormatTypeName(nullableType)}?";
        }

        return type.Name switch
        {
            "String" => "string",
            "Int32" => "int",
            "Int64" => "long",
            "Boolean" => "bool",
            "Double" => "double",
            "Decimal" => "decimal",
            "Single" => "float",
            "DateTime" => "DateTime",
            _ => type.Name
        };
    }

    /// <summary>
    /// Generates markdown content from project documentation with folder hierarchy.
    /// </summary>
    private static string GenerateProjectMarkdown(ProjectDocumentation projectDoc)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {projectDoc.ProjectName}");
        sb.AppendLine();
        sb.AppendLine("Model documentation for the WSDC Submissions system.");
        sb.AppendLine();

        foreach (var folderDoc in projectDoc.Folders)
        {
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine($"## {folderDoc.FolderName}");
            sb.AppendLine();

            // Generate class documentation
            foreach (var classDoc in folderDoc.Classes)
            {
                sb.AppendLine($"### {classDoc.ClassName}");
                sb.AppendLine();

                if (!string.IsNullOrEmpty(classDoc.Summary))
                {
                    sb.AppendLine(classDoc.Summary);
                    sb.AppendLine();
                }

                if (classDoc.Properties.Count > 0)
                {
                    sb.AppendLine("| Property | Type | Description |");
                    sb.AppendLine("|----------|------|-------------|");

                    foreach (var prop in classDoc.Properties)
                    {
                        var escapedSummary = prop.Summary.Replace("|", "\\|");
                        sb.AppendLine($"| {prop.PropertyName} | `{prop.TypeName}` | {escapedSummary} |");
                    }

                    sb.AppendLine();
                }
            }

            // Generate enum documentation
            foreach (var enumDoc in folderDoc.Enums)
            {
                sb.AppendLine($"### {enumDoc.EnumName}");
                sb.AppendLine();

                if (!string.IsNullOrEmpty(enumDoc.Summary))
                {
                    sb.AppendLine(enumDoc.Summary);
                    sb.AppendLine();
                }

                if (enumDoc.Values.Count > 0)
                {
                    sb.AppendLine("| Value | Integer | Description |");
                    sb.AppendLine("|-------|---------|-------------|");

                    foreach (var value in enumDoc.Values)
                    {
                        var escapedSummary = value.Summary.Replace("|", "\\|");
                        sb.AppendLine($"| {value.Name} | `{value.Value}` | {escapedSummary} |");
                    }

                    sb.AppendLine();
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates markdown content from folder documentation.
    /// </summary>
    private static string GenerateFolderMarkdown(FolderDocumentation folderDoc)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {folderDoc.FolderName}");
        sb.AppendLine();
        sb.AppendLine("Documentation for the WSDC Submissions system.");
        sb.AppendLine();

        // Generate class documentation
        foreach (var classDoc in folderDoc.Classes)
        {
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine($"## {classDoc.ClassName}");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(classDoc.Summary))
            {
                sb.AppendLine(classDoc.Summary);
                sb.AppendLine();
            }

            if (classDoc.Properties.Count > 0)
            {
                sb.AppendLine("| Property | Type | Description |");
                sb.AppendLine("|----------|------|-------------|");

                foreach (var prop in classDoc.Properties)
                {
                    var escapedSummary = prop.Summary.Replace("|", "\\|");
                    sb.AppendLine($"| {prop.PropertyName} | `{prop.TypeName}` | {escapedSummary} |");
                }

                sb.AppendLine();
            }
        }

        // Generate enum documentation
        foreach (var enumDoc in folderDoc.Enums)
        {
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine($"## {enumDoc.EnumName}");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(enumDoc.Summary))
            {
                sb.AppendLine(enumDoc.Summary);
                sb.AppendLine();
            }

            if (enumDoc.Values.Count > 0)
            {
                sb.AppendLine("| Value | Integer | Description |");
                sb.AppendLine("|-------|---------|-------------|");

                foreach (var value in enumDoc.Values)
                {
                    var escapedSummary = value.Summary.Replace("|", "\\|");
                    sb.AppendLine($"| {value.Name} | `{value.Value}` | {escapedSummary} |");
                }

                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Writes the generated markdown to the docs folder.
    /// </summary>
    /// <param name="outputName">Name for the output file (without extension).</param>
    /// <param name="markdown">Markdown content to write.</param>
    private void WriteOutput(string outputName, string markdown)
    {
        var docsPath = Path.Combine(_solutionRoot, "docs");
        Directory.CreateDirectory(docsPath);

        var outputPath = Path.Combine(docsPath, $"{outputName}.md");
        File.WriteAllText(outputPath, markdown);

        Console.WriteLine($"Documentation generated: {outputPath}");
    }
}
