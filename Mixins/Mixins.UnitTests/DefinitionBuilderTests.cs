using System;
using System.Collections.Generic;
using NUnit.Framework;
using Mixins.Context;
using System.Reflection;
using Mixins.Definitions;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests
{
  [TestFixture]
  public class DefinitionBuilderTests
  {
    public static ApplicationDefinition GetApplicationDefinition ()
    {
      ApplicationContext assemblyContext = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      return DefinitionBuilder.CreateApplicationDefinition (assemblyContext);
    }

    [Test]
    public void CorrectlyCopiesContext ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();

      Assert.IsTrue(application.HasBaseClass(typeof(BaseType1)));
      BaseClassDefinition baseClass = application.GetBaseClass (typeof (BaseType1));
      
      Assert.IsTrue(baseClass.HasMixin(typeof(BT1Mixin1)));
      Assert.IsTrue(baseClass.HasMixin(typeof(BT1Mixin2)));
    }

    [Test]
    public void IntroducedInterface ()
    {
      ApplicationDefinition application = GetApplicationDefinition();
      BaseClassDefinition baseClass = application.GetBaseClass (typeof (BaseType1));
      MixinDefinition mixin1 = baseClass.GetMixin (typeof (BT1Mixin1));

      Assert.IsTrue (baseClass.HasIntroducedInterface (typeof (IBT1Mixin1)));
      InterfaceIntroductionDefinition introducedInterface = baseClass.GetIntroducedInterface (typeof (IBT1Mixin1));
      Assert.AreSame (mixin1, introducedInterface.Implementer);

      Assert.IsTrue (mixin1.HasInterfaceIntroduction (typeof (IBT1Mixin1)));
      Assert.AreSame (introducedInterface, mixin1.GetInterfaceIntroduction (typeof (IBT1Mixin1)));
    }

    [Test]
    public void Methods ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.GetBaseClass (typeof (BaseType1));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      Assert.IsTrue (baseClass.HasMember (baseMethod1));
      Assert.IsFalse (baseClass.HasMember(mixinMethod1));
      MemberDefinition member = baseClass.GetMember(baseMethod1);

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (baseClass, member.DeclaringClass);

      Assert.IsTrue (baseClass.HasMember (baseMethod2));
      Assert.AreNotSame (member, baseClass.GetMember (baseMethod2));

      MixinDefinition mixin1 = baseClass.GetMixin (typeof (BT1Mixin1));

      Assert.IsFalse (mixin1.HasMember (baseMethod1));
      Assert.IsTrue(mixin1.HasMember(mixinMethod1));
      member = mixin1.GetMember (mixinMethod1);

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (mixin1, member.DeclaringClass);
    }

    [Test]
    public void Overrides ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.GetBaseClass (typeof (BaseType1));
      MixinDefinition mixin1 = baseClass.GetMixin (typeof (BT1Mixin1));
      MixinDefinition mixin2 = baseClass.GetMixin (typeof (BT1Mixin2));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      MemberDefinition overridden = baseClass.GetMember(baseMethod1);

      Assert.IsTrue (overridden.HasOverride (typeof(BT1Mixin1)));
      MemberDefinition overrider = overridden.GetOverride (typeof (BT1Mixin1));
      
      Assert.AreSame (overrider, mixin1.GetMember(mixinMethod1));
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame(overridden, overrider.Base);

      MemberDefinition notOverridden = baseClass.GetMember (baseMethod2);
      Assert.AreEqual (0, new List<MemberDefinition>(notOverridden.Overrides).Count);

      Assert.IsTrue (overridden.HasOverride (typeof (BT1Mixin2)));
      overrider = overridden.GetOverride (typeof (BT1Mixin2));

      Assert.IsTrue (new List<MemberDefinition> (mixin2.Overrides).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
    }

    [Test]
    public void MixinAppliedToInterface ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsFalse (application.HasBaseClass (typeof (BaseType2)));
      Assert.IsTrue (application.HasBaseClass (typeof (IBaseType2)));
      
      BaseClassDefinition baseClass = application.GetBaseClass (typeof (IBaseType2));
      Assert.IsTrue (baseClass.IsInterface);

      MethodInfo method = typeof (IBaseType2).GetMethod ("IfcMethod");
      Assert.IsNotNull (method);

      MemberDefinition member = baseClass.GetMember (method);
      Assert.IsNotNull (member);

      MixinDefinition mixin = baseClass.GetMixin (typeof (BT2Mixin1));
      Assert.IsNotNull (mixin);
      Assert.AreSame (baseClass, mixin.BaseClass);
    }

    [Test]
    [Ignore("Merge not yet implemented")]
    public void MergeNoMixins ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsFalse (application.HasBaseClass (typeof (DateTime)));

      BaseClassDefinition baseClass = DefinitionBuilder.GetMergedBaseClassDefinition (typeof (DateTime), application);
      Assert.IsNull (baseClass);
    }

    [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeStandardMixins ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsTrue (application.HasBaseClass (typeof (BaseType1)));

      BaseClassDefinition baseClass = DefinitionBuilder.GetMergedBaseClassDefinition (typeof (BaseType1), application);
      Assert.IsNotNull (baseClass);
      Assert.AreSame (baseClass, application.GetBaseClass (typeof (BaseType1)));
    }

    [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeInterfaceMixins ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsFalse (application.HasBaseClass (typeof (BaseType2)));
      Assert.IsTrue (application.HasBaseClass (typeof (IBaseType2)));

      BaseClassDefinition interfaceDefinition = application.GetBaseClass (typeof (IBaseType2));
      Assert.IsTrue (interfaceDefinition.IsInterface);
      Assert.AreEqual (typeof (IBaseType2), interfaceDefinition.Type);

      BaseClassDefinition baseClass = DefinitionBuilder.GetMergedBaseClassDefinition (typeof (BaseType2), application);
      Assert.IsNotNull (baseClass);
      Assert.AreNotSame (interfaceDefinition, baseClass);

      List<BaseClassDefinition> originalDefinitions = new List<BaseClassDefinition> (application.BaseClasses);
      Assert.IsFalse (originalDefinitions.Contains (baseClass));

      Assert.AreEqual (typeof (BaseType2), baseClass.Type);

      MethodInfo methodOfInterface = typeof (IBaseType2).GetMethod ("IfcMethod");
      MethodInfo methodOfClass = typeof (BaseType2).GetMethod ("IfcMethod");

      Assert.IsTrue (interfaceDefinition.HasMember (methodOfInterface));
      Assert.IsFalse (interfaceDefinition.HasMember (methodOfClass));

      Assert.IsTrue (baseClass.HasMember (methodOfClass));
      Assert.IsFalse (baseClass.HasMember (methodOfInterface));

      Assert.Contains (typeof (IBaseType2), new List<Type> (baseClass.ImplementedInterfaces));
      Assert.IsTrue (baseClass.HasRequiredFaceInterface (typeof (IBaseType2)));
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
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.GetBaseClass (typeof (BaseType1));
      MixinDefinition mixin = baseClass.GetMixin (typeof (BT1Mixin1));
      Assert.AreEqual (0, new List<MethodDefinition>(mixin.InitializationMethods).Count);

      baseClass = application.GetBaseClass (typeof (BaseType3));
      mixin = baseClass.GetMixin (typeof (BT3Mixin1));
      
      List<MethodDefinition> initializationMethods = new List<MethodDefinition> (mixin.InitializationMethods);
      Assert.AreEqual (1, initializationMethods.Count);

      MethodInfo methodInfo = typeof (BT3Mixin1).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (methodInfo);
      
      Assert.AreEqual (methodInfo, initializationMethods[0].MethodInfo);
    }

    [Test]
    public void FaceInterfaces ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.GetBaseClass (typeof (BaseType3));

      List<Type> requiredFaceInterfaces = new List<Type> (baseClass.RequiredFaceInterfaces);
      Assert.Contains (typeof (IBaseType31), requiredFaceInterfaces);
      Assert.Contains (typeof (IBaseType32), requiredFaceInterfaces);
      Assert.Contains (typeof (IBaseType33), requiredFaceInterfaces);
      Assert.Contains (typeof (IBaseType34), requiredFaceInterfaces);
      Assert.IsFalse (requiredFaceInterfaces.Contains (typeof (IBaseType35)));
    }
  }
}
