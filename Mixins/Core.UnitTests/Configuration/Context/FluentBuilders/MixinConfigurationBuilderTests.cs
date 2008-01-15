using System;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.FluentBuilders
{
  [TestFixture]
  public class MixinConfigurationBuilderTests
  {
    [Test]
    public void Intitialization_WithoutParent ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      Assert.IsNull (builder.ParentConfiguration);
      Assert.That (builder.ClassContextBuilders, Is.Empty);
      MixinConfiguration configuration = builder.BuildConfiguration();
      Assert.AreEqual (0, configuration.ClassContextCount);
    }

    [Test]
    public void Intitialization_WithParent ()
    {
      MixinConfiguration parent = new MixinConfiguration (null);
      parent.AddClassContext (new ClassContext (typeof (string)));

      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (parent);
      Assert.AreSame (parent, builder.ParentConfiguration);
      Assert.That (builder.ClassContextBuilders, Is.Empty);

      MixinConfiguration configuration = builder.BuildConfiguration ();
      Assert.AreEqual (1, configuration.ClassContextCount);
      Assert.IsTrue (configuration.ContainsClassContext (typeof (string)));
    }

    [Test]
    public void ForClass_NonGeneric ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass (typeof (BaseType1));
      Assert.AreSame (typeof (BaseType1), classBuilder.TargetType);
      Assert.AreSame (builder, classBuilder.Parent);
      Assert.That (builder.ClassContextBuilders, List.Contains (classBuilder));
    }

    [Test]
    public void ForClass_Twice()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass (typeof (BaseType1));
      ClassContextBuilder classBuilder2 = builder.ForClass (typeof (BaseType1));
      Assert.AreSame (classBuilder, classBuilder2);
    }

    [Test]
    public void ForClass_Generic ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.AreSame (typeof (BaseType1), classBuilder.TargetType);
      Assert.AreSame (builder, classBuilder.Parent);
      Assert.That (builder.ClassContextBuilders, List.Contains (classBuilder));
    }

    [Test]
    public void ForClass_WithExistingContext ()
    {
      ClassContext existingContext = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1));
      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      parentConfiguration.AddClassContext (existingContext);

      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (parentConfiguration);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.That (classBuilder.MixinContextBuilders, Is.Not.Empty);
    }

    [Test]
    public void ForClass_WithoutExistingContext_NullParentConfiguration ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.That (classBuilder.MixinContextBuilders, Is.Empty);
    }

    [Test]
    public void ForClass_WithoutExistingContext_WithParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (parentConfiguration);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.That (classBuilder.MixinContextBuilders, Is.Empty);
    }

    [Test]
    public void BuildConfiguration ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      builder.ForClass<BaseType1> ();
      MixinConfiguration configurtation = builder.BuildConfiguration();
      Assert.AreEqual (1, configurtation.ClassContextCount);
      Assert.IsTrue (configurtation.ContainsClassContext (typeof (BaseType1)));
    }

    [Test]
    public void EnterScope ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType4)));
      using (new MixinConfigurationBuilder (null).ForClass<BaseType4> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType4)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void ClassContextInheritance_Base_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration =
          new MixinConfigurationBuilder (null).ForClass<NullTarget>().AddMixin (typeof (NullMixin)).BuildConfiguration();
      MixinConfiguration configuration =
          new MixinConfigurationBuilder (parentConfiguration).ForClass<DerivedNullTarget>().AddMixin (typeof (NullMixin2)).BuildConfiguration();
      ClassContext derivedContext = configuration.GetClassContext (typeof (DerivedNullTarget));
      Assert.AreEqual (2, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void ClassContextInheritance_TypeDefinition_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration =
          new MixinConfigurationBuilder (null).ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin)).BuildConfiguration ();
      MixinConfiguration configuration =
          new MixinConfigurationBuilder (parentConfiguration).ForClass<GenericTargetClass<int>> ().AddMixin (typeof (NullMixin2)).BuildConfiguration ();
      ClassContext derivedContext = configuration.GetClassContext (typeof (GenericTargetClass<int>));
      Assert.AreEqual (2, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void ClassContextInheritance_BaseAndTypeDefinition_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (DerivedGenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin3))
          .BuildConfiguration ();
      MixinConfiguration configuration = new MixinConfigurationBuilder (parentConfiguration)
          .ForClass<DerivedGenericTargetClass<int>> ().AddMixin (typeof (NullMixin4))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.GetClassContext (typeof (DerivedGenericTargetClass<int>));
      Assert.AreEqual (4, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin3)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin4)));
    }

    [Test]
    public void ClassContextInheritance_WithOverrides_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (NullTarget)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      MixinConfiguration configuration = new MixinConfigurationBuilder (parentConfiguration)
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (DerivedNullMixin))
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (DerivedNullMixin))
          .BuildConfiguration ();
      
      ClassContext derivedContext1 = configuration.GetClassContext (typeof (DerivedNullTarget));
      Assert.AreEqual (1, derivedContext1.Mixins.Count);
      Assert.IsTrue (derivedContext1.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (derivedContext1.Mixins.ContainsKey (typeof (NullMixin)));

      ClassContext derivedContext2 = configuration.GetClassContext (typeof (GenericTargetClass<int>));
      Assert.AreEqual (1, derivedContext2.Mixins.Count);
      Assert.IsTrue (derivedContext2.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (derivedContext2.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void ClassContextInheritance_Base_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (NullMixin2))
          .ForClass<NullTarget> ().AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.GetClassContext (typeof (DerivedNullTarget));
      Assert.AreEqual (2, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void ClassContextInheritance_TypeDefinition_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.GetClassContext (typeof (GenericTargetClass<int>));
      Assert.AreEqual (2, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void ClassContextInheritance_BaseAndTypeDefinition_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<DerivedGenericTargetClass<int>> ().AddMixin (typeof (NullMixin4))
          .ForClass (typeof (DerivedGenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin3))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.GetClassContext (typeof (DerivedGenericTargetClass<int>));
      Assert.AreEqual (4, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin3)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin4)));
    }

    [Test]
    public void ClassContextInheritance_WithOverrides_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (NullTarget)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (DerivedNullMixin))
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (DerivedNullMixin))
          .BuildConfiguration ();

      ClassContext derivedContext1 = configuration.GetClassContext (typeof (DerivedNullTarget));
      Assert.AreEqual (1, derivedContext1.Mixins.Count);
      Assert.IsTrue (derivedContext1.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (derivedContext1.Mixins.ContainsKey (typeof (NullMixin)));

      ClassContext derivedContext2 = configuration.GetClassContext (typeof (GenericTargetClass<int>));
      Assert.AreEqual (1, derivedContext2.Mixins.Count);
      Assert.IsTrue (derivedContext2.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (derivedContext2.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void CompleteInterfaceRegistration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType6))
          .AddMixin (typeof (BT6Mixin1))
          .AddCompleteInterface (typeof (ICBT6Mixin1)).BuildConfiguration();
      ClassContext resolvedContext = configuration.ResolveInterface (typeof (ICBT6Mixin1));
      Assert.IsNotNull (resolvedContext);
      Assert.AreEqual (typeof (BaseType6), resolvedContext.Type);
      Assert.AreEqual (1, resolvedContext.Mixins.Count);
      Assert.IsTrue (resolvedContext.Mixins.ContainsKey (typeof (BT6Mixin1)));
    }
  }
}