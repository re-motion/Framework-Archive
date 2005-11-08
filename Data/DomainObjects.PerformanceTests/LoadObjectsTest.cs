using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.PerformanceTests.TestDomain;

namespace Rubicon.Data.DomainObjects.PerformanceTests
{
[TestFixture]
public class LoadObjectsTest : DatabaseTest
{
  // types

  // static members and constants

  private static ObjectID s_clientID = new ObjectID ("Client", new Guid ("6F20355F-FA99-4c4e-B432-02C41F7BD390"));
  private static ObjectID s_fileID = new ObjectID ("File", Guid.NewGuid ());

  // member fields

  // construction and disposing

  public LoadObjectsTest ()
  {
  }

  // methods and properties

  [Test]
  public void LoadObjectsOverRelationTest ()
  {
    const int numberOfTests = 10;

    Console.WriteLine ("Expected average duration of LoadObjectsOverRelationTest on reference system: ~270ms");
 
    TimeSpan elapsedTime = new TimeSpan (0);
    for (int i = 0; i < numberOfTests; i++)
    {
      ClientTransaction.SetCurrent (null);
      Client client = Client.GetObject (s_clientID);

      DateTime startTime = DateTime.Now;

      DomainObjectCollection files = client.Files;

      DateTime endTime = DateTime.Now;

      elapsedTime += (endTime - startTime);
    }

    double averageMilliSeconds = elapsedTime.TotalMilliseconds / numberOfTests;
    Console.WriteLine ("LoadObjectsOverRelationTest (executed {0}x): Average duration: {1} ms", numberOfTests, averageMilliSeconds.ToString ("n"));
  }
}
}
