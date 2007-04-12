using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  [Ignore]
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
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, c_testDomainProviderID, typeof (DomainBase), false);

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          (string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false, domainBaseClass);

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          (string) "OrganizationalUnit", (string) "TableInheritance_OrganizationalUnit", (string) c_testDomainProviderID, typeof (OrganizationalUnit), (bool) false, domainBaseClass);

      ReflectionBasedClassDefinition clientClass = new ReflectionBasedClassDefinition ((string) "Client", (string) "TableInheritance_Client", (string) c_testDomainProviderID, typeof (Client), (bool) false);

      domainBaseClass.MyPropertyDefinitions.Add (new PropertyDefinition ("Client", "ClientID", TypeInfo.ObjectIDMappingTypeName));

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
