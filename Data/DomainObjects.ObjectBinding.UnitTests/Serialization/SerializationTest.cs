using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.Serialization
{
[TestFixture]
public class SerializationTest : DatabaseTest
{
  // types

  // static members and constants

  // member fields

  private ClientTransactionScope _transactionScope;

  // construction and disposing

  public SerializationTest ()
  {
  }

  public override void SetUp ()
  {
    base.SetUp ();
    _transactionScope = new ClientTransactionScope ();
  }

  public override void TearDown ()
  {
    _transactionScope.Leave ();
  }

  // methods and properties

  [Test]
  public void BindableDomainObjectTest ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);

    Order deserializedOrder = (Order) SerializeAndDeserialize (order);

    Assert.IsNotNull (deserializedOrder);
  }

  [Test]
  [Ignore ("TODO: FS: This will be fixed when DomainObject is decoupled from DataContainer.")]
  public void BindableDomainObjectWithReflector ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);

    Order deserializedOrder = (Order) SerializeAndDeserialize (order);

    Assert.AreEqual (((IBusinessObject) order).GetProperty ("OrderNumber"), ((IBusinessObject) deserializedOrder).GetProperty ("OrderNumber"));
  }

  [Test]
  public void BindableSearchObjectTest ()
  {
    TestSearchObject testSearchObject = new TestSearchObject ();

    TestSearchObject deserializedTestSearchObject = (TestSearchObject) SerializeAndDeserialize (testSearchObject);

    Assert.IsNotNull (deserializedTestSearchObject);
  }

  [Test]
  public void BindableSearchObjectWithReflector ()
  {
    TestSearchObject testSearchObject = new TestSearchObject ();
    testSearchObject.StringProperty = "test";

    TestSearchObject deserializedTestSearchObject = (TestSearchObject) SerializeAndDeserialize (testSearchObject);

    Assert.AreEqual (((IBusinessObject) testSearchObject).GetProperty ("StringProperty"), ((IBusinessObject) deserializedTestSearchObject).GetProperty ("StringProperty"));
  }

  private object SerializeAndDeserialize  (object graph)
  {
    using (MemoryStream stream = new MemoryStream ())
    {
      BinaryFormatter serializationFormatter = new BinaryFormatter ();
      serializationFormatter.Serialize (stream, graph);

      stream.Position = 0;

      BinaryFormatter deserializationFormatter = new BinaryFormatter ();
      return deserializationFormatter.Deserialize (stream);
    }
  }
}
}
