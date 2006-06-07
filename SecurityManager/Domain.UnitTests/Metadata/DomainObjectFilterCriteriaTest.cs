using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;

using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.UnitTests.Metadata
{
      //DomainObjectFilter filter = new DomainObjectFilter (typeof (SecurableClassDefinition));
      //filter.ExpectPropertyValue ("MetadataItemID", new Guid(""));
      //filter.ExpectPropertyValue ("Name", "blabla");
      //DomainObject importedObject = filter.In (importedObjects);
      //RpfAssert.Contains (filter, importedObjects);

      //RpfAssert.Contains (DomainObjectFilter.ExpectType (typeof (SecurableClassDefinition))
      //    .ExpectPropertyValue ("MetadataItemID", new Guid (""))
      //    .ExpectPropertyValue ("Name", "blabla"), importedObjects);

      //DomainObject importedObject = DomainObjectFilter.ExpectType (typeof (SecurableClassDefinition))
      //    .ExpectPropertyValue ("MetadataItemID", new Guid (""))
      //    .ExpectPropertyValue ("Name", "blabla").In (importedObjects);

  [TestFixture]
  public class DomainObjectFilterCriteriaTest
  {
    private DomainObjectFilterCriteria _filterCriteria;

    [SetUp]
    public void SetUp ()
    {
      _filterCriteria = new DomainObjectFilterCriteria (typeof (EnumValueDefinitionWithIdentity));
    }

    [Test]
    public void ObjectSatisfiesType ()
    {
      EnumValueDefinitionWithIdentity enumValueDefinition = new EnumValueDefinitionWithIdentity ();
      Assert.IsTrue (_filterCriteria.IsSatisfied (enumValueDefinition));
    }

    [Test]
    public void ObjectDoesNotSatisfyType ()
    {
      SecurableClassDefinition securableClassDefinition = new SecurableClassDefinition ();
      Assert.IsFalse (_filterCriteria.IsSatisfied (securableClassDefinition));
    }

    [Test]
    public void ObjectDoesNotSatisfyProperty ()
    {
      _filterCriteria.ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0001-000000000000"));

      EnumValueDefinitionWithIdentity enumValueDefinition = new EnumValueDefinitionWithIdentity ();
      enumValueDefinition.MetadataItemID = new Guid ("00000000-0000-0000-0002-000000000000");

      Assert.IsFalse (_filterCriteria.IsSatisfied (enumValueDefinition));
    }

    [Test]
    public void ObjectSatisfiesDerivedType ()
    {
      AbstractRoleDefinition abstractRoleDefinition = new AbstractRoleDefinition ();
      Assert.IsTrue (_filterCriteria.IsSatisfied (abstractRoleDefinition));
    }
  }
}
