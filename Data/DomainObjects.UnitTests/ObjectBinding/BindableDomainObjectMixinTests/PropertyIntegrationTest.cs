using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class PropertyIntegrationTest : ObjectBindingBaseTest
  {
    [Test]
    public void TestPropertyAccess ()
    {
      BindableSampleDomainObject instance = BindableSampleDomainObject.NewObject ();
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.IsNull (instanceAsIBusinessObject.GetProperty ("Int32"));

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (0, instance.Int32);
        Assert.AreEqual (0, instanceAsIBusinessObject.GetProperty ("Int32"));
      }

      instanceAsIBusinessObject.SetProperty ("Int32", 1);
      Assert.AreEqual (1, instance.Int32);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (1, instance.Int32);
        Assert.AreEqual (1, instanceAsIBusinessObject.GetProperty ("Int32"));
      }

      instance.Int32 = 2;
      Assert.AreEqual (2, instanceAsIBusinessObject.GetProperty ("Int32"));
      Assert.AreEqual ("2", instanceAsIBusinessObject.GetPropertyString ("Int32"));

      instance.Int32 = 1;
      Assert.AreEqual (1, instanceAsIBusinessObject.GetProperty ("Int32"));

      instance.Int32 = 0;
      Assert.AreEqual (0, instanceAsIBusinessObject.GetProperty ("Int32"));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (0, instance.Int32);
        Assert.AreEqual (0, instanceAsIBusinessObject.GetProperty ("Int32"));
      }
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      BindableObjectClass bindableObjectClass = BindableObjectProvider.GetBindableObjectClassFromProvider (typeof (BindableSampleDomainObject));
      IBusinessObjectProperty[] properties = bindableObjectClass.GetPropertyDefinitions ();
      string[] propertiesByName =
          Array.ConvertAll<IBusinessObjectProperty, string> (properties, delegate (IBusinessObjectProperty property) { return property.Identifier; });
      Assert.That (propertiesByName, Is.EquivalentTo (new string[] { "Name", "Int32" }));
    }
  }
}