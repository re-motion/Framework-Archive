using System;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.Context;
using Mixins.Definitions.Building;

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
  }
}
