using System;
using NUnit.Framework;

namespace Remotion.Mixins.UnitTests
{
  [TestFixture]
  public class MixedTypeInstantiationScopeTests
  {
    [SetUp]
    public void SetUp ()
    {
      MixedTypeInstantiationScope.SetCurrent (null);
    }

    [TearDown]
    public void TearDown ()
    {
      MixedTypeInstantiationScope.SetCurrent (null);
    }
    
    [Test]
    public void ScopeInitializedOnDemand ()
    {
      Assert.IsFalse (MixedTypeInstantiationScope.HasCurrent);
      Assert.IsNotNull (MixedTypeInstantiationScope.Current);
      Assert.IsTrue (MixedTypeInstantiationScope.HasCurrent);
    }

    [Test (Description = "Checks (in conjunction with ScopeInitializedOnDemand) whether this test fixture correctly resets the scope.")]
    public void CurrentIsReset ()
    {
      Assert.IsFalse (MixedTypeInstantiationScope.HasCurrent);
      Assert.IsNotNull (MixedTypeInstantiationScope.Current);
      Assert.IsTrue (MixedTypeInstantiationScope.HasCurrent);
    }

    [Test]
    public void DefaultMixinInstancesEmpty ()
    {
      Assert.IsNotNull (MixedTypeInstantiationScope.Current.SuppliedMixinInstances);
      Assert.AreEqual (0, MixedTypeInstantiationScope.Current.SuppliedMixinInstances.Length);
    }

    [Test]
    public void InstancesCanBeSuppliedInScopes ()
    {
      Assert.AreEqual (0, MixedTypeInstantiationScope.Current.SuppliedMixinInstances.Length);
      using (new MixedTypeInstantiationScope ("1", "2"))
      {
        Assert.AreEqual (2, MixedTypeInstantiationScope.Current.SuppliedMixinInstances.Length);
        Assert.AreEqual ("1", MixedTypeInstantiationScope.Current.SuppliedMixinInstances[0]);
        Assert.AreEqual ("2", MixedTypeInstantiationScope.Current.SuppliedMixinInstances[1]);
        
        using (new MixedTypeInstantiationScope ("a"))
        {
          Assert.AreEqual (1, MixedTypeInstantiationScope.Current.SuppliedMixinInstances.Length);
          Assert.AreEqual ("a", MixedTypeInstantiationScope.Current.SuppliedMixinInstances[0]);

          using (new MixedTypeInstantiationScope ())
          {
            Assert.AreEqual (0, MixedTypeInstantiationScope.Current.SuppliedMixinInstances.Length);
          }

          Assert.AreEqual (1, MixedTypeInstantiationScope.Current.SuppliedMixinInstances.Length);
          Assert.AreEqual ("a", MixedTypeInstantiationScope.Current.SuppliedMixinInstances[0]);
        }

        Assert.AreEqual (2, MixedTypeInstantiationScope.Current.SuppliedMixinInstances.Length);
        Assert.AreEqual ("1", MixedTypeInstantiationScope.Current.SuppliedMixinInstances[0]);
        Assert.AreEqual ("2", MixedTypeInstantiationScope.Current.SuppliedMixinInstances[1]);
      }
      Assert.AreEqual (0, MixedTypeInstantiationScope.Current.SuppliedMixinInstances.Length);
    }
  }
}