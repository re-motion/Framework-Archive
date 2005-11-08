using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries.Configuration;

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
    // Have all xml files loaded, so if the code is instrumented by a profiler, 
    // the loading does not falsify the method run times during the first call of GetObject.
    MappingConfiguration mapping = MappingConfiguration.Current;
    StorageProviderConfiguration storageProviderConfiguration = StorageProviderConfiguration.Current;
    QueryConfiguration queryConfiguration = QueryConfiguration.Current;

    LoadObjectsTest test = new LoadObjectsTest ();
    test.SetUp ();
    test.LoadObjectsOverRelationTest ();
  }
  
}
}
