using System;
using System.Collections;
using System.IO;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Text.CommandLine;
using Rubicon.Data.DomainObjects.ConfigurationLoader;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.Console
{

[Flags]
public enum OperationMode
{
  [CommandLineMode ("sql", Description = "Generate database setup script.")]
  Sql = 1,

  [CommandLineMode ("classes", Description = "Generate domain object code.")]
  DomainModel = 2,

  [CommandLineMode ("classesForAttributeMapping", Description = "Generate domain object code for the attribute based mapping.")]
  Migration = 6,

  [CommandLineMode ("full", Description = "Generate domain object code and database scripts.")]
  Full = Sql | DomainModel
}

public class Arguments 
{
  [CommandLineModeArgument (false)]
  public OperationMode Mode;

  [CommandLineStringArgument ("sqloutput", true, 
      Description = "Create SQL file(s) in this directory (default: current).",
      Placeholder = "directory")]
  public string SqlOutput = string.Empty;

  [CommandLineStringArgument ("classoutput", true, 
      Description = "Create domain object file(s) in this directory (default: current).",
      Placeholder = "directory")]
  public string ClassOutput = string.Empty;

  [CommandLineStringArgument ("config", false,
      Description = 
      "The config file holding the application's configuration. The mapping file's path is resolved up relative to the location of the config file.",
      Placeholder = "app.config")]
  public string ConfigFile = string.Empty;

  [CommandLineStringArgument ("dobase", true, 
      Description = "Create domain object classes derived from this class (default: " + DomainObjectBuilder.DefaultBaseClass + ")",
      Placeholder = "classname")]
  public string DomainObjectBaseClass = DomainObjectBuilder.DefaultBaseClass;

  [CommandLineStringArgument ("collbase", true, 
      Description = "Create domain object collection classes derived from this class (default: DomainObjectCollection)",
      Placeholder = "classname")]
  public string DomainObjectCollectionBaseClass = DomainObjectCollectionBuilder.DefaultBaseClass;

  [CommandLineFlagArgument ("serializable", false, Description = "Assign the [Serializable] attribute to classes (default: false).")]
  public bool Serializable;

  [CommandLineFlagArgument ("multilingualResources", false, Description = "Assign the [MultiLingualResources] attribute to classes (default: false).")]
  public bool MultiLingualResources;

  [CommandLineFlagArgument ("verbose", false,
      Description = "Verbose output")]
  public bool Verbose;

  [CommandLineStringArgument ("sqlBuilder", true,
      Description = "The assembly qualified type name of the SqlFileBuilder to use for generating the SQL scripts.",
      Placeholder = "Namespace.ClassName,AssemblyName")]
  public string SqlBuilderTypeName = typeof (Rubicon.Data.DomainObjects.Legacy.CodeGenerator.Sql.SqlServer.SqlFileBuilder).AssemblyQualifiedName;

  public void CheckArguments()
  {
    if ((Mode & OperationMode.DomainModel) != 0 )
    {
      if (DomainObjectBaseClass.Length == 0)
        throw new CommandLineArgumentApplicationException ("Domain object base class must not be empty.");
      if (DomainObjectCollectionBaseClass.Length == 0)
        throw new CommandLineArgumentApplicationException ("Domain object collection base class must not be empty.");
    }
  }
}
}