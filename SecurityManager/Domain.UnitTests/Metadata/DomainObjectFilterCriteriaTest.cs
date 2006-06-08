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
  public class DomainObjectFilterCriteriaTest : DomainTest
  {
    private DomainObjectFilterCriteria _filterCriteria;

    public override void SetUp ()
    {
      base.SetUp ();

      _filterCriteria = new DomainObjectFilterCriteria (typeof (EnumValueDefinition));
    }

    [Test]
    public void ObjectSatisfiesType ()
    {
      EnumValueDefinition enumValueDefinition = new EnumValueDefinition (ClientTransaction.Current);
      Assert.IsTrue (_filterCriteria.IsSatisfied (enumValueDefinition));
    }

    [Test]
    public void ObjectDoesNotSatisfyType ()
    {
      SecurableClassDefinition securableClassDefinition = new SecurableClassDefinition (ClientTransaction.Current);
      Assert.IsFalse (_filterCriteria.IsSatisfied (securableClassDefinition));
    }

    [Test]
    public void ObjectDoesNotSatisfyProperty ()
    {
      _filterCriteria.ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0001-000000000000"));

      EnumValueDefinition enumValueDefinition = new EnumValueDefinition (ClientTransaction.Current);
      enumValueDefinition.MetadataItemID = new Guid ("00000000-0000-0000-0002-000000000000");

      Assert.IsFalse (_filterCriteria.IsSatisfied (enumValueDefinition));
    }

    [Test]
    public void ObjectSatisfiesDerivedType ()
    {
      AbstractRoleDefinition abstractRoleDefinition = new AbstractRoleDefinition (ClientTransaction.Current);
      Assert.IsTrue (_filterCriteria.IsSatisfied (abstractRoleDefinition));
    }
  }
}
