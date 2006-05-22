using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Text.CommandLine;

namespace Rubicon.Security.Metadata.Extractor
{
  public class CommandLineArguments
  {
    [CommandLineStringArgument ("assembly", false,
        Description="The path to the assembly containing the application domain to analyze.",
        Placeholder="assemblyPath")]
    public string[] DomainAssemblyName;

    [CommandLineStringArgument ("output", false,
        Description = "The name of the XML metadata output file.",
        Placeholder = "metadata")]
    public string MetadataOutputFile;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;
  }
}
