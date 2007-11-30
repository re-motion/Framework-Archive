using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class AnonymousEndPointTest : ClientTransactionBaseTest
  {
    private Client _client;
    private RelationDefinition _clientToLocationDefinition;
    private IRelationEndPointDefinition _clientEndPointDefinition;
    private IRelationEndPointDefinition _locationEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _client = Client.GetObject (DomainObjectIDs.Client3);
      _clientToLocationDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Location)].GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      _clientEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Client", null);
      _locationEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Location", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
    }

    [Test]
    public void InitializeWithDomainObject ()
    {
      AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, _client, _clientToLocationDefinition);

      Assert.IsNotNull (endPoint as INullObject);
      Assert.IsNotNull (endPoint as IEndPoint);
      Assert.IsFalse (endPoint.IsNull);
      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.AreSame (_client, endPoint.GetDomainObject ());
      Assert.AreSame (_client.InternalDataContainer, endPoint.GetDataContainer ());
      Assert.AreEqual (_client.ID, endPoint.ObjectID);

      Assert.AreSame (_clientToLocationDefinition, endPoint.RelationDefinition);
      Assert.AreSame (_clientEndPointDefinition, endPoint.Definition);
      Assert.IsNotNull (endPoint.Definition as AnonymousRelationEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The provided relation definition must contain a AnonymousRelationEndPointDefinition.\r\nParameter name: relationDefinition")]
    public void InitializeWithInvalidRelationDefinition ()
    {
      RelationDefinition invalidRelationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
      AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, Order.GetObject (DomainObjectIDs.Order1), invalidRelationDefinition);
    }

    [Test]
    public void GetDataContainerUsesStoredTransaction ()
    {
      AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, _client, _clientToLocationDefinition);
      using (ClientTransaction.NewTransaction ().EnterDiscardingScope ())
      {
        Assert.AreSame (ClientTransactionMock, endPoint.GetDataContainer ().ClientTransaction);
      }
    }
  }
}
