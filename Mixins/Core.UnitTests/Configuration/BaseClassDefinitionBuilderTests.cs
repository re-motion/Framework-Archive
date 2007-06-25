using System;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class BaseClassDefinitionBuilderTests
  {
    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "contains generic parameters", MatchType = MessageMatch.Contains)]
    public void ThrowsOnGenericBaseClass()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BT3Mixin3<,>)))
      {
        BaseClassDefinitionBuilder builder = new BaseClassDefinitionBuilder();
        builder.Build (new ClassContext (typeof (BT3Mixin3<,>)));
      }
    }

    [Test]
    public void BaseClassKnowsItsContext()
    {
      ClassContext classContext = new ClassContext (typeof (BaseType1));
      using (MixinConfiguration.ScopedExtend(classContext))
      {
        BaseClassDefinition def = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsNotNull (def.ConfigurationContext);
        Assert.AreSame (classContext, def.ConfigurationContext);
      }
    }
  }
}
