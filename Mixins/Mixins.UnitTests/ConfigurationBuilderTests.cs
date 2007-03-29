using System;
using NUnit.Framework;
using Mixins.Context;
using System.Reflection;
using Mixins.Configuration;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests
{
  [TestFixture]
  public class ConfigurationBuilderTests
  {
    public static ApplicationConfiguration GetConfiguration ()
    {
      ApplicationContext assemblyContext = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      return ConfigurationBuilder.CreateConfiguration (assemblyContext);
    }

    [Test]
    public void CorrectlyCopiesContext ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();

      Assert.IsTrue(configuration.HasBaseClassConfiguration(typeof(BaseType1)));
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType1));
      
      Assert.IsTrue(classConfiguration.HasMixin(typeof(Mixin1ForBT1)));
      Assert.IsTrue(classConfiguration.HasMixin(typeof(Mixin2ForBT1)));
    }

    [Test]
    public void IntroducedInterface ()
    {
      ApplicationConfiguration configuration = GetConfiguration();
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType1));
      MixinConfiguration mixinConf1 = classConfiguration.GetMixin (typeof (Mixin1ForBT1));

      Assert.IsTrue (classConfiguration.HasIntroducedInterface (typeof (IMixin1ForBT1)));
      InterfaceIntroductionConfiguration introducedInterface = classConfiguration.GetIntroducedInterface (typeof (IMixin1ForBT1));
      Assert.AreSame (mixinConf1, introducedInterface.Implementer);

      Assert.IsTrue (mixinConf1.HasInterfaceIntroduction (typeof (IMixin1ForBT1)));
      Assert.AreSame (introducedInterface, mixinConf1.GetInterfaceIntroduction (typeof (IMixin1ForBT1)));
    }

    [Test]
    public void Members ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType1));

      Assert.IsTrue (classConfiguration.HasMember (typeof (BaseType1).GetMethod ("VirtualMethod")));
      Assert.IsFalse (classConfiguration.HasMember(typeof(Mixin1ForBT1).GetMethod("VirtualMethod")));
      MemberConfiguration member = classConfiguration.GetMember(typeof (BaseType1).GetMethod ("VirtualMethod"));

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (classConfiguration, member.ParentClass);

      MixinConfiguration mixinConf1 = classConfiguration.GetMixin (typeof (Mixin1ForBT1));
      
      Assert.IsFalse (mixinConf1.HasMember (typeof (BaseType1).GetMethod ("VirtualMethod")));
      Assert.IsTrue(mixinConf1.HasMember(typeof(Mixin1ForBT1).GetMethod("VirtualMethod")));
      member = mixinConf1.GetMember (typeof (Mixin1ForBT1).GetMethod ("VirtualMethod"));

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (Mixin1ForBT1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (mixinConf1, member.ParentClass);
    }
  }
}
