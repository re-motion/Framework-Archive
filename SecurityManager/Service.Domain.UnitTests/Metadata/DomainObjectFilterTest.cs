using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;
using Rubicon.Security.Service.Domain.Metadata;

namespace Rubicon.Security.Service.Domain.UnitTests.Metadata
{
  [TestFixture]
  public class DomainObjectFilterTest
  {
    private DomainObjectCollection _collection;
    private EnumValueDefinitionWithIdentity _object1;
    private EnumValueDefinitionWithIdentity _object2;
    private SecurableClassDefinition _securableClassDefinition;

    private DomainObjectFilter _filter;

    [SetUp]
    public void SetUp ()
    {
      _collection = new DomainObjectCollection ();

      _object1 = new EnumValueDefinitionWithIdentity ();
      _object1.MetadataItemID = new Guid ("00000000-0000-0000-0001-000000000000");
      _object1.Name = "Class1";

      _object2 = new EnumValueDefinitionWithIdentity ();
      _object2.MetadataItemID = new Guid ("00000000-0000-0000-0002-000000000000");
      _object2.Name = "Class2";

      _securableClassDefinition = new SecurableClassDefinition ();

      _collection.Add (_object1);
      _collection.Add (_object2);
      _collection.Add (_securableClassDefinition);

      _filter = new DomainObjectFilter (_collection);
    }

    [Test]
    public void FilterObjectsByType ()
    {
      DomainObjectFilterCriteria filterCriteria = new DomainObjectFilterCriteria (typeof (EnumValueDefinitionWithIdentity));
      DomainObjectCollection result = _filter.GetObjects (filterCriteria);

      Assert.AreEqual (2, result.Count);
      Assert.Contains (_object1, result);
      Assert.Contains (_object2, result);
    }

    [Test]
    public void FilterObjectsByProperty ()
    {
      DomainObjectFilterCriteria filterCriteria = new DomainObjectFilterCriteria (typeof (EnumValueDefinitionWithIdentity));
      filterCriteria.ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0002-000000000000"));
      filterCriteria.ExpectPropertyValue ("Name", "Class2");

      DomainObjectCollection result = _filter.GetObjects (filterCriteria);

      Assert.AreEqual (1, result.Count);
      Assert.Contains (_object2, result);
    }

    [Test]
    public void FilterObjectsByPropertyWithNoResult ()
    {
      DomainObjectFilterCriteria filterCriteria = new DomainObjectFilterCriteria (typeof (EnumValueDefinitionWithIdentity));
      filterCriteria.ExpectPropertyValue ("MetadataItemID", new Guid ("00000000-0000-0000-0002-000000000000"));
      filterCriteria.ExpectPropertyValue ("Name", "Clas1");

      DomainObjectCollection result = _filter.GetObjects (filterCriteria);

      Assert.AreEqual (0, result.Count);
    }

    [Test]
    public void FilterObject ()
    {
      DomainObjectFilterCriteria filterCriteria = new DomainObjectFilterCriteria (typeof (SecurableClassDefinition));
      DomainObject result = _filter.GetObject (filterCriteria);

      Assert.AreSame (_securableClassDefinition, result);
    }

    [Test]
    public void FilterObjectWithNoResult ()
    {
      DomainObjectFilterCriteria filterCriteria = new DomainObjectFilterCriteria (typeof (StatePropertyDefinition));
      DomainObject result = _filter.GetObject (filterCriteria);

      Assert.IsNull (result);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Multiple objects satisfy the filter criteria.\r\nParameter name: criteria")]
    public void GetExceptionWhenMultipleObjectsSatisfyFilterCriteriaForFilterDomainObject ()
    {
      DomainObjectFilterCriteria filterCriteria = new DomainObjectFilterCriteria (typeof (EnumValueDefinitionWithIdentity));
      DomainObject result = _filter.GetObject (filterCriteria);
    }
  }
}
