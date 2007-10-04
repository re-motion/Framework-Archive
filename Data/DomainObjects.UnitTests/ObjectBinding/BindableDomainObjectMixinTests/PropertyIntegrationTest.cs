using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class PropertyIntegrationTest : ObjectBindingBaseTest
  {
    [Test]
    public void TestPropertyAccess ()
    {
      BindableSampleDomainObject instance = BindableSampleDomainObject.NewObject ();
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;
      
      instanceAsIBusinessObject.SetProperty ("Name", "Fritz");
      Assert.AreEqual ("Fritz", instance.Name);

      instance.Name = "Bert";
      Assert.AreEqual ("Bert", instanceAsIBusinessObject.GetProperty ("Name"));

      instance.Name = "Fred";
      Assert.AreEqual ("Fred", instanceAsIBusinessObject.GetPropertyString ("Name"));
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      BindableObjectClass bindableObjectClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (BindableSampleDomainObject));
      IBusinessObjectProperty[] properties = bindableObjectClass.GetPropertyDefinitions ();
      string[] propertiesByName =
          Array.ConvertAll<IBusinessObjectProperty, string> (properties, delegate (IBusinessObjectProperty property) { return property.Identifier; });
      Assert.That (propertiesByName, Is.EquivalentTo (new string[] { "Name" }));
    }
  }
}