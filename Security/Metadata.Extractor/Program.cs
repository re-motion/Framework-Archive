using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

using Rubicon.Security;
using Rubicon.Text.CommandLine;

namespace Rubicon.Security.Metadata.Extractor
{
  public class Program
  {
    public static int Main (string[] args)
    {
      CommandLineArguments arguments;
      CommandLineClassParser parser = new CommandLineClassParser (typeof (CommandLineArguments));

      try
      {
        arguments = (CommandLineArguments) parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        Console.WriteLine (e.Message);
        Console.WriteLine ("Usage:");
        Console.WriteLine (parser.GetAsciiSynopsis (System.Environment.GetCommandLineArgs ()[0], 79));
        return 1;
      }

      try
      {
        MetadataExtractor extractor = new MetadataExtractor (new MetadataToXmlConverter ());

        foreach (string assemblyName in arguments.DomainAssemblyName)
          extractor.AddAssembly (assemblyName);

        extractor.Save (arguments.MetadataOutputFile);
      }
      catch (Exception e)
      {
        if (arguments.Verbose)
        {
          Console.Error.WriteLine ("Execution aborted. Exception stack:");
          
          for (; e != null; e = e.InnerException)
          {
            Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType ().FullName, e.Message, e.StackTrace);
          }
        }
        else
        {
          Console.Error.WriteLine ("Execution aborted: {0}", e.Message);
        }

        return 1;
      }

      return 0;
    }
  }
}
