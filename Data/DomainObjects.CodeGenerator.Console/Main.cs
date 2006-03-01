using System;
using System.IO;
using System.Collections;
using System.Reflection;
using Rubicon.Text.CommandLine;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Console
{

class MainClass
{
	static int Main(string[] args)
	{
    Arguments arguments;
    CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
    try
    {
      arguments = (Arguments) parser.Parse (args);
    }
    catch (CommandLineArgumentException e)
    {
      System.Console.WriteLine (e.Message);
      System.Console.WriteLine ("Usage:");
      System.Console.WriteLine (parser.GetAsciiSynopsis (System.Environment.GetCommandLineArgs()[0], 79));
      return 1;
    }

    try
    {
      using (new ConfigurationLoader (arguments.ConfigDirectory, arguments.SchemaDirectory))
      {
        if ((arguments.Mode & OperationMode.Sql) != 0)
        {
          SqlBuilder.Build (arguments.SqlOutput);
        }

        if ((arguments.Mode & OperationMode.DomainModel) != 0)
        {
          DomainModelBuilder.Build (
              arguments.ClassOutput, 
              arguments.DomainObjectBaseClass, arguments.DomainObjectCollectionBaseClass, 
              arguments.Serializable);
        }
      }
    }
    catch (Exception e)
    {
      if (arguments.Verbose)
      {
        System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
        for (; e != null; e = e.InnerException)
        {
          System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace);
        }
      }
      else
      {
        System.Console.Error.WriteLine ("Execution aborted: {0}", e.Message);
      }
      return 1;
    }
    return 0;
  }
}

}
