using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ConcreteMixinTypeAttributeTests
  {
    [ConcreteMixinType (3, typeof (ConcreteMixinTypeAttributeTests),
        new Type[] {typeof (string), typeof (object), typeof (int)},
        new Type[] {typeof (int)},
        new Type[] {typeof (object), typeof (double), typeof (bool), typeof (NextMixinDependency), typeof (string), typeof (bool)})]
    private class TestType
    {
    }

    [Test]
    public void FromAttributeApplication ()
    {
      ConcreteMixinTypeAttribute attribute = ((ConcreteMixinTypeAttribute[]) typeof (TestType).GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false))[0];

      Assert.AreEqual (3, attribute.MixinIndex);

      Assert.AreEqual (typeof (ConcreteMixinTypeAttributeTests), attribute.TargetType);
      
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
      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (7, simpleContext);

      Assert.AreEqual (7, attribute.MixinIndex);
      Assert.AreEqual (typeof (object), attribute.TargetType);
      Assert.AreEqual (1, attribute.MixinTypes.Length);
      Assert.AreEqual (typeof (string), attribute.MixinTypes[0]);
      Assert.AreEqual (0, attribute.CompleteInterfaces.Length);
      Assert.AreEqual (0, attribute.ExplicitDependenciesPerMixin.Length);
    }

    [Test]
    public void FromClassContextComplex ()
    {
      ClassContext context = new ClassContext (typeof (int));
      context.AddCompleteInterface (typeof (uint));
      context.AddMixinContext (new MixinContext (typeof (string), new Type[] {typeof (bool)}));
      context.AddMixinContext (new MixinContext (typeof (double), new Type[] {typeof (int)}));

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (5, context);

      Assert.AreEqual (5, attribute.MixinIndex);

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
    public void GetMixinDefinition ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (BaseType3));
      MixinDefinition referenceDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context).Mixins[0];

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (0, context);
      MixinDefinition definition = attribute.GetMixinDefinition ();
      Assert.AreSame (referenceDefinition, definition);
    }
  }
}