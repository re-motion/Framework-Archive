using System;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Configuration.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationBuildTests
  {
    [Test]
    public void BuildNew ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
      using (MixinConfiguration.BuildNew ().ForClass<BaseType7> ().AddMixin<BT7Mixin0> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType7)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (BaseType7)).ContainsMixin (typeof (BT7Mixin0)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFrom ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      MixinConfiguration parentConfiguration = new MixinConfiguration();
      parentConfiguration.GetOrAddClassContext (typeof (BaseType2)).AddMixin (typeof (BT2Mixin1));
      
      Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
      using (MixinConfiguration.BuildFrom (parentConfiguration).ForClass<BaseType7> ().AddMixin<BT7Mixin0> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.AreNotSame (parentConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType7)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (BaseType7)).ContainsMixin (typeof (BT7Mixin0)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType2)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFromActive ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType4)));
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType4> ().AddMixin<BT4Mixin1> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType4)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (BaseType4)).ContainsMixin (typeof (BT4Mixin1)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFromActive_CausesDefaultConfigurationToBeAnalyzed ()
    {
      MixinConfiguration.SetActiveConfiguration (null);
      Assert.IsFalse (MixinConfiguration.HasActiveConfiguration);
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)).EnterScope ())
      {
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType3)));
      }
      Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
    }

  }
}