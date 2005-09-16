using System;

namespace Rubicon.Data.DomainObjects.PerformanceTests
{
/// <summary>
/// This class is the root to execute a single test in the profiler.
/// Switch the project type to a ConsoleApplication in order to use it.
/// </summary>
public class Root
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  private Root ()
  {
  }

  // methods and properties

  [STAThread]
  public static void Main (string[] args)
  {
    Mapping.MappingConfiguration mappingConfiguration = Mapping.MappingConfiguration.Current;
    LoadObjectsTest test = new LoadObjectsTest ();
    test.SetUp ();
    test.LoadObjectsOverRelationTest ();
  }
  
}
}
