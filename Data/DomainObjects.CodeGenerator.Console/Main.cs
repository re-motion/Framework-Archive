using System;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Console
{
class MainClass
{
  // types

  // static members and constants

	[STAThread]
	static void Main(string[] args)
	{
    BuilderFactory builderFactory;
    try
    {
      builderFactory = new BuilderFactory (args);
    }
    catch (Exception ex)
    {
      PrintException (ex, true);
      BuilderFactory.PrintHelp ();
      return;
    }

    if (builderFactory.IsOperationModeSet (OperationMode.Help))
    {
      BuilderFactory.PrintHelp ();
      return;
    }

    try
    {
      IBuilder[] builders = builderFactory.GetBuilders ();
      foreach (IBuilder builder in builders)
        builder.Build ();
    }
    catch (Exception ex)
    {
      PrintException (ex);
    }
	}

  private static void PrintException (Exception ex)
  {
    PrintException (ex, false);
  }

  private static void PrintException (Exception ex, bool hideStackTrace)
  {
      while (ex != null)
      {
        System.Console.Error.WriteLine ("");
        System.Console.Error.WriteLine ("Exception: " + ex.GetType ().Name);
        System.Console.Error.WriteLine (ex.Message);
        System.Console.Error.WriteLine ("");
        if (!hideStackTrace)
        {
          System.Console.Error.WriteLine (ex.StackTrace);
          System.Console.Error.WriteLine ("");
        }

        ex = ex.InnerException;
      }
  }

  // member fields

  // construction and disposing

  // methods and properties

}
}
