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
      ClassContext existingContext = new ClassContext (typeof (BaseType1));
      existingContext.AddMixin (typeof (BT1Mixin1));
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
  }
}