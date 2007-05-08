using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class InsertCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      Order order = Order.NewObject ();
      new InsertCommandBuilder (Provider, order.DataContainer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "State of provided DataContainer must be 'New', but is 'Unchanged'.\r\nParameter name: dataContainer")]
    public void InitializeWithDataContainerOfInvalidState ()
    {
      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);

      Provider.Connect ();
      new InsertCommandBuilder (Provider, order.DataContainer);
    }
  }
}
