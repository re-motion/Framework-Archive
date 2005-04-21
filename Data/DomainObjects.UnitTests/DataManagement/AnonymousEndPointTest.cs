using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class AnonymousEndPointTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private Client _client;
  private RelationDefinition _clientToLocationDefinition;
  private IRelationEndPointDefinition _clientEndPointDefinition;
  private IRelationEndPointDefinition _locationEndPointDefinition;

  // construction and disposing

  public AnonymousEndPointTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _client = Client.GetObject (DomainObjectIDs.Client3);
    _clientToLocationDefinition = MappingConfiguration.Current.ClassDefinitions["Location"].GetRelationDefinition ("Client");
    _clientEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Client", null);
    _locationEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Location", "Client");
  }

  [Test]
  public void InitializeWithDomainObject ()
  {
    AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, _client, _clientToLocationDefinition);

    Assert.IsNotNull (endPoint as INullableObject);
    Assert.IsNotNull (endPoint as IEndPoint);
    Assert.IsFalse (endPoint.IsNull);
    Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
    Assert.AreSame (_client, endPoint.GetDomainObject ());
    Assert.AreSame (_client.DataContainer, endPoint.GetDataContainer ());
    Assert.AreEqual (_client.ID, endPoint.ObjectID);

    Assert.AreEqual (_clientEndPointDefinition.ClassDefinition, endPoint.ClassDefinition);
    Assert.AreSame (_clientToLocationDefinition, endPoint.RelationDefinition);
    Assert.AreSame (_clientEndPointDefinition, endPoint.Definition);
    Assert.IsNotNull (endPoint.Definition as NullRelationEndPointDefinition);
  }

  [Test]
  public void InitializeWithDataContainer ()
  {
    AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, _client.DataContainer, _clientToLocationDefinition);

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
      "The provided relation definition must contain a NullRelationEndPointDefinition.\r\nParameter name: relationDefinition")]
  public void InitializeWithInvalidRelationDefinition ()
  {
    RelationDefinition invalidRelationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory ("OrderToOrderTicket");
    AnonymousEndPoint endPoint = new AnonymousEndPoint (ClientTransactionMock, DomainObjectIDs.Order1, invalidRelationDefinition);
  }
}
}
