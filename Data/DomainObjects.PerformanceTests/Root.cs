using System;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
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

    private Root()
    {
    }

    // methods and properties

    [STAThread]
    public static void Main (string[] args)
    {
      LoadObjectsTest test1 = new LoadObjectsTest();
      test1.TestFixtureSetUp();

      // Have all xml files loaded, so if the code is instrumented by a profiler, 
      // the loading does not falsify the method run times during the first call of GetObject.
      MappingConfiguration mapping = MappingConfiguration.Current;
      QueryConfiguration queryConfiguration = DomainObjectsConfiguration.Current.Query;

      test1.SetUp();
      test1.LoadObjectsOverRelationTest();
      test1.TearDown();

      test1.SetUp ();
      test1.LoadObjectsOverRelationWithAbstractBaseClass ();
      test1.TearDown ();

      test1.TestFixtureTearDown();

      SerializationTest test2 = new SerializationTest();
      test2.TestFixtureSetUp();

      test2.SetUp();
      test2.Serialize5ValuePropertyObjects ();
      test2.TearDown();

      test2.SetUp ();
      test2.Serialize50ValuePropertyObjects ();
      test2.TearDown ();

      test2.SetUp ();
      test2.Serialize500ValuePropertyObjects ();
      test2.TearDown ();

      test2.SetUp ();
      test2.Serialize1025ValuePropertyObjects ();
      test2.TearDown ();

      test2.SetUp ();
      test2.Serialize41RelationPropertyObjects ();
      test2.TearDown ();

      test2.SetUp ();
      test2.Serialize410RelationPropertyObjects ();
      test2.TearDown ();

      test2.SetUp ();
      test2.Serialize1025RelationPropertyObjects ();
      test2.TearDown ();

      test2.TestFixtureTearDown();

      Console.ReadLine();
    }
  }
}