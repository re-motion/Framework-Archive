using System;
using System.Data;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class DeleteCommandBuilderTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private SqlProvider _provider;
  private DataContainer _deletedOrderContainer;
  private DeleteCommandBuilder _commandBuilder;

  // construction and disposing

  public DeleteCommandBuilderTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    RdbmsProviderDefinition definition = new RdbmsProviderDefinition (
        c_testDomainProviderID, typeof (SqlProvider), c_connectionString);

    _provider = new SqlProvider (definition);
    _provider.Connect ();

    Order order = Order.GetObject (DomainObjectIDs.Order1);
    order.Delete ();
    _deletedOrderContainer = order.DataContainer;

    _commandBuilder = new DeleteCommandBuilder (_provider, _deletedOrderContainer);
  }

  public override void TearDown()
  {
    base.TearDown ();

    _provider.Dispose ();
  }

  [Test]
  public void Create ()
  {
    using (IDbCommand deleteCommand = _commandBuilder.Create ())
    {
      string expectedCommandText = "DELETE FROM [Order] WHERE [ID] = @ID AND [Timestamp] = @Timestamp;";
      Assert.AreEqual (expectedCommandText, deleteCommand.CommandText);

      Assert.AreEqual (2, deleteCommand.Parameters.Count);

      IDataParameter idParameter = (IDataParameter) deleteCommand.Parameters["@ID"];
      IDataParameter timestampParameter = (IDataParameter) deleteCommand.Parameters["@Timestamp"];

      Assert.AreEqual (_deletedOrderContainer.ID.Value, idParameter.Value);
      Assert.AreEqual (_deletedOrderContainer.Timestamp, timestampParameter.Value);
    }
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void InitializeWithDataContainerOfInvalidState ()
  {
    _commandBuilder = new DeleteCommandBuilder (_provider, TestDataContainerFactory.CreateOrder1DataContainer ());
  }
}
}
