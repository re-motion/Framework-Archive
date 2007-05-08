using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class UpdateCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);
      new UpdateCommandBuilder (Provider, order.DataContainer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "State of provided DataContainer must not be 'Unchanged'.\r\nParameter name: dataContainer")]
    public void InitializeWithDataContainerOfInvalidState ()
    {
      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);

      Provider.Connect ();
      new UpdateCommandBuilder (Provider, order.DataContainer);
    }
  }
}
