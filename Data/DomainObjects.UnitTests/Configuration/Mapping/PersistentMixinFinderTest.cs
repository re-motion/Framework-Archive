using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Mixins;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PersistentMixinFinderTest
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

    public class GenericMixin<T> : DomainObjectMixin<T>
        where T : DomainObject
    {
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "The persistence-relevant mixin "
        + "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PersistentMixinFinderTest+GenericMixin`1 applied to class "
        + "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order has open generic type parameters. All type parameters of the mixin must be "
        + "specified when it is applied to a DomainObject.")]
    public void ForInvalidOpenGenericMixin ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (GenericMixin<>)))
      {
        PersistentMixinFinder.GetPersistentMixins (typeof (Order));
      }
    }

    private void CheckPersistentMixins (Type targetType, params Type[] expectedTypes)
    {
      List<Type> mixinTypes = PersistentMixinFinder.GetPersistentMixins (targetType);
      Assert.That (mixinTypes, Is.EquivalentTo (expectedTypes));
    }
  }
}