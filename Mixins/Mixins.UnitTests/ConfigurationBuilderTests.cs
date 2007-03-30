using System;
using System.Collections.Generic;
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
    public void Methods ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType1));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (Mixin1ForBT1).GetMethod ("VirtualMethod", new Type[0]);

      Assert.IsTrue (classConfiguration.HasMember (baseMethod1));
      Assert.IsFalse (classConfiguration.HasMember(mixinMethod1));
      MemberConfiguration member = classConfiguration.GetMember(baseMethod1);

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (classConfiguration, member.DeclaringClass);

      Assert.IsTrue (classConfiguration.HasMember (baseMethod2));
      Assert.AreNotSame (member, classConfiguration.GetMember (baseMethod2));

      MixinConfiguration mixinConf1 = classConfiguration.GetMixin (typeof (Mixin1ForBT1));

      Assert.IsFalse (mixinConf1.HasMember (baseMethod1));
      Assert.IsTrue(mixinConf1.HasMember(mixinMethod1));
      member = mixinConf1.GetMember (mixinMethod1);

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (Mixin1ForBT1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (mixinConf1, member.DeclaringClass);
    }

    [Test]
    public void Overrides ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType1));
      MixinConfiguration mixinConf1 = classConfiguration.GetMixin(typeof(Mixin1ForBT1));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (Mixin1ForBT1).GetMethod ("VirtualMethod", new Type[0]);

      MemberConfiguration overridden = classConfiguration.GetMember(baseMethod1);

      Assert.IsTrue (overridden.HasOverride (typeof(Mixin1ForBT1)));
      MemberConfiguration overrider = overridden.GetOverride (typeof (Mixin1ForBT1));
      
      Assert.AreSame (overrider, mixinConf1.GetMember(mixinMethod1));
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame(overridden, overrider.Base);

      MemberConfiguration notOverridden = classConfiguration.GetMember (baseMethod2);
      Assert.AreEqual (0, new List<MemberConfiguration>(notOverridden.Overrides).Count);
    }
  }
}
