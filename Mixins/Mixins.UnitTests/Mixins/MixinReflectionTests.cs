using System;
using System.Reflection;
using Mixins.CodeGeneration;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class MixinReflectionTests : MixinTestBase
  {
    [Test]
    public void FindMixinInstanceInTarget ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin2)).With();
      BT3Mixin2 mixin = MixinReflectionHelper.GetMixinOf<BT3Mixin2> (bt3);
      Assert.IsNotNull (mixin);
    }

    [Test]
    public void NullIfMixinNotFound ()
    {
      BT3Mixin2 mixin = MixinReflectionHelper.GetMixinOf<BT3Mixin2> (new object());
      Assert.IsNull (mixin);
    }

    [Test]
    public void IMixinTarget ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      ApplicationDefinition applicationDefinition = DefinitionBuilder.CreateApplicationDefinition (context);

      using (new CurrentTypeFactoryScope (applicationDefinition))
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
        IMixinTarget mixinTarget = bt1 as IMixinTarget;
        Assert.IsNotNull (mixinTarget);

        BaseClassDefinition configuration = mixinTarget.Configuration;
        Assert.IsNotNull (configuration);

        Assert.AreSame (applicationDefinition.BaseClasses[typeof (BaseType1)], configuration);

        object[] mixins = mixinTarget.Mixins;
        Assert.IsNotNull (mixins);
        Assert.AreEqual (configuration.Mixins.Count, mixins.Length);
      }
    }
  }
}
