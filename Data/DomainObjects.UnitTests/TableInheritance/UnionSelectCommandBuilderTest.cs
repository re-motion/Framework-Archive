using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class UnionSelectCommandBuilderTest : SqlProviderBaseTest
  {
    private UnionSelectCommandBuilder _builder;

    public override void SetUp ()
    {
      base.SetUp ();

      ClassDefinition domainBaseClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));

      _builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, domainBaseClass, domainBaseClass.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.DomainBase.Client"), DomainObjectIDs.Client);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_builder.Provider, Is.SameAs (Provider));
      Assert.That (((CommandBuilder) _builder).UsesView, Is.False);
    }

    [Test]
    public void Create ()
    {
      // Note: This test builds its own relations without a sort expression.
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, c_testDomainProviderID, typeof (DomainBase), false);

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false, domainBaseClass);

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", c_testDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      ReflectionBasedClassDefinition clientClass = new ReflectionBasedClassDefinition ("Client", "TableInheritance_Client", c_testDomainProviderID, typeof (Client), false);

      domainBaseClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (domainBaseClass, "Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.DomainBase.Client", "ClientID", typeof (ObjectID)));

      RelationEndPointDefinition domainBaseEndPointDefinition = new RelationEndPointDefinition (domainBaseClass, "Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.DomainBase.Client", false);

      VirtualRelationEndPointDefinition clientEndPointDefinition = new VirtualRelationEndPointDefinition (
          clientClass, "AssignedObjects", false, CardinalityType.Many, typeof (DomainObjectCollection));

      RelationDefinition clientToDomainBaseDefinition = new RelationDefinition (
          "ClientToDomainBase", clientEndPointDefinition, domainBaseEndPointDefinition);

      domainBaseClass.MyRelationDefinitions.Add (clientToDomainBaseDefinition);
      clientClass.MyRelationDefinitions.Add (clientToDomainBaseDefinition);

      UnionSelectCommandBuilder builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, domainBaseClass, domainBaseClass.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.DomainBase.Client"), DomainObjectIDs.Client);

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
