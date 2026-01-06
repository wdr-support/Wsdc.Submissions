namespace Wsdc.Submissions.Apps.DocBuilder;

///////////////////////////////////////////////////////////////////////////////
// POCO DOCUMENTATION MODELS
///////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents documentation for a single POCO class.
/// </summary>
public class ClassDocumentation
{
    /// <summary>
    /// Name of the class.
    /// </summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// XML summary documentation for the class.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Collection of property documentation for this class.
    /// </summary>
    public List<PropertyDocumentation> Properties { get; set; } = new();
}

/// <summary>
/// Represents documentation for a single property.
/// </summary>
public class PropertyDocumentation
{
    /// <summary>
    /// Name of the property.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Type name of the property (formatted for display).
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// XML summary documentation for the property.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Represents documentation for a single enum type.
/// </summary>
public class EnumDocumentation
{
    /// <summary>
    /// Name of the enum.
    /// </summary>
    public string EnumName { get; set; } = string.Empty;

    /// <summary>
    /// XML summary documentation for the enum.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Collection of enum value documentation.
    /// </summary>
    public List<EnumValueDocumentation> Values { get; set; } = new();
}

/// <summary>
/// Represents documentation for a single enum value.
/// </summary>
public class EnumValueDocumentation
{
    /// <summary>
    /// Name of the enum value.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Integer value of the enum member.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// XML summary documentation for the enum value.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Represents the complete documentation for a folder of POCOs.
/// </summary>
public class FolderDocumentation
{
    /// <summary>
    /// Name of the folder being documented.
    /// </summary>
    public string FolderName { get; set; } = string.Empty;

    /// <summary>
    /// Collection of class documentation in this folder.
    /// </summary>
    public List<ClassDocumentation> Classes { get; set; } = new();

    /// <summary>
    /// Collection of enum documentation in this folder.
    /// </summary>
    public List<EnumDocumentation> Enums { get; set; } = new();
}

/// <summary>
/// Represents the complete documentation for an entire project.
/// </summary>
public class ProjectDocumentation
{
    /// <summary>
    /// Name of the project being documented.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Collection of folder documentation in this project.
    /// </summary>
    public List<FolderDocumentation> Folders { get; set; } = new();
}
