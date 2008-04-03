using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  [TestFixture]
  public class UnionSelectCommandBuilderTest : SqlProviderBaseTest
  {
    // types

    // static members and constants

    // member fields

    private UnionSelectCommandBuilder _builder;

    // construction and disposing

    public UnionSelectCommandBuilderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      ClassDefinition domainBaseClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));

      _builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, domainBaseClass, domainBaseClass.GetMandatoryPropertyDefinition ("Client"), DomainObjectIDs.Client);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (Provider, _builder.Provider);
    }

    [Test]
    public void Create ()
    {
      // Note: This test builds its own relations without a sort expression.
      XmlBasedClassDefinition domainBaseClass = new XmlBasedClassDefinition ("DomainBase", null, c_testDomainProviderID, typeof (DomainBase));

      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition (
          "Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);

      XmlBasedClassDefinition organizationalUnitClass = new XmlBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", c_testDomainProviderID, typeof (OrganizationalUnit), domainBaseClass);

      XmlBasedClassDefinition clientClass = new XmlBasedClassDefinition ("Client", "TableInheritance_Client", c_testDomainProviderID, typeof (Client));

      domainBaseClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (domainBaseClass, "Client", "ClientID", TypeInfo.ObjectIDMappingTypeName));

      RelationEndPointDefinition domainBaseEndPointDefinition = new RelationEndPointDefinition (domainBaseClass, "Client", false);

      VirtualRelationEndPointDefinition clientEndPointDefinition = new VirtualRelationEndPointDefinition (
          clientClass, "AssignedObjects", false, CardinalityType.Many, typeof (DomainObjectCollection));

      RelationDefinition clientToDomainBaseDefinition = new RelationDefinition (
          "ClientToDomainBase", clientEndPointDefinition, domainBaseEndPointDefinition);

      domainBaseClass.MyRelationDefinitions.Add (clientToDomainBaseDefinition);
      clientClass.MyRelationDefinitions.Add (clientToDomainBaseDefinition);

      UnionSelectCommandBuilder builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, domainBaseClass, domainBaseClass.GetMandatoryPropertyDefinition ("Client"), DomainObjectIDs.Client);

      using (IDbCommand command = builder.Create ())
      {
        string expectedCommandText = 
            "SELECT [ID], [ClassID] FROM [TableInheritance_Person] WHERE [ClientID] = @ClientID\n"
            + "UNION ALL SELECT [ID], [ClassID] FROM [TableInheritance_OrganizationalUnit] WHERE [ClientID] = @ClientID;";

        Assert.IsNotNull (command);
        Assert.AreEqual (expectedCommandText, command.CommandText);
        Assert.AreEqual (1, command.Parameters.Count);
        Assert.AreEqual ("@ClientID", ((SqlParameter) command.Parameters[0]).ParameterName);
        Assert.AreEqual (DomainObjectIDs.Client.Value, ((SqlParameter) command.Parameters[0]).Value);
      }
    }

    [Test]
    public void CreateWithSortExpression ()
    {
      using (IDbCommand command = _builder.Create ())
      {
        string expectedCommandText =
            "SELECT [ID], [ClassID], CreatedAt FROM [TableInheritance_Person] WHERE [ClientID] = @ClientID\n"
            + "UNION ALL SELECT [ID], [ClassID], CreatedAt FROM [TableInheritance_OrganizationalUnit] WHERE [ClientID] = @ClientID ORDER BY CreatedAt asc;";

        Assert.IsNotNull (command);
        Assert.AreEqual (expectedCommandText, command.CommandText);
        Assert.AreEqual (1, command.Parameters.Count);
        Assert.AreEqual ("@ClientID", ((SqlParameter) command.Parameters[0]).ParameterName);
        Assert.AreEqual (DomainObjectIDs.Client.Value, ((SqlParameter) command.Parameters[0]).Value);
      }
    }

  }
}
