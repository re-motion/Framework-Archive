using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class NullAnonymousEndPointTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private RelationDefinition _clientToLocationDefinition;
    private IRelationEndPointDefinition _clientEndPointDefinition;
    private IRelationEndPointDefinition _locationEndPointDefinition;

    // construction and disposing

    public NullAnonymousEndPointTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _clientToLocationDefinition = MappingConfiguration.Current.ClassDefinitions["Location"].GetRelationDefinition ("Client");
      _clientEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Client", null);
      _locationEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Location", "Client");
    }

    [Test]
    public void Initialize ()
    {
      NullAnonymousEndPoint endPoint = new NullAnonymousEndPoint (_clientToLocationDefinition);

      Assert.IsNotNull (endPoint as INullableObject);
      Assert.IsNotNull (endPoint as IEndPoint);
      Assert.IsNotNull (endPoint as AnonymousEndPoint);
      Assert.IsTrue (endPoint.IsNull);
      Assert.IsNull (endPoint.ClientTransaction);
      Assert.IsNull (endPoint.GetDomainObject ());
      Assert.IsNull (endPoint.GetDataContainer ());
      Assert.IsNull (endPoint.ObjectID);

      Assert.AreSame (_clientToLocationDefinition, endPoint.RelationDefinition);
      Assert.AreSame (_clientEndPointDefinition, endPoint.Definition);
      Assert.IsNotNull (endPoint.Definition as NullRelationEndPointDefinition);
    }
  }
}
