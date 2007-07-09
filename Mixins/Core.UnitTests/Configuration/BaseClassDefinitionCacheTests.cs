using System;
using System.Threading;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class BaseClassDefinitionCacheTests
  {
    [SetUp]
    [TearDown]
    public void ResetCache ()
    {
      BaseClassDefinitionCache.SetCurrent (null);
    }

    [Test]
    public void IsCached()
    {
      Assert.IsFalse (BaseClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
      BaseClassDefinitionCache.Current.GetBaseClassDefinition (new ClassContext (typeof (BaseType1)));
      Assert.IsTrue (BaseClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
    }

    [Test (Description = "Checks whether the test fixture correctly resets the cache before running the test.")]
    public void IsCached2 ()
    {
      Assert.IsFalse (BaseClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
    }

    [Test]
    public void GetBaseClassDefinitionReturnsValidClassDef ()
    {
      ClassContext context = new ClassContext (typeof (BaseType1));
      BaseClassDefinition def = BaseClassDefinitionCache.Current.GetBaseClassDefinition (context);
      Assert.IsNotNull (def);
      Assert.AreSame (context, def.ConfigurationContext);
    }

    [Test]
    public void GetBaseClassDefinitionImplementsCaching ()
    {
      BaseClassDefinition def = BaseClassDefinitionCache.Current.GetBaseClassDefinition(new ClassContext (typeof (BaseType1)));
      BaseClassDefinition def2 = BaseClassDefinitionCache.Current.GetBaseClassDefinition(new ClassContext (typeof (BaseType1)));
      Assert.IsNotNull (def);
      Assert.AreSame (def, def2);
    }

    [Test]
    public void ClassContextFrozenAfterDefinitionGeneration ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsFalse (cc.IsFrozen);
      BaseClassDefinitionCache.Current.GetBaseClassDefinition (cc);
      Assert.IsTrue (cc.IsFrozen);
    }

    [Test]
    public void ClassContextFrozenEvenIfNotNewGeneration ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsFalse (cc.IsFrozen);
      ClassDefinition cd = BaseClassDefinitionCache.Current.GetBaseClassDefinition (cc);
      Assert.IsTrue (cc.IsFrozen);

      ClassContext cc2 = new ClassContext (typeof (BaseType1));
      Assert.IsFalse (cc2.IsFrozen);
      ClassDefinition cd2 = BaseClassDefinitionCache.Current.GetBaseClassDefinition (cc2);
      Assert.AreSame (cd, cd2);
      Assert.IsTrue (cc2.IsFrozen);
    }

    [Test]
    [ExpectedException (typeof (ValidationException), ExpectedMessage = "could not be validated", MatchType = MessageMatch.Contains)]
    public void CacheValidatesWhenGeneratingDefinition()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1), typeof (BT3Mixin2));
      BaseClassDefinitionCache.Current.GetBaseClassDefinition (cc);
    }

    [Test]
    public void CurrentIsGlobalSingleton ()
    {
      BaseClassDefinitionCache newCache = new BaseClassDefinitionCache ();
      Assert.IsFalse (BaseClassDefinitionCache.HasCurrent);
      Thread setterThread = new Thread ((ThreadStart) delegate { BaseClassDefinitionCache.SetCurrent (newCache); });
      setterThread.Start ();
      setterThread.Join ();

      Assert.IsTrue (BaseClassDefinitionCache.HasCurrent);
      Assert.AreSame (newCache, BaseClassDefinitionCache.Current);
    }
  }
}
