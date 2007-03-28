using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Mixins.Context;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests
{
  [TestFixture]
  public class DefaultContextBuilderTests
  {
    [Test]
    public void NewContextHasNoDefinitions ()
    {
      ApplicationContext context = new ApplicationContext ();
      Assert.IsFalse (context.HasClassContext (typeof (BaseType1)));
      List<ClassContext> classContexts = new List<ClassContext> (context.ClassContexts);
      Assert.AreEqual (0, classContexts.Count);
    }

    [Test]
    public void BuildFromTestAssembly ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      Assert.IsTrue (context.HasClassContext (typeof (BaseType1)));
      
      List<ClassContext> classContexts = new List<ClassContext> (context.ClassContexts);
      Assert.IsTrue (classContexts.Count > 0);

      ClassContext contextForBaseType1 = context.GetClassContext (typeof (BaseType1));
      List<MixinDefinition> mixinDefinitions = new List<MixinDefinition> (contextForBaseType1.MixinDefinitions);
      Assert.AreEqual (2, mixinDefinitions.Count);

      MixinDefinition def1 = mixinDefinitions.Find (delegate (MixinDefinition mixinDefinition) { return mixinDefinition.MixinType == typeof (Mixin1ForBT1); });
      Assert.IsNotNull (def1);

      Assert.AreEqual (typeof (Mixin1ForBT1), def1.MixinType);
      Assert.AreEqual (typeof (BaseType1), def1.TargetType);
      Assert.AreEqual (typeof (BaseType1), def1.DefiningAttribute.TargetType);

      MixinDefinition def2 = mixinDefinitions.Find (delegate (MixinDefinition mixinDefinition) { return mixinDefinition.MixinType == typeof (Mixin2ForBT1); });
      Assert.IsNotNull (def2);

      Assert.AreEqual (typeof (Mixin2ForBT1), def2.MixinType);
      Assert.AreEqual (typeof (BaseType1), def2.TargetType);
      Assert.AreEqual (typeof (BaseType1), def2.DefiningAttribute.TargetType);
    }

    [Test][ExpectedException (typeof (InvalidOperationException))]
    public void ThrowsOnDuplicateContexts ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      context.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    public void MultipleMixinTypeOccurrencesOnSameTargetType ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      DefaultContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly (), context);

      ClassContext classContext = context.GetClassContext (typeof (BaseType1));
      List<MixinDefinition> mixinDefinitions = new List<MixinDefinition> (classContext.MixinDefinitions);
      Assert.AreEqual (4, mixinDefinitions.Count);

      List<MixinDefinition> defs1 = mixinDefinitions.FindAll (delegate (MixinDefinition mixinDefinition) { return mixinDefinition.MixinType == typeof (Mixin1ForBT1); });
      Assert.AreEqual (2, defs1.Count);

      List<MixinDefinition> defs2 = mixinDefinitions.FindAll (delegate (MixinDefinition mixinDefinition) { return mixinDefinition.MixinType == typeof (Mixin2ForBT1); });
      Assert.AreEqual (2, defs2.Count);
     
    }
  }
}
