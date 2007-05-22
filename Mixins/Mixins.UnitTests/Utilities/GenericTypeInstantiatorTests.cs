using System;
using System.Collections.Generic;
using System.Text;
using Mixins.UnitTests.SampleTypes;
using Mixins.Utilities;
using NUnit.Framework;

namespace Mixins.UnitTests.Utilities
{
  [TestFixture]
  public class GenericTypeInstantiatorTests
  {
    [Test]
    public void NonGenericTypeIsntTouched()
    {
      Assert.AreSame (typeof (GenericTypeInstantiatorTests), GenericTypeInstantiator.EnsureClosedType (typeof (GenericTypeInstantiatorTests)));
    }

    class NoConstraints<T> { }

    [Test]
    public void GenericArgsWithoutConstraints ()
    {
      Assert.AreEqual (typeof (NoConstraints<object>), GenericTypeInstantiator.EnsureClosedType (typeof(NoConstraints<>)));
    }

    class Outer<T>
    {
      public class Inner : Outer<T> { }
    }

    [Test]
    public void NestedWorks ()
    {
      Assert.AreEqual (typeof (Outer<object>.Inner), GenericTypeInstantiator.EnsureClosedType (typeof (Outer<>.Inner)));
    }

    class ClassContraint<T> where T : class {}

    [Test]
    public void ClassConstraint()
    {
      Assert.AreEqual (typeof (ClassContraint<object>), GenericTypeInstantiator.EnsureClosedType (typeof (ClassContraint<>)));
    }

    class ValueTypeContraint<T> where T : struct { }

    [Test]
    public void ValueTypeConstraint ()
    {
      Assert.AreEqual (typeof (ValueTypeContraint<int>), GenericTypeInstantiator.EnsureClosedType (typeof (ValueTypeContraint<>)));
    }

    class InterfaceConstraint<T> where T : IServiceProvider { }

    [Test]
    public void SingleInterfaceConstraint ()
    {
      Assert.AreEqual (typeof (InterfaceConstraint<IServiceProvider>), GenericTypeInstantiator.EnsureClosedType (typeof (InterfaceConstraint<>)));
    }

    class BaseClassConstraint<T> where T : GenericTypeInstantiatorTests { }

    [Test]
    public void SingleBaseClassConstraint ()
    {
      Assert.AreEqual (typeof (BaseClassConstraint<GenericTypeInstantiatorTests>),
          GenericTypeInstantiator.EnsureClosedType (typeof (BaseClassConstraint<>)));
    }

    class CompatibleConstraints1<T> where T : BaseType3, IBaseType31, IBaseType32 { }

    [Test]
    public void MultipleCompatibleConstraints1 ()
    {
      Assert.AreEqual (typeof (CompatibleConstraints1<BaseType3>),
          GenericTypeInstantiator.EnsureClosedType (typeof (CompatibleConstraints1<>)));
    }

    class CompatibleConstraints2<T> where T : IBaseType33, IBaseType34 { }

    [Test]
    public void MultipleCompatibleConstraints2 ()
    {
      Assert.AreEqual (typeof (CompatibleConstraints2<IBaseType34>),
          GenericTypeInstantiator.EnsureClosedType (typeof (CompatibleConstraints2<>)));
    }

    public class IncompatibleConstraints1<T> where T : BaseType3, IBaseType31, IBaseType2 { }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter T has incompatible constraints",
        MatchType = MessageMatch.Contains)]
    public void MultipleIncompatibleConstraintsThrows1 ()
    {
      GenericTypeInstantiator.EnsureClosedType (typeof (IncompatibleConstraints1<>));
    }

    class IncompatibleConstraints2<T> where T : struct, IBaseType2 { }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter T has incompatible constraints",
        MatchType = MessageMatch.Contains)]
    public void MultipleIncompatibleConstraintsThrows2 ()
    {
      GenericTypeInstantiator.EnsureClosedType (typeof (IncompatibleConstraints2<>));
    }

    class IncompatibleConstraints3<T> where T : IBaseType31, IBaseType32 { }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter T has incompatible constraints",
        MatchType = MessageMatch.Contains)]
    public void MultipleIncompatibleConstraintsThrows3 ()
    {
      GenericTypeInstantiator.EnsureClosedType (typeof (IncompatibleConstraints3<>));
    }

    class Uninstantiable<T>
        where T : GenericTypeInstantiatorTests.Uninstantiable<T>.IT
    {
      public interface IT {}
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "The generic type parameter T has a constraint IT which itself contains generic parameters",
        MatchType = MessageMatch.Contains)]
    public void UninstantiableClass()
    {
      GenericTypeInstantiator.EnsureClosedType (typeof (Uninstantiable<>));
    }
  }
}
