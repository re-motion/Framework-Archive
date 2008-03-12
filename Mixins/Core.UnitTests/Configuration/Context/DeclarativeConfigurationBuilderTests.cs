using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Configuration.Context
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderTests
  {
    private DeclarativeConfigurationBuilder _builder;
    private ClassContext _globalClassContext;

    [SetUp]
    public void SetUp ()
    {
      _builder = new DeclarativeConfigurationBuilder (null);
      _globalClassContext = new ClassContextBuilder (typeof (TargetClassForGlobalMix))
          .AddMixin (typeof (MixinForGlobalMix)).WithDependency (typeof (AdditionalDependencyForGlobalMix))
          .AddMixin (typeof (AdditionalDependencyForGlobalMix)).BuildClassContext ();
    }

    [Test]
    public void AddType ()
    {
      _builder.AddType (typeof (object));
      _builder.AddType (typeof (string));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new Type[] { typeof (object), typeof (string) }));
    }

    [Test]
    public void AddType_Twice ()
    {
      _builder.AddType (typeof (object));
      _builder.AddType (typeof (object));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new Type[] { typeof (object) }));
    }

    [Test]
    public void AddType_WithDerivedType ()
    {
      _builder.AddType (typeof (string));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new Type[] { typeof (object), typeof (string) }));
    }

    [Test]
    public void AddType_WithOpenGenericType ()
    {
      _builder.AddType (typeof (List<>));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new Type[] { typeof (List<>), typeof (object) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type must be non-generic or a generic type definition.\r\nParameter name: type")]
    public void AddType_WithClosedGenericType ()
    {
      _builder.AddType (typeof (List<int>));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new Type[] { typeof (List<>) }));
    }

    class DerivedList : List<int> { }

    [Test]
    public void AddType_WithDerivedFromGenericType ()
    {
      _builder.AddType (typeof (DerivedList));

      Assert.That (_builder.AllTypes, Is.EquivalentTo (new Type[] { typeof (DerivedList), typeof (List<>), typeof (object) }));
    }

    [Test]
    public void AddAssembly ()
    {
      _builder.AddAssembly (typeof (DeclarativeConfigurationBuilderTests).Assembly);

      DeclarativeConfigurationBuilder referenceBuilder = new DeclarativeConfigurationBuilder (null);
      foreach (Type t in typeof (DeclarativeConfigurationBuilderTests).Assembly.GetTypes ())
      {
        if (!t.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false))
          referenceBuilder.AddType (t);
      }

      Assert.That (_builder.AllTypes, Is.EquivalentTo ((ICollection) referenceBuilder.AllTypes));
    }

    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    class User { }

    [Extends (typeof (NullTarget))]
    [IgnoreForMixinConfiguration]
    class Extender { }

    [CompleteInterface (typeof (NullTarget))]
    [IgnoreForMixinConfiguration]
    interface ICompleteInterface { }


    [Test]
    public void BuildConfiguration ()
    {
      _builder.AddType (typeof (User));
      _builder.AddType (typeof (Extender));
      _builder.AddType (typeof (ICompleteInterface));

      MixinConfiguration configuration = _builder.BuildConfiguration();
      ClassContext c1 = new ClassContext (typeof (User), typeof (NullMixin));
      ClassContext c2 = new ClassContextBuilder(typeof (NullTarget))
          .AddMixin (typeof (Extender)).AddCompleteInterface (typeof (ICompleteInterface)).BuildClassContext ();
      Assert.That (configuration.ClassContexts, Is.EquivalentTo (new object[] { c1, c2, _globalClassContext }));
    }

    [Test]
    public void BuildConfiguration_WithParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = MixinConfiguration.BuildNew().ForClass<int>().AddMixin<string>().BuildConfiguration();
      DeclarativeConfigurationBuilder builder = new DeclarativeConfigurationBuilder (parentConfiguration);
      builder.AddType (typeof (User));

      MixinConfiguration configuration = builder.BuildConfiguration ();
      ClassContext c1 = new ClassContext (typeof (User), typeof (NullMixin));
      Assert.That (configuration.ClassContexts,
          Is.EquivalentTo (new object[] { c1, parentConfiguration.ClassContexts.GetExact (typeof (int)), _globalClassContext }));
    }
  }
}