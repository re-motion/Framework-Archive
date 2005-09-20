using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.PerformanceTests.TestDomain;

namespace Rubicon.Data.DomainObjects.PerformanceTests
{
[TestFixture]
public class LoadObjectsTest
{
  // types

  // static members and constants

  private static ObjectID s_clientID = new ObjectID ("Client", new Guid ("6F20355F-FA99-4c4e-B432-02C41F7BD390"));
  private static ObjectID s_fileID = new ObjectID ("File", new Guid ());

  // member fields

  private Client _client;

  // construction and disposing

  public LoadObjectsTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    ClientTransaction.SetCurrent (null);
    _client = Client.GetObject (s_clientID);
  }

  [Test]
  public void LoadObjectsOverRelationTest ()
  {
    DateTime startTime = DateTime.Now;

    DomainObjectCollection files = _client.Files;

    DateTime endTime = DateTime.Now;
    TimeSpan elapsedTime = endTime - startTime;

    Console.WriteLine ("Zeit über DateTime in Millisekunden: {0}", elapsedTime.TotalMilliseconds.ToString ("n"));
  }
}
}
