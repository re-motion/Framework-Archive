using System;
using System.Data;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
[TestFixture]
public class DeleteCommandBuilderTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private SqlProvider _provider;

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
  }

  public override void TearDown()
  {
    base.TearDown ();

    _provider.Dispose ();
  }

  [Test]
  public void CreateWithoutForeignKeyColumn ()
  {
    ClassWithAllDataTypes classWithAllDataTypes = 
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

    classWithAllDataTypes.Delete ();
    DataContainer deletedContainer = classWithAllDataTypes.DataContainer;
    CommandBuilder commandBuilder = new DeleteCommandBuilder (_provider, deletedContainer);

    using (IDbCommand deleteCommand = commandBuilder.Create ())
    {
      string expectedCommandText = "DELETE FROM [TableWithAllDataTypes] WHERE [ID] = @ID AND [Timestamp] = @Timestamp;";
      Assert.AreEqual (expectedCommandText, deleteCommand.CommandText);

      Assert.AreEqual (2, deleteCommand.Parameters.Count);

      IDataParameter idParameter = (IDataParameter) deleteCommand.Parameters["@ID"];
      IDataParameter timestampParameter = (IDataParameter) deleteCommand.Parameters["@Timestamp"];

      Assert.AreEqual (deletedContainer.ID.Value, idParameter.Value);
      Assert.AreEqual (deletedContainer.Timestamp, timestampParameter.Value);
    }
  }

  [Test]
  public void CreateWithForeignKeyColumn ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    order.Delete ();
    DataContainer deletedOrderContainer = order.DataContainer;
    CommandBuilder commandBuilder = new DeleteCommandBuilder (_provider, deletedOrderContainer);

    using (IDbCommand deleteCommand = commandBuilder.Create ())
    {
      string expectedCommandText = "DELETE FROM [Order] WHERE [ID] = @ID;";
      Assert.AreEqual (expectedCommandText, deleteCommand.CommandText);

      Assert.AreEqual (1, deleteCommand.Parameters.Count);

      IDataParameter idParameter = (IDataParameter) deleteCommand.Parameters["@ID"];

      Assert.AreEqual (deletedOrderContainer.ID.Value, idParameter.Value);
    }
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void InitializeWithDataContainerOfInvalidState ()
  {
    CommandBuilder commandBuilder = new DeleteCommandBuilder (_provider, TestDataContainerFactory.CreateOrder1DataContainer ());
  }
}
}
