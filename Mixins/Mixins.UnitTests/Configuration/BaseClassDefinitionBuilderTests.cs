using System;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class BaseClassDefinitionBuilderTests
  {
    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "contains generic parameters", MatchType = MessageMatch.Contains)]
    public void ThrowsOnGenericBaseClass()
    {
      using (new MixinConfiguration (typeof (BT3Mixin3<,>)))
      {
        BaseClassDefinitionBuilder builder = new BaseClassDefinitionBuilder();
        builder.Build (new ClassContext (typeof (BT3Mixin3<,>)));
      }
    }

    [Test]
    public void BaseClassKnowsItsContext()
    {
      ClassContext classContext = new ClassContext (typeof (BaseType1));
      using (new MixinConfiguration (classContext))
      {
        BaseClassDefinition def = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsNotNull (def.ConfigurationContext);
        Assert.AreSame (classContext, def.ConfigurationContext);
      }
    }
  }
}
