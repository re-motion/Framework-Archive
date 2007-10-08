using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class DefaultValueTest : ObjectBindingBaseTest
  {
    private Order _order;
    private IBusinessObject _businessOrder;

    public override void SetUp ()
    {
      base.SetUp ();
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (BindableDomainObjectMixin)))
      {
        _order = Order.GetObject (DomainObjectIDs.Order1);
        _businessOrder = (IBusinessObject) _order;
      }
    }

    [Test]
    public void GetPropertyReturnsNullIfDefaultValue ()
    {
      Assert.IsNull (_businessOrder.GetProperty ("OrderNumber"));
    }

    [Test]
    public void GetPropertyReturnsNonNullIfNonDefaultValue ()
    {
      _order.OrderNumber = _order.OrderNumber;
      Assert.IsNotNull (_businessOrder.GetProperty ("OrderNumber"));
    }

    [Test]
    public void GetPropertyDefaultForNonMappingProperties ()
    {
      IBusinessObject businessObject = (IBusinessObject) DomainObject.NewObject (typeof (BindableDomainObjectWithProperties));
      Assert.IsNotNull (businessObject.GetProperty ("RequiredPropertyNotInMapping"));
      Assert.AreEqual (true, businessObject.GetProperty ("RequiredPropertyNotInMapping"));
    }
  }
}