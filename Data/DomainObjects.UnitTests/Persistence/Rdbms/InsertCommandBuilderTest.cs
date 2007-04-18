using System;
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class InsertCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void UsesView ()
    {
      Order order = Order.NewObject ();

      Provider.Connect ();
      CommandBuilder commandBuilder = new InsertCommandBuilder (Provider, order.DataContainer);
      Assert.That (commandBuilder.UsesView, Is.False);
    }

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
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Provider.Connect ();
      new InsertCommandBuilder (Provider, order.DataContainer);
    }
  }
}
