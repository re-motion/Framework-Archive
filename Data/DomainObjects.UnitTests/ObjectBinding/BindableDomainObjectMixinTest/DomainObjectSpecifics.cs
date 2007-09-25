using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject.Properties;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTest
{
  [TestFixture]
  public class DomainObjectSpecifics
  {
    [Test]
    public void OrdinaryProperty ()
    {
      BindableObjectClass businessObjectClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (BindableSampleDomainObject));
      Assert.IsTrue (businessObjectClass.HasPropertyDefinition ("Name"));
    }

    [Test]
    [Ignore ("TODO: Implement custom PropertyFinder")]
    public void NoIDProperty ()
    {
      BindableObjectClass businessObjectClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (BindableSampleDomainObject));
      Assert.IsFalse (businessObjectClass.HasPropertyDefinition ("ID"));
    }

    [Test]
    [Ignore ("TODO: Implement custom PropertyFinder")]
    public void NoPropertyFromDomainObject ()
    {
      BindableObjectClass businessObjectClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (BindableSampleDomainObject));
      PropertyBase[] properties = (PropertyBase[]) businessObjectClass.GetPropertyDefinitions ();

      foreach (PropertyBase property in properties)
        Assert.AreNotEqual (typeof (DomainObject), property.PropertyInfo.DeclaringType);
    }

    [Test]
    [Ignore ("TODO: Implement custom PropertyReflector")]
    public void NullabilityFromMapping ()
    {
      Assert.Fail ();
    }

    [Test]
    [Ignore ("TODO: Implement custom PropertyReflector")]
    public void RequirednessFromMapping ()
    {
      Assert.Fail ();
    }

    [Test]
    [Ignore ("TODO: Implement custom PropertyReflector")]
    public void MaxLengthFromMapping ()
    {
      Assert.Fail ();
    }
  }
}