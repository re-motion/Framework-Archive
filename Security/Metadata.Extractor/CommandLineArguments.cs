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
    public string DomainAssemblyName;

    [CommandLineStringArgument ("output", false,
        Description = "The name of the XML metadata output file.",
        Placeholder = "metadata")]
    public string MetadataOutputFile;

    [CommandLineStringArgument ("language", true,
        Description="The language code for the multilingual descriptions of the metadata objects.",
        Placeholder="language")]
    public string Languages = string.Empty;

    [CommandLineFlagArgument ("suppress", false,
        Description = "Suppress export of metadata file.")]
    public bool SuppressMetadata = false;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;
  }
}
