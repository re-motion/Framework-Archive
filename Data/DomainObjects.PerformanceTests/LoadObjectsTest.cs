using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.PerformanceTests.TestDomain;

namespace Rubicon.Data.DomainObjects.PerformanceTests
{
// TODO ES: Inherit from DatabaseTest class which initializes test data correctly.
[TestFixture]
public class LoadObjectsTest
{
  // types

  // static members and constants

  private static ObjectID s_clientID = new ObjectID ("Client", new Guid ("6F20355F-FA99-4c4e-B432-02C41F7BD390"));
  private static ObjectID s_fileID = new ObjectID ("File", Guid.NewGuid ());

  // member fields

  private Client _client;

  // construction and disposing

  public LoadObjectsTest ()
  {
  }

  // methods and properties

  [Test]
  public void LoadObjectsOverRelationTest ()
  {
    const int numberOfTests = 10;

    Console.WriteLine ("Expected average duration of LoadObjectsOverRelationTest: ~900ms");
 
    TimeSpan elapsedTime = new TimeSpan (0);
    for (int i = 0; i < numberOfTests; i++)
    {
      ClientTransaction.SetCurrent (null);
      _client = Client.GetObject (s_clientID);

      DateTime startTime = DateTime.Now;

      DomainObjectCollection files = _client.Files;

      DateTime endTime = DateTime.Now;

      elapsedTime += (endTime - startTime);
    }

    double averageMilliSeconds = elapsedTime.TotalMilliseconds / numberOfTests;
    Console.WriteLine ("LoadObjectsOverRelationTest (executed {0}x): Average duration: {1} ms", numberOfTests, averageMilliSeconds.ToString ("n"));
  }
}
}
