using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTest
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
  }
}