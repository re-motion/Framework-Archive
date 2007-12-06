using System;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Utilities;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.ClassContextTests
{
  [TestFixture]
  public class ClassContextInheritanceTests
  {
    [Test]
    public void ContainsOverrideForMixin_False ()
    {
      ClassContext context = new ClassContext (typeof (string));
      context.AddMixin (typeof (NullTarget));
      context.AddMixin (typeof (GenericClassExtendedByMixin<>));

      Assert.IsFalse (context.ContainsOverrideForMixin (typeof (int))); // completely unrelated
      Assert.IsFalse (context.ContainsOverrideForMixin (typeof (DerivedNullTarget))); // subtype
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (GenericClassExtendedByMixin<object>))); // specialization doesn't matter
      Assert.IsFalse (context.ContainsOverrideForMixin (typeof (DerivedGenericMixin<>))); // subtype
    }

    [Test]
    public void ContainsOverrideForMixin_Same ()
    {
      ClassContext context = new ClassContext (typeof (string));
      context.AddMixin (typeof (NullTarget));
      context.AddMixin (typeof (GenericClassExtendedByMixin<>));

      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (NullTarget)));
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (GenericClassExtendedByMixin<>)));
    }

    [Test]
    public void ContainsOverrideForMixin_True ()
    {
      ClassContext context = new ClassContext (typeof (string));
      context.AddMixin (typeof (DerivedNullTarget));
      context.AddMixin (typeof (GenericMixinWithVirtualMethod<object>));
      
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (NullTarget))); // supertype
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<>))); // less specialized
    }

    [Test]
    public void ContainsOverrideForMixin_DerivedAndSpecialized ()
    {
      ClassContext context = new ClassContext (typeof (string));
      context.AddMixin (typeof (DerivedNullTarget));
      context.AddMixin (typeof (DerivedGenericMixin<object>));

      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<object>)));
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (DerivedGenericMixin<>)));
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (DerivedGenericMixin<object>)));
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<string>))); // different type arguments don't matter
      Assert.IsTrue (context.ContainsOverrideForMixin (typeof (DerivedGenericMixin<string>))); // different specialization doesn't matter
    }

    [Test]
    public void Mixins ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DateTime));
      baseContext.AddMixin (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.MixinCount);
      Assert.That (EnumerableUtility.ToArray (inheritor.Mixins), Is.EqualTo (EnumerableUtility.ToArray (baseContext.Mixins)));
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DerivedNullTarget));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.InheritFrom (baseContext);

      Assert.IsTrue (inheritor.ContainsAssignableMixin (typeof (DerivedNullTarget)));
      Assert.IsTrue (inheritor.ContainsAssignableMixin (typeof (NullTarget)));
    }

    [Test]
    public void GetOrAddMixin ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DateTime)).AddExplicitDependency (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.InheritFrom (baseContext);

      Assert.AreEqual (baseContext.GetOrAddMixinContext (typeof (DateTime)), inheritor.GetOrAddMixinContext (typeof (DateTime)));
    }

    [Test]
    public void ExistingMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DateTime)).AddExplicitDependency (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddMixin (typeof (DateTime)).AddExplicitDependency (typeof (decimal));
      
      inheritor.InheritFrom (baseContext); // ignores inherited DateTime because DateTime already exists

      Assert.AreEqual (1, inheritor.MixinCount);
      Assert.IsTrue (inheritor.ContainsMixin (typeof (DateTime)));
      Assert.IsFalse (inheritor.GetOrAddMixinContext (typeof (DateTime)).ContainsExplicitDependency (typeof (int)));
      Assert.IsTrue (inheritor.GetOrAddMixinContext (typeof (DateTime)).ContainsExplicitDependency (typeof (decimal)));
    }

    [Test]
    public void DerivedMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (NullTarget)).AddExplicitDependency (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddMixin (typeof (DerivedNullTarget)).AddExplicitDependency (typeof (decimal));

      inheritor.InheritFrom (baseContext); // ignores inherited NullTarget because DerivedNullTarget already exists

      Assert.AreEqual (1, inheritor.MixinCount);
      Assert.IsFalse (inheritor.ContainsMixin (typeof (NullTarget)));
      Assert.IsTrue (inheritor.ContainsMixin (typeof (DerivedNullTarget)));
      Assert.IsFalse (inheritor.GetOrAddMixinContext (typeof (DerivedNullTarget)).ContainsExplicitDependency (typeof (int)));
      Assert.IsTrue (inheritor.GetOrAddMixinContext (typeof (DerivedNullTarget)).ContainsExplicitDependency (typeof (decimal)));
    }

    [Test]
    public void SpecializedGenericMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (GenericMixinWithVirtualMethod<>)).AddExplicitDependency (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddMixin (typeof (GenericMixinWithVirtualMethod<object>)).AddExplicitDependency (typeof (decimal));

      inheritor.InheritFrom (baseContext);

      Assert.AreEqual (1, inheritor.MixinCount);
      Assert.IsFalse (inheritor.ContainsMixin (typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsTrue (inheritor.ContainsMixin (typeof (GenericMixinWithVirtualMethod<object>)));
      Assert.IsFalse (inheritor.GetOrAddMixinContext (typeof (GenericMixinWithVirtualMethod<object>))
          .ContainsExplicitDependency (typeof (int)));
      Assert.IsTrue (inheritor.GetOrAddMixinContext (typeof (GenericMixinWithVirtualMethod<object>))
          .ContainsExplicitDependency (typeof (decimal)));
    }

    class DerivedGenericMixin<T> : GenericMixinWithVirtualMethod<T> where T : class { }

    [Test]
    public void SpecializedDerivedGenericMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (GenericMixinWithVirtualMethod<>)).AddExplicitDependency (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddMixin (typeof (DerivedGenericMixin<object>)).AddExplicitDependency (typeof (decimal));

      inheritor.InheritFrom (baseContext);

      Assert.AreEqual (1, inheritor.MixinCount);
      Assert.IsFalse (inheritor.ContainsMixin (typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsFalse (inheritor.ContainsMixin (typeof (GenericMixinWithVirtualMethod<object>)));
      Assert.IsFalse (inheritor.ContainsMixin (typeof (DerivedGenericMixin<>)));
      Assert.IsTrue (inheritor.ContainsMixin (typeof (DerivedGenericMixin<object>)));

      Assert.IsFalse (inheritor.GetOrAddMixinContext (typeof (DerivedGenericMixin<object>))
          .ContainsExplicitDependency (typeof (int)));
      Assert.IsTrue (inheritor.GetOrAddMixinContext (typeof (DerivedGenericMixin<object>))
          .ContainsExplicitDependency (typeof (decimal)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void InheritedDerivedMixin_Throws ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DerivedNullTarget)).AddExplicitDependency (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddMixin (typeof (NullTarget)).AddExplicitDependency (typeof (decimal));

      inheritor.InheritFrom (baseContext);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void InheritedSpecializedDerivedGenericMixin_Throws ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DerivedGenericMixin<object>)).AddExplicitDependency (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddMixin (typeof (GenericMixinWithVirtualMethod<>)).AddExplicitDependency (typeof (decimal));

      inheritor.InheritFrom (baseContext);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The class System.Double inherits the mixin "
       + ".*DerivedGenericMixin\\`1 from class System.String, but it is explicitly configured for the less "
        + "specific mixin .*GenericMixinWithVirtualMethod\\`1\\[T\\].", MatchType = MessageMatch.Regex)]
    public void InheritedUnspecializedDerivedGenericMixin_Throws ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DerivedGenericMixin<>)).AddExplicitDependency (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddMixin (typeof (GenericMixinWithVirtualMethod<>)).AddExplicitDependency (typeof (decimal));

      inheritor.InheritFrom (baseContext);
    }

    [Test]
    public void RemoveInheritedMixin ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DateTime));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.InheritFrom (baseContext);
      inheritor.RemoveMixin (typeof (DateTime));
      Assert.AreEqual (0, inheritor.MixinCount);
      Assert.IsFalse (inheritor.ContainsMixin (typeof (DateTime)));
    }

    [Test]
    public void CompleteInterfaces ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddCompleteInterface (typeof (object));
      baseContext.AddCompleteInterface (typeof (int));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.InheritFrom (baseContext);

      Assert.AreEqual (2, baseContext.CompleteInterfaceCount);
      Assert.That (EnumerableUtility.ToArray (baseContext.CompleteInterfaces),
          Is.EqualTo (EnumerableUtility.ToArray (baseContext.CompleteInterfaces)));
    }

    [Test]
    public void ContainsCompleteInterface ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddCompleteInterface (typeof (object));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.InheritFrom (baseContext);

      Assert.IsTrue (inheritor.ContainsCompleteInterface (typeof (object)));
    }

    [Test]
    public void ExistingCompleteInterface_NotReplacedByInheritance ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddCompleteInterface (typeof (object));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddCompleteInterface (typeof (object));
      inheritor.InheritFrom (baseContext);

      Assert.AreEqual (1, inheritor.CompleteInterfaceCount);
      Assert.IsTrue (inheritor.ContainsCompleteInterface (typeof (object)));
    }

    [Test]
    public void RemoveInheritedCompleteInterface ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddCompleteInterface (typeof (object));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.InheritFrom (baseContext);
      inheritor.RemoveCompleteInterface (typeof (object));

      Assert.AreEqual (0, inheritor.CompleteInterfaceCount);
      Assert.IsFalse (inheritor.ContainsCompleteInterface (typeof (object)));
    }

    [Test]
    public void InheritFrom_LeavesExistingData ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DateTime));
      baseContext.AddCompleteInterface (typeof (object));

      ClassContext inheritor = new ClassContext (typeof (double));
      inheritor.AddMixin (typeof (string));
      inheritor.AddCompleteInterface (typeof (int));
      inheritor.InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.MixinCount);
      Assert.AreEqual (2, inheritor.CompleteInterfaceCount);
    }
  }
}