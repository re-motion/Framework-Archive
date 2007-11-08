using System;
using NUnit.Framework;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class DefaultValueTest
  {
    public class DefaultValueTrueMixin : Mixin<BindableObjectMixin>
    {
      [OverrideTarget]
      public bool IsDefaultValue (PropertyBase property, object nativeValue)
      {
        return true;
      }
    }

    [Test]
    public void GetProperty_NormallyReturnsNonNull ()
    {
      ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> ().With ();
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.IsNotNull (instanceAsIBusinessObject.GetProperty ("Scalar"));
      Assert.AreEqual (instance.Scalar, instanceAsIBusinessObject.GetProperty ("Scalar"));
    }

    [Test]
    public void GetProperty_ReturnsNull_WhenDefaultValueTrue ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BindableObjectMixin), typeof (DefaultValueTrueMixin)))
      {
        ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>>().With();
        IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

        Assert.IsNull (instanceAsIBusinessObject.GetProperty ("Scalar"));
      }
    }

    [Test]
    public void GetProperty_ReturnsNonNull_WhenDefaultValueTrueOnList ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BindableObjectMixin), typeof (DefaultValueTrueMixin)))
      {
        ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> ().With ();
        IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

        Assert.IsNotNull (instanceAsIBusinessObject.GetProperty ("List"));
        Assert.AreEqual (instance.List, instanceAsIBusinessObject.GetProperty ("List"));
      }
    }
  }
}