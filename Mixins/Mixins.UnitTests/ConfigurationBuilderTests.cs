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
      
      Assert.IsTrue(classConfiguration.HasMixin(typeof(BT1Mixin1)));
      Assert.IsTrue(classConfiguration.HasMixin(typeof(BT1Mixin2)));
    }

    [Test]
    public void IntroducedInterface ()
    {
      ApplicationConfiguration configuration = GetConfiguration();
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType1));
      MixinConfiguration mixinConf1 = classConfiguration.GetMixin (typeof (BT1Mixin1));

      Assert.IsTrue (classConfiguration.HasIntroducedInterface (typeof (IBT1Mixin1)));
      InterfaceIntroductionConfiguration introducedInterface = classConfiguration.GetIntroducedInterface (typeof (IBT1Mixin1));
      Assert.AreSame (mixinConf1, introducedInterface.Implementer);

      Assert.IsTrue (mixinConf1.HasInterfaceIntroduction (typeof (IBT1Mixin1)));
      Assert.AreSame (introducedInterface, mixinConf1.GetInterfaceIntroduction (typeof (IBT1Mixin1)));
    }

    [Test]
    public void Methods ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType1));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

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

      MixinConfiguration mixinConf1 = classConfiguration.GetMixin (typeof (BT1Mixin1));

      Assert.IsFalse (mixinConf1.HasMember (baseMethod1));
      Assert.IsTrue(mixinConf1.HasMember(mixinMethod1));
      member = mixinConf1.GetMember (mixinMethod1);

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualMethod", member.FullName);
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
      MixinConfiguration mixinConf1 = classConfiguration.GetMixin (typeof (BT1Mixin1));
      MixinConfiguration mixinConf2 = classConfiguration.GetMixin (typeof (BT1Mixin2));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      MemberConfiguration overridden = classConfiguration.GetMember(baseMethod1);

      Assert.IsTrue (overridden.HasOverride (typeof(BT1Mixin1)));
      MemberConfiguration overrider = overridden.GetOverride (typeof (BT1Mixin1));
      
      Assert.AreSame (overrider, mixinConf1.GetMember(mixinMethod1));
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame(overridden, overrider.Base);

      MemberConfiguration notOverridden = classConfiguration.GetMember (baseMethod2);
      Assert.AreEqual (0, new List<MemberConfiguration>(notOverridden.Overrides).Count);

      Assert.IsTrue (overridden.HasOverride (typeof (BT1Mixin2)));
      overrider = overridden.GetOverride (typeof (BT1Mixin2));

      Assert.IsTrue (new List<MemberConfiguration> (mixinConf2.Overrides).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
    }

    [Test]
    public void MixinAppliedToInterface ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      Assert.IsFalse (configuration.HasBaseClassConfiguration (typeof (BaseType2)));
      Assert.IsTrue (configuration.HasBaseClassConfiguration (typeof (IBaseType2)));
      
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (IBaseType2));
      Assert.IsTrue (classConfiguration.IsInterface);

      MethodInfo method = typeof (IBaseType2).GetMethod ("IfcMethod");
      Assert.IsNotNull (method);

      MemberConfiguration member = classConfiguration.GetMember (method);
      Assert.IsNotNull (member);

      MixinConfiguration mixin = classConfiguration.GetMixin (typeof (BT2Mixin1));
      Assert.IsNotNull (mixin);
      Assert.AreSame (classConfiguration, mixin.BaseClass);
    }

    [Test]
    [Ignore("Merge not yet implemented")]
    public void MergeNoMixins ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      Assert.IsFalse (configuration.HasBaseClassConfiguration (typeof (DateTime)));

      BaseClassConfiguration classConfiguration = ConfigurationBuilder.GetMergedConfiguration (typeof (DateTime), configuration);
      Assert.IsNull (classConfiguration);
    }

    [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeStandardMixins ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      Assert.IsTrue (configuration.HasBaseClassConfiguration (typeof (BaseType1)));

      BaseClassConfiguration classConfiguration = ConfigurationBuilder.GetMergedConfiguration (typeof (BaseType1), configuration);
      Assert.IsNotNull (classConfiguration);
      Assert.AreSame (classConfiguration, configuration.GetBaseClassConfiguration (typeof (BaseType1)));
    }

    [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeInterfaceMixins ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      Assert.IsFalse (configuration.HasBaseClassConfiguration (typeof (BaseType2)));
      Assert.IsTrue (configuration.HasBaseClassConfiguration (typeof (IBaseType2)));

      BaseClassConfiguration interfaceConfiguration = configuration.GetBaseClassConfiguration (typeof (IBaseType2));
      Assert.IsTrue (interfaceConfiguration.IsInterface);
      Assert.AreEqual (typeof (IBaseType2), interfaceConfiguration.Type);

      BaseClassConfiguration classConfiguration = ConfigurationBuilder.GetMergedConfiguration (typeof (BaseType2), configuration);
      Assert.IsNotNull (classConfiguration);
      Assert.AreNotSame (interfaceConfiguration, classConfiguration);

      List<BaseClassConfiguration> originalConfigurations = new List<BaseClassConfiguration> (configuration.BaseClassConfigurations);
      Assert.IsFalse (originalConfigurations.Contains (classConfiguration));

      Assert.AreEqual (typeof (BaseType2), classConfiguration.Type);

      MethodInfo methodOfInterface = typeof (IBaseType2).GetMethod ("IfcMethod");
      MethodInfo methodOfClass = typeof (BaseType2).GetMethod ("IfcMethod");

      Assert.IsTrue (interfaceConfiguration.HasMember (methodOfInterface));
      Assert.IsFalse (interfaceConfiguration.HasMember (methodOfClass));

      Assert.IsTrue (classConfiguration.HasMember (methodOfClass));
      Assert.IsFalse (classConfiguration.HasMember (methodOfInterface));

      Assert.Contains (typeof (IBaseType2), new List<Type> (classConfiguration.ImplementedInterfaces));
      Assert.IsTrue (classConfiguration.HasRequiredFaceInterface (typeof (IBaseType2)));
    }

    [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeSeveralInterfaceAndStandardMixins ()
    {
      Assert.Fail ();
    }

    [Test]
    public void InitializationMethod ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType1));
      MixinConfiguration mixinConf = classConfiguration.GetMixin (typeof (BT1Mixin1));
      Assert.AreEqual (0, new List<MethodConfiguration>(mixinConf.InitializationMethods).Count);

      classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType3));
      mixinConf = classConfiguration.GetMixin (typeof (BT3Mixin1));
      
      List<MethodConfiguration> initializationMethods = new List<MethodConfiguration> (mixinConf.InitializationMethods);
      Assert.AreEqual (1, initializationMethods.Count);

      MethodInfo methodInfo = typeof (BT3Mixin1).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (methodInfo);
      
      Assert.AreEqual (methodInfo, initializationMethods[0].MethodInfo);
    }

    [Test]
    public void FaceInterfaces ()
    {
      ApplicationConfiguration configuration = GetConfiguration ();
      BaseClassConfiguration classConfiguration = configuration.GetBaseClassConfiguration (typeof (BaseType3));

      List<Type> requiredFaceInterfaces = new List<Type> (classConfiguration.RequiredFaceInterfaces);
      Assert.Contains (typeof (IBaseType31), requiredFaceInterfaces);
      Assert.Contains (typeof (IBaseType32), requiredFaceInterfaces);
      Assert.Contains (typeof (IBaseType33), requiredFaceInterfaces);
      Assert.Contains (typeof (IBaseType34), requiredFaceInterfaces);
      Assert.IsFalse (requiredFaceInterfaces.Contains (typeof (IBaseType35)));
    }
  }
}
