using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins.Context;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ClassReflectorTests
{
  [TestFixture]
  public class GetPersistentMixins
  {
    [Test]
    public void ForTargetClassBase ()
    {
      Type targetType = typeof (TargetClassBase);
      CheckPersistentMixins (targetType, typeof (MixinBase));
    }

    [Test]
    public void ForTargetClassA ()
    {
      Type targetType = typeof (TargetClassA);
      CheckPersistentMixins (targetType, typeof (MixinA), typeof (MixinC), typeof (MixinD));
    }

    [Test]
    public void ForTargetClassB ()
    {
      Type targetType = typeof (TargetClassB);
      CheckPersistentMixins (targetType, typeof (MixinB), typeof (MixinE));
    }

    private void CheckPersistentMixins (Type targetType, params Type[] expectedTypes)
    {
      List<Type> mixinTypes = ClassReflector.GetPersistentMixins (targetType);
      Assert.That (mixinTypes, Is.EquivalentTo (expectedTypes));
    }
  }
}