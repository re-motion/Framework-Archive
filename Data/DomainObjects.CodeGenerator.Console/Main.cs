using System;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Console
{
class MainClass
{
  // types

  // static members and constants

  //TODO: Find a way to provide path to the (stubs-)dll files (for dom/sql generation) as command line argument
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
      PrintException (ex);
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
      while (ex != null)
      {
        System.Console.Error.WriteLine ("");
        System.Console.Error.WriteLine ("Exception: " + ex.GetType ().Name);
        System.Console.Error.WriteLine (ex.Message);
        System.Console.Error.WriteLine ("");
        System.Console.Error.WriteLine (ex.StackTrace);
        System.Console.Error.WriteLine ("");

        ex = ex.InnerException;
      }
  }

  // member fields

  // construction and disposing

  // methods and properties

}
}
