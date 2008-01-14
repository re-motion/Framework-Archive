using System;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins.Context.FluentBuilders;
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

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.MixinCount);
      Assert.That (EnumerableUtility.ToArray (inheritor.Mixins), Is.EqualTo (EnumerableUtility.ToArray (baseContext.Mixins)));
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      ClassContext baseContext = new ClassContext (typeof (string));
      baseContext.AddMixin (typeof (DerivedNullTarget));

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.IsTrue (inheritor.ContainsAssignableMixin (typeof (DerivedNullTarget)));
      Assert.IsTrue (inheritor.ContainsAssignableMixin (typeof (NullTarget)));
    }

    [Test]
    public void GetOrAddMixin ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DateTime>().WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (baseContext.GetOrAddMixinContext (typeof (DateTime)), inheritor.GetOrAddMixinContext (typeof (DateTime)));
    }

    [Test]
    public void ExistingMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DateTime>().WithDependency<int>().BuildClassContext();
      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DateTime>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext); // ignores inherited DateTime because DateTime already exists

      Assert.AreEqual (1, inheritor.MixinCount);
      Assert.IsTrue (inheritor.ContainsMixin (typeof (DateTime)));
      Assert.IsFalse (inheritor.GetOrAddMixinContext (typeof (DateTime)).ContainsExplicitDependency (typeof (int)));
      Assert.IsTrue (inheritor.GetOrAddMixinContext (typeof (DateTime)).ContainsExplicitDependency (typeof (decimal)));
    }

    [Test]
    public void DerivedMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<NullTarget>().WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DerivedNullTarget>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext); // ignores inherited NullTarget because DerivedNullTarget already exists

      Assert.AreEqual (1, inheritor.MixinCount);
      Assert.IsFalse (inheritor.ContainsMixin (typeof (NullTarget)));
      Assert.IsTrue (inheritor.ContainsMixin (typeof (DerivedNullTarget)));
      Assert.IsFalse (inheritor.GetOrAddMixinContext (typeof (DerivedNullTarget)).ContainsExplicitDependency (typeof (int)));
      Assert.IsTrue (inheritor.GetOrAddMixinContext (typeof (DerivedNullTarget)).ContainsExplicitDependency (typeof (decimal)));
    }

    [Test]
    public void SpecializedGenericMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<GenericMixinWithVirtualMethod<object>>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);

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
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DerivedGenericMixin<object>>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);

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
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DerivedNullTarget>().WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin<NullTarget>().WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void InheritedSpecializedDerivedGenericMixin_Throws ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DerivedGenericMixin<object>>().WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The class System.Double inherits the mixin "
       + ".*DerivedGenericMixin\\`1 from class System.String, but it is explicitly configured for the less "
        + "specific mixin .*GenericMixinWithVirtualMethod\\`1\\[T\\].", MatchType = MessageMatch.Regex)]
    public void InheritedUnspecializedDerivedGenericMixin_Throws ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (DerivedGenericMixin<>)).WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<decimal>().BuildClassContext()
          .InheritFrom (baseContext);
    }

    [Test]
    public void CompleteInterfaces ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string))
          .AddCompleteInterface (typeof (object))
          .AddCompleteInterface (typeof (int))
          .BuildClassContext();

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.CompleteInterfaceCount);
      Assert.That (EnumerableUtility.ToArray (inheritor.CompleteInterfaces),
          Is.EqualTo (EnumerableUtility.ToArray (inheritor.CompleteInterfaces)));
    }

    [Test]
    public void ContainsCompleteInterface ()
    {
      ClassContext baseContext = new ClassContext (typeof (string), new MixinContext[0], new Type[] {typeof (object)});

      ClassContext inheritor = new ClassContext (typeof (double)).InheritFrom (baseContext);

      Assert.IsTrue (inheritor.ContainsCompleteInterface (typeof (object)));
    }

    [Test]
    public void ExistingCompleteInterface_NotReplacedByInheritance ()
    {
      ClassContext baseContext = new ClassContext (typeof (string), new MixinContext[0], new Type[] { typeof (object) });

      ClassContext inheritor = new ClassContext (typeof (double), new MixinContext[0], new Type[] {typeof (object)})
          .InheritFrom (baseContext);

      Assert.AreEqual (1, inheritor.CompleteInterfaceCount);
      Assert.IsTrue (inheritor.ContainsCompleteInterface (typeof (object)));
    }

    [Test]
    public void InheritFrom_LeavesExistingData ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string))
          .AddMixin (typeof (DateTime))
          .AddCompleteInterface (typeof (object))
          .BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double))
          .AddMixin (typeof (string))
          .AddCompleteInterface (typeof (int)).BuildClassContext()
          .InheritFrom (baseContext);

      Assert.AreEqual (2, inheritor.MixinCount);
      Assert.AreEqual (2, inheritor.CompleteInterfaceCount);
    }
  }
}