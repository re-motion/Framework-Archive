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

  private ObjectID _clientID;
  private ObjectID _fileID;

  // member fields

  // construction and disposing

  public LoadObjectsTest ()
  {
  }

  // methods and properties

  public override void TestFixtureSetUp ()
  {
    base.TestFixtureSetUp ();
   
   _clientID = new ObjectID ("Client", new Guid ("6F20355F-FA99-4c4e-B432-02C41F7BD390"));
   _fileID = new ObjectID ("File", Guid.NewGuid ());
 }
  
  [Test]
  public void LoadObjectsOverRelationTest ()
  {
    const int numberOfTests = 10;

    Console.WriteLine ("Expected average duration of LoadObjectsOverRelationTest on reference system: ~270ms");
 
    TimeSpan elapsedTime = new TimeSpan (0);
    for (int i = 0; i < numberOfTests; i++)
    {
      ClientTransaction.SetCurrent (null);
      Client client = Client.GetObject (_clientID);

      DateTime startTime = DateTime.Now;

      DomainObjectCollection files = client.Files;

      DateTime endTime = DateTime.Now;

      elapsedTime += (endTime - startTime);
      Assert.AreEqual (3000, files.Count);
    }

    double averageMilliSeconds = elapsedTime.TotalMilliseconds / numberOfTests;
    Console.WriteLine ("LoadObjectsOverRelationTest (executed {0}x): Average duration: {1} ms", numberOfTests, averageMilliSeconds.ToString ("n"));
  }

  [Test]
  public void LoadObjectsOverRelationWithAbstractBaseClass ()
  {
    const int numberOfTests = 10;

    Console.WriteLine ("Expected average duration of LoadObjectsOverRelationWithAbstractBaseClass on reference system: ??? ms");

    TimeSpan elapsedTime = new TimeSpan (0);
    for (int i = 0; i < numberOfTests; i++)
    {
      ClientTransaction.SetCurrent (null);
      Client client = Client.GetObject (_clientID);

      DateTime startTime = DateTime.Now;

      DomainObjectCollection clientBoundBaseClasses = client.ClientBoundBaseClasses;

      DateTime endTime = DateTime.Now;

      elapsedTime += (endTime - startTime);
      Assert.AreEqual (3000, clientBoundBaseClasses.Count);
    }

    double averageMilliSeconds = elapsedTime.TotalMilliseconds / numberOfTests;
    Console.WriteLine ("LoadObjectsOverRelationWithAbstractBaseClass (executed {0}x): Average duration: {1} ms", numberOfTests, averageMilliSeconds.ToString ("n"));
  }
}
}
