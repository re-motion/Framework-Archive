using System;
using System.Xml;

using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Text.CommandLine;
using Rubicon.Utilities;
using Rubicon.Security.Metadata;

namespace Rubicon.SecurityManager.Metadata.Importer
{
  public class Program
  {
    public static int Main (string[] args)
    {
      ClientTransaction transaction = new ClientTransaction ();

      CommandLineArguments arguments = GetArguments (args);
      if (arguments == null)
        return 1;

      Program program = new Program (arguments);
      return program.Run ();
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

    CommandLineArguments _arguments;

    private Program (CommandLineArguments arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);
      _arguments = arguments;
    }

    public int Run ()
    {
      try
      {
        ClientTransaction transaction = new ClientTransaction ();
        
        if (_arguments.ImportMetadata)
          ImportMetadata (transaction);

        if (_arguments.ImportLocalization)
          ImportLocalization (transaction);

        transaction.Commit ();

        return 0;
      }
      catch (Exception e)
      {
        HandleException (e);
        return 1;
      }
    }

    private void ImportMetadata (ClientTransaction transaction)
    {
      MetadataImporter importer = new MetadataImporter (transaction);
      importer.Import (_arguments.MetadataFile);
    }

    private void ImportLocalization (ClientTransaction transaction)
    {
      CultureImporter importer = new CultureImporter (transaction);
      LocalizationFileNameStrategy localizationFileNameStrategy = new LocalizationFileNameStrategy ();
      string[] localizationFileNames = localizationFileNameStrategy.GetLocalizationFileNames (_arguments.MetadataFile);

      foreach (string localizationFileName in localizationFileNames)
        importer.Import (localizationFileName);
    }

    private void HandleException (Exception exception)
    {
      if (_arguments.Verbose)
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
