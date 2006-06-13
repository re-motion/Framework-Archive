using System;
using System.Collections;
using System.IO;
using Rubicon.Text.CommandLine;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Console
{

[Flags]
public enum OperationMode
{
  [CommandLineMode ("sql", Description = "Generate database setup script.")]
  Sql = 1,

  [CommandLineMode ("classes", Description = "Generate domain object code.")]
  DomainModel = 2,
  
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

  [CommandLineStringArgument ("config", true,
      Description = "Search for XML files in this directory (default: current).",
      Placeholder = "directory")]
  public string ConfigDirectory = string.Empty;

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

  public void CheckArguments()
  {
    if ((Mode & OperationMode.DomainModel) != 0)
    {
      if (DomainObjectBaseClass.Length == 0)
        throw new CommandLineArgumentApplicationException ("Domain object base class must not be empty.");
      if (DomainObjectCollectionBaseClass.Length == 0)
        throw new CommandLineArgumentApplicationException ("Domain object collection base class must not be empty.");
    }
  }
}
}