using System;
using System.Threading;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.Configuration.Definitions
{
  [TestFixture]
  public class TargetClassDefinitionCacheTests
  {
    [SetUp]
    [TearDown]
    public void ResetCache ()
    {
      TargetClassDefinitionCache.SetCurrent (null);
    }

    [Test]
    public void IsCached()
    {
      Assert.IsFalse (TargetClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
      TargetClassDefinitionCache.Current.GetTargetClassDefinition (new ClassContext (typeof (BaseType1)));
      Assert.IsTrue (TargetClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
    }

    [Test (Description = "Checks whether the test fixture correctly resets the cache before running the test.")]
    public void IsCached2 ()
    {
      Assert.IsFalse (TargetClassDefinitionCache.Current.IsCached (new ClassContext (typeof (BaseType1))));
    }

    [Test]
    public void GetTargetClassDefinitionReturnsValidClassDef ()
    {
      ClassContext context = new ClassContext (typeof (BaseType1));
      TargetClassDefinition def = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context);
      Assert.IsNotNull (def);
      Assert.AreSame (context, def.ConfigurationContext);
    }

    [Test]
    public void GetTargetClassDefinitionImplementsCaching ()
    {
      TargetClassDefinition def = TargetClassDefinitionCache.Current.GetTargetClassDefinition(new ClassContext (typeof (BaseType1)));
      TargetClassDefinition def2 = TargetClassDefinitionCache.Current.GetTargetClassDefinition(new ClassContext (typeof (BaseType1)));
      Assert.IsNotNull (def);
      Assert.AreSame (def, def2);
    }

    [Test]
    public void ClassContextFrozenAfterDefinitionGeneration ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsFalse (cc.IsFrozen);
      TargetClassDefinitionCache.Current.GetTargetClassDefinition (cc);
      Assert.IsTrue (cc.IsFrozen);
    }

    [Test]
    public void ClassContextFrozenEvenIfNotNewGeneration ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsFalse (cc.IsFrozen);
      ClassDefinitionBase cd = TargetClassDefinitionCache.Current.GetTargetClassDefinition (cc);
      Assert.IsTrue (cc.IsFrozen);

      ClassContext cc2 = new ClassContext (typeof (BaseType1));
      Assert.IsFalse (cc2.IsFrozen);
      ClassDefinitionBase cd2 = TargetClassDefinitionCache.Current.GetTargetClassDefinition (cc2);
      Assert.AreSame (cd, cd2);
      Assert.IsTrue (cc2.IsFrozen);
    }

    [Test]
    [ExpectedException (typeof (ValidationException))]
    public void CacheValidatesWhenGeneratingDefinition()
    {
      ClassContext cc = new ClassContext (typeof (DateTime));
      TargetClassDefinitionCache.Current.GetTargetClassDefinition (cc);
    }

    [Test]
    public void CurrentIsGlobalSingleton ()
    {
      TargetClassDefinitionCache newCache = new TargetClassDefinitionCache ();
      Assert.IsFalse (TargetClassDefinitionCache.HasCurrent);
      Thread setterThread = new Thread ((ThreadStart) delegate { TargetClassDefinitionCache.SetCurrent (newCache); });
      setterThread.Start ();
      setterThread.Join ();

      Assert.IsTrue (TargetClassDefinitionCache.HasCurrent);
      Assert.AreSame (newCache, TargetClassDefinitionCache.Current);
    }
  }
}