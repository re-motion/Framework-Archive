using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

using Rubicon.Security;
using Rubicon.Text.CommandLine;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Metadata.Importer
{
  public class Program
  {
    public static int Main (string[] args)
    {
      CommandLineArguments arguments = GetArguments (args);
      if (arguments == null)
        return 1;

      return ImportMetadata (arguments);
    }

    private static CommandLineArguments GetArguments (string[] args)
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (CommandLineArguments));

      try
      {
        return (CommandLineArguments) parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        Console.WriteLine (e.Message);
        WriteUsage (parser);

        return null;
      }
    }

    private static void WriteUsage (CommandLineClassParser parser)
    {
      Console.WriteLine ("Usage:");
      
      string commandName = Environment.GetCommandLineArgs ()[0];
      Console.WriteLine (parser.GetAsciiSynopsis (commandName, Console.BufferWidth));
    }

    private static void SetUpDomain ()
    {
      MappingConfiguration.SetCurrent (new MappingConfiguration (@"SecurityManagerMapping.xml"));
      StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (@"SecurityManagerStorageProviders.xml"));
      QueryConfiguration.SetCurrent (new QueryConfiguration (@"SecurityManagerQueries.xml"));
    }

    private static int ImportMetadata (CommandLineArguments arguments)
    {
      try
      {
        SetUpDomain ();

        XmlDocument metadataXmlDocument = new XmlDocument ();
        metadataXmlDocument.Load (arguments.MetadataFile);

        ClientTransaction transaction = new ClientTransaction ();
        MetadataImporter importer = new MetadataImporter (transaction);
        importer.Import (metadataXmlDocument);
        transaction.Commit ();

        return 0;
      }
      catch (Exception e)
      {
        HandleException (e, arguments.Verbose);
        return 1;
      }
    }

    private static void HandleException (Exception exception, bool verbose)
    {
      if (verbose)
      {
        Console.Error.WriteLine ("Execution aborted. Exception stack:");

        for (; exception != null; exception = exception.InnerException)
        {
          Console.Error.WriteLine ("{0}: {1}\n{2}", exception.GetType ().FullName, exception.Message, exception.StackTrace);
        }
      }
      else
      {
        Console.Error.WriteLine ("Execution aborted: {0}", exception.Message);
      }
    }
  }
}
