using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class ConcreteTableInheritanceRelationLoaderTest : SqlProviderBaseTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinition _domainBaseClass;
    private ConcreteTableInheritanceRelationLoader _loader;

    // construction and disposing

    public ConcreteTableInheritanceRelationLoaderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _domainBaseClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));

      _loader = new ConcreteTableInheritanceRelationLoader (
          Provider, _domainBaseClass, _domainBaseClass.GetMandatoryPropertyDefinition ("Client"), DomainObjectIDs.Client);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (Provider, _loader.Provider);
    }

    [Test]
    public void LoadDataContainers ()
    {
      DataContainerCollection dataContainers = _loader.LoadDataContainers ();

      Assert.IsNotNull (dataContainers);
      Assert.AreEqual (3, dataContainers.Count);
      Assert.IsTrue (dataContainers.Contains (DomainObjectIDs.Customer));
      Assert.IsTrue (dataContainers.Contains (DomainObjectIDs.Person));
      Assert.IsTrue (dataContainers.Contains (DomainObjectIDs.OrganizationalUnit));
    }
  }
}
