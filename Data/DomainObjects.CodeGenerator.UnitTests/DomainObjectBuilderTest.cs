using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests
{
  [TestFixture]
  public class DomainObjectBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private StringWriter _stringWriter;
    private DomainObjectBuilder _domainObjectBuilder;

    // construction and disposing

    public DomainObjectBuilderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _stringWriter = new StringWriter ();
      _domainObjectBuilder = new DomainObjectBuilder (MappingConfiguration, _stringWriter);
    }

    public override void TearDown ()
    {
      base.TearDown ();

      _stringWriter.Dispose ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (MappingConfiguration, _domainObjectBuilder.MappingConfiguration);
    }

    [Test]
    public void GetAllDistinctPropertyTypeNames ()
    {
      List<TypeName> actualdistinctPropertyTypeNames = _domainObjectBuilder.GetAllDistinctPropertyTypeNames ();

      string customerQualifiedName =
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests.TestDomain.Customer+CustomerType, Rubicon.Data.DomainObjects.CodeGenerator.UnitTests";
      string orderPriorityQualifiedName =
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests.TestDomain.OrderPriority, Rubicon.Data.DomainObjects.CodeGenerator.UnitTests";
      string enumTypeQualifiedName =
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests.TestDomain.ClassWithAllDataTypes+EnumType, Rubicon.Data.DomainObjects.CodeGenerator.UnitTests";

      Assert.AreEqual (3, actualdistinctPropertyTypeNames.Count);
      Assert.AreEqual (customerQualifiedName, actualdistinctPropertyTypeNames[0].AssemblyQualifiedName);
      Assert.AreEqual (orderPriorityQualifiedName, actualdistinctPropertyTypeNames[1].AssemblyQualifiedName);
      Assert.AreEqual (enumTypeQualifiedName, actualdistinctPropertyTypeNames[2].AssemblyQualifiedName);
    }

    [Test]
    public void GetNestedPropertyTypeNames ()
    {
      TypeName typeName = new TypeName (
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests.TestDomain.Customer, Rubicon.Data.DomainObjects.CodeGenerator.UnitTests");

      List<TypeName> nestedPropertyTypeNames = _domainObjectBuilder.GetNestedPropertyTypeNames (typeName);

      string nestedTypeQualifiedName =
          "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests.TestDomain.Customer+CustomerType, Rubicon.Data.DomainObjects.CodeGenerator.UnitTests";

      Assert.AreEqual (1, nestedPropertyTypeNames.Count);
      Assert.AreEqual (nestedTypeQualifiedName, nestedPropertyTypeNames[0].AssemblyQualifiedName);
    }

  }
}
