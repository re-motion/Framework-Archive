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

      _client = DomainObject.GetObject<Client> (DomainObjectIDs.Client3);
      _clientToLocationDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Location)].GetRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      _clientEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Client", null);
      _locationEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Location", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
    }

    [Test]
    public void InitializeWithDomainObject ()
    {
      AnonymousEndPoint endPoint = new AnonymousEndPoint (_client, _clientToLocationDefinition);

      Assert.IsNotNull (endPoint as INullObject);
      Assert.IsNotNull (endPoint as IEndPoint);
      Assert.IsFalse (endPoint.IsNull);
      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.AreSame (_client, endPoint.GetDomainObject ());
      Assert.AreSame (_client.DataContainer, endPoint.GetDataContainer ());
      Assert.AreEqual (_client.ID, endPoint.ObjectID);

      Assert.AreSame (_clientToLocationDefinition, endPoint.RelationDefinition);
      Assert.AreSame (_clientEndPointDefinition, endPoint.Definition);
      Assert.IsNotNull (endPoint.Definition as AnonymousRelationEndPointDefinition);
    }

    [Test]
    public void InitializeWithDataContainer ()
    {
      AnonymousEndPoint endPoint = new AnonymousEndPoint (_client.DataContainer, _clientToLocationDefinition);

      Assert.AreSame (_client.DataContainer, endPoint.GetDataContainer ());
    }

    [Test]
    public void InitializeWithObjectID ()
    {
      AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, _client.ID, _clientToLocationDefinition);

      Assert.AreSame (_client.ID, endPoint.ObjectID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The provided relation definition must contain a AnonymousRelationEndPointDefinition.\r\nParameter name: relationDefinition")]
    public void InitializeWithInvalidRelationDefinition ()
    {
      RelationDefinition invalidRelationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
      AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, DomainObjectIDs.Order1, invalidRelationDefinition);
    }
  }
}
