using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ConcreteMixedTypeAttributeTests
  {
    [ConcreteMixedType (typeof (ConcreteMixedTypeAttributeTests),
        new Type[] {typeof (string), typeof (object), typeof (int)},
        new Type[] {typeof (int)},
        new Type[] {typeof (object), typeof (double), typeof (bool), typeof (NextMixinDependency), typeof (string), typeof (bool)})]
    private class TestType
    {
    }

    [Test]
    public void FromAttributeApplication ()
    {
      ConcreteMixedTypeAttribute attribute = ((ConcreteMixedTypeAttribute[]) typeof (TestType).GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false))[0];

      Assert.AreEqual (typeof (ConcreteMixedTypeAttributeTests), attribute.TargetType);
      
      Assert.AreEqual (3, attribute.MixinTypes.Length);
      Assert.AreEqual (typeof (string), attribute.MixinTypes[0]);
      Assert.AreEqual (typeof (object), attribute.MixinTypes[1]);
      Assert.AreEqual (typeof (int), attribute.MixinTypes[2]);

      Assert.AreEqual (1, attribute.CompleteInterfaces.Length);
      Assert.AreEqual (typeof (int), attribute.CompleteInterfaces[0]);

      Assert.AreEqual (6, attribute.ExplicitDependenciesPerMixin.Length);
      Assert.AreEqual (typeof (object), attribute.ExplicitDependenciesPerMixin[0]);
      Assert.AreEqual (typeof (double), attribute.ExplicitDependenciesPerMixin[1]);
      Assert.AreEqual (typeof (bool), attribute.ExplicitDependenciesPerMixin[2]);
      Assert.AreEqual (typeof (NextMixinDependency), attribute.ExplicitDependenciesPerMixin[3]);
      Assert.AreEqual (typeof (string), attribute.ExplicitDependenciesPerMixin[4]);
      Assert.AreEqual (typeof (bool), attribute.ExplicitDependenciesPerMixin[5]);
    }

    [Test]
    public void FromClassContextSimple ()
    {
      ClassContext simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (simpleContext);

      Assert.AreEqual (typeof (object), attribute.TargetType);
      Assert.AreEqual (1, attribute.MixinTypes.Length);
      Assert.AreEqual (typeof (string), attribute.MixinTypes[0]);
      Assert.AreEqual (0, attribute.CompleteInterfaces.Length);
      Assert.AreEqual (0, attribute.ExplicitDependenciesPerMixin.Length);
    }

    [Test]
    public void FromClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .AddMixin (typeof (double)).WithDependency (typeof (int))
          .BuildClassContext();

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);

      Assert.AreEqual (typeof (int), attribute.TargetType);
      Assert.AreEqual (2, attribute.MixinTypes.Length);
      Assert.AreEqual (typeof (string), attribute.MixinTypes[0]);
      Assert.AreEqual (typeof (double), attribute.MixinTypes[1]);
      
      Assert.AreEqual (1, attribute.CompleteInterfaces.Length);
      Assert.AreEqual (typeof (uint), attribute.CompleteInterfaces[0]);

      Assert.AreEqual (5, attribute.ExplicitDependenciesPerMixin.Length);
      Assert.AreEqual (typeof (string), attribute.ExplicitDependenciesPerMixin[0]);
      Assert.AreEqual (typeof (bool), attribute.ExplicitDependenciesPerMixin[1]);
      Assert.AreEqual (typeof (NextMixinDependency), attribute.ExplicitDependenciesPerMixin[2]);
      Assert.AreEqual (typeof (double), attribute.ExplicitDependenciesPerMixin[3]);
      Assert.AreEqual (typeof (int), attribute.ExplicitDependenciesPerMixin[4]);
    }

    [Test]
    public void GetClassContextSimple ()
    {
      ClassContext simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (simpleContext);
      ClassContext regeneratedContext = attribute.GetClassContext ();

      Assert.AreEqual (regeneratedContext, simpleContext);
      Assert.AreNotSame (regeneratedContext, simpleContext);
    }

    [Test]
    public void GetClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddMixin (typeof (double))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .BuildClassContext();

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();

      Assert.AreEqual (regeneratedContext, context);
      Assert.AreNotSame (regeneratedContext, context);
    }

    [Test]
    public void DependencyParsing ()
    {
      ConcreteMixedTypeAttribute attribute = ((ConcreteMixedTypeAttribute[]) typeof (TestType).GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false))[0];

      ClassContext context = attribute.GetClassContext ();

      Assert.AreEqual (3, context.MixinCount);

      Assert.IsTrue (context.ContainsMixin (typeof (object)));
      Assert.AreEqual (2, context.GetMixinContext (typeof (object)).ExplicitDependencyCount);
      Assert.IsTrue (context.GetMixinContext (typeof (object)).ContainsExplicitDependency (typeof (double)));
      Assert.IsTrue (context.GetMixinContext (typeof (object)).ContainsExplicitDependency (typeof (bool)));

      Assert.IsTrue (context.ContainsMixin (typeof (string)));
      Assert.AreEqual (1, context.GetMixinContext (typeof (string)).ExplicitDependencyCount);
      Assert.IsTrue (context.GetMixinContext (typeof (string)).ContainsExplicitDependency (typeof (bool)));

      Assert.IsTrue (context.ContainsMixin (typeof (int)));
      Assert.AreEqual (0, context.GetMixinContext (typeof (int)).ExplicitDependencyCount);
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (BaseType3));
      TargetClassDefinition referenceDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      TargetClassDefinition definition = attribute.GetTargetClassDefinition ();
      Assert.AreSame (referenceDefinition, definition);
    }

    [Test]
    public void AttributeWithGenericType ()
    {
      ClassContext context = new ClassContext (typeof (List<>)).SpecializeWithTypeArguments (new Type[] {typeof (int)});
      Assert.AreEqual (typeof (List<int>), context.Type);
      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      Assert.AreEqual (typeof (List<int>), attribute.TargetType);
      ClassContext context2 = attribute.GetClassContext ();
      Assert.AreEqual (typeof (List<int>), context2.Type);

      TargetClassDefinition definition = attribute.GetTargetClassDefinition ();
      Assert.AreEqual (typeof (List<int>), definition.Type);
    }
  }
}