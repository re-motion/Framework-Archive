using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Text.CommandLine;

namespace Rubicon.SecurityManager.Metadata.Importer
{
  public class CommandLineArguments
  {
    [CommandLineStringArgument (false,
        Description = "The name of the XML metadata file.",
        Placeholder = "metadata")]
    public string MetadataFile;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;
  }
}
