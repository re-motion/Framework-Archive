using System;
using Rubicon.Mixins;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using NUnit.Framework;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class TypeFactoryTests
  {
    [Test]
    public void GetActiveConfiguration()
    {
      using (MixinConfiguration.ScopedEmpty())
      {
        Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
        Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));
        Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
        Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
        Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
        Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));

        using (MixinConfiguration.ScopedExtend(typeof (BaseType1)))
        {
          Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
          Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));
          Assert.AreSame (
              BaseClassDefinitionCache.Current.GetBaseClassDefinition (new ClassContext (typeof (BaseType1))),
              TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
          Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
          Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
          Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));

          using (MixinConfiguration.ScopedExtend(typeof (BaseType2)))
          {
            Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
            Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));

            Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
            Assert.AreSame (
                BaseClassDefinitionCache.Current.GetBaseClassDefinition (new ClassContext (typeof (BaseType1))),
                TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
            Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
            Assert.AreSame (
                BaseClassDefinitionCache.Current.GetBaseClassDefinition (new ClassContext (typeof (BaseType2))),
                TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
          }
        }
      }
    }

    [Test]
    public void DefinitionGeneratedEvenIfNoConfig()
    {
      Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (object)));
      Type t = TypeFactory.GetConcreteType (typeof (object));
      Assert.IsTrue (typeof (IMixinTarget).IsAssignableFrom (t));

      // Check caching for definitions generated for types without mixin configuration
      BaseClassDefinition d1 = TypeFactory.GetActiveConfiguration (typeof (object));
      BaseClassDefinition d2 = TypeFactory.GetActiveConfiguration (typeof (object));
      Assert.AreSame (d1, d2);
    }
  }
}
