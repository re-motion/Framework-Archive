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
      ApplicationContext appContext = new ApplicationContext();
      appContext.AddClassContext (new ClassContext (typeof (BT3Mixin3<,>)));
      DefinitionBuilder.CreateApplicationDefinition (appContext);
    }

    [Test]
    public void BaseClassKnowsItsContext()
    {
      ApplicationContext appContext = new ApplicationContext ();
      ClassContext classContext = new ClassContext (typeof (BaseType1));
      appContext.AddClassContext (classContext);
      BaseClassDefinition def = DefinitionBuilder.CreateApplicationDefinition (appContext).BaseClasses[typeof(BaseType1)];
      Assert.IsNotNull (def.ConfigurationContext);
      Assert.AreSame (classContext, def.ConfigurationContext);
    }
  }
}
