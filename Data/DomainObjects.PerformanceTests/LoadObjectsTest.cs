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

  // member fields

  private Client _client;

  // construction and disposing

  public LoadObjectsTest ()
  {
  }

  // methods and properties

  // TODO: implement this
//  [SetUp]
//  public void SetUp ()
//  {
//    ClientTransaction.SetCurrent (null);
//    _client = Client.GetObject (s_clientID);
//  }
//
//  [Test]
//  public void LoadObjectsOverRelationTest ()
//  {
//    DateTime startTime = DateTime.Now;
//    long startTicks = DateTime.Now.Ticks;
//
//    DomainObjectCollection files = _client.Files;
//
//    DateTime endTime = DateTime.Now;
//    long endTicks = DateTime.Now.Ticks;
//
//    TimeSpan elapsedTime = endTime - startTime;
//
//    Console.WriteLine ("Zeit in Millisekunden: {0}", elapsedTime.Milliseconds);
//  }

}
}
