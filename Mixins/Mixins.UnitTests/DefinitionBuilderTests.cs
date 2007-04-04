using System;
using System.Collections.Generic;
using Mixins.Definitions.Building;
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

      Assert.IsTrue(application.BaseClasses.HasItem(typeof(BaseType1)));
      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType1));
      
      Assert.IsTrue(baseClass.Mixins.HasItem(typeof(BT1Mixin1)));
      Assert.IsTrue(baseClass.Mixins.HasItem(typeof(BT1Mixin2)));
    }

    [Test]
    public void IntroducedInterface ()
    {
      ApplicationDefinition application = GetApplicationDefinition();
      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType1));
      MixinDefinition mixin1 = baseClass.Mixins.Get (typeof (BT1Mixin1));

      Assert.IsTrue (mixin1.InterfaceIntroductions.HasItem (typeof (IBT1Mixin1)));
      InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions.Get (typeof (IBT1Mixin1));
      Assert.AreSame (mixin1, introducedInterface.Implementer);
      List<InterfaceIntroductionDefinition> introducedInterfaces = new List<InterfaceIntroductionDefinition> (baseClass.IntroducedInterfaces);
      Assert.Contains(introducedInterface, introducedInterfaces);
      Assert.AreSame (baseClass, introducedInterface.BaseClass);
    }

    [Test]
    public void Methods ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType1));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      Assert.IsTrue (baseClass.Members.HasItem (baseMethod1));
      Assert.IsFalse (baseClass.Members.HasItem(mixinMethod1));
      MemberDefinition member = baseClass.Members.Get(baseMethod1);

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (baseClass, member.DeclaringClass);

      Assert.IsTrue (baseClass.Members.HasItem (baseMethod2));
      Assert.AreNotSame (member, baseClass.Members.Get (baseMethod2));

      MixinDefinition mixin1 = baseClass.Mixins.Get (typeof (BT1Mixin1));

      Assert.IsFalse (mixin1.Members.HasItem (baseMethod1));
      Assert.IsTrue(mixin1.Members.HasItem(mixinMethod1));
      member = mixin1.Members.Get (mixinMethod1);

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
      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType1));
      MixinDefinition mixin1 = baseClass.Mixins.Get (typeof (BT1Mixin1));
      MixinDefinition mixin2 = baseClass.Mixins.Get (typeof (BT1Mixin2));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      MemberDefinition overridden = baseClass.Members.Get(baseMethod1);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof(BT1Mixin1)));
      MemberDefinition overrider = overridden.Overrides.Get (typeof (BT1Mixin1));
      
      Assert.AreSame (overrider, mixin1.Members.Get(mixinMethod1));
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame(overridden, overrider.Base);

      MemberDefinition notOverridden = baseClass.Members.Get (baseMethod2);
      Assert.AreEqual (0, new List<MemberDefinition>(notOverridden.Overrides).Count);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin2)));
      overrider = overridden.Overrides.Get (typeof (BT1Mixin2));

      Assert.IsTrue (new List<MemberDefinition> (mixin2.Overrides).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
    }

    [Test]
    public void OverrideNonVirtual ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType4), typeof (BT4Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType4));
      MixinDefinition mixin = baseClass.Mixins.Get(typeof(BT4Mixin1));
      Assert.IsNotNull (mixin);
      MemberDefinition overrider = mixin.Members.Get(typeof(BT4Mixin1).GetMethod("NonVirtualMethod"));
      Assert.IsNotNull(overrider);
      Assert.IsNotNull(overrider.Base);

      Assert.AreSame(baseClass, overrider.Base.DeclaringClass);

      List<MemberDefinition> overrides = new List<MemberDefinition> (baseClass.Members.Get (typeof (BaseType4).GetMethod ("NonVirtualMethod")).Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    [ExpectedException(typeof( ConfigurationException), ExpectedMessage = "Could not find base member for overrider Mixins.UnitTests.SampleTypes.BT5Mixin1.Method.")]
    public void ThrowsWhenInexistingOverrideBase ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType5));
      MixinDefinition mixin = baseClass.Mixins.Get (typeof (BT5Mixin1));
      Assert.IsNotNull (mixin);
      MemberDefinition overrider = mixin.Members.Get (typeof (BT5Mixin1).GetMethod ("NonVirtualMethod"));
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (baseClass, overrider.Base.DeclaringClass);

      List<MemberDefinition> overrides = new List<MemberDefinition> (baseClass.Members.Get (typeof (BaseType5).GetMethod ("NonVirtualMethod")).Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    public void MixinAppliedToInterface ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsFalse (application.BaseClasses.HasItem (typeof (BaseType2)));
      Assert.IsTrue (application.BaseClasses.HasItem (typeof (IBaseType2)));
      
      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (IBaseType2));
      Assert.IsTrue (baseClass.IsInterface);

      MethodInfo method = typeof (IBaseType2).GetMethod ("IfcMethod");
      Assert.IsNotNull (method);

      MemberDefinition member = baseClass.Members.Get (method);
      Assert.IsNotNull (member);

      MixinDefinition mixin = baseClass.Mixins.Get (typeof (BT2Mixin1));
      Assert.IsNotNull (mixin);
      Assert.AreSame (baseClass, mixin.BaseClass);
    }

    // [Test]
    [Ignore("Merge not yet implemented")]
    public void MergeNoMixins ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsFalse (application.BaseClasses.HasItem (typeof (DateTime)));

      BaseClassDefinition baseClass = DefinitionBuilder.GetMergedBaseClassDefinition (typeof (DateTime), application);
      Assert.IsNull (baseClass);
    }

    // [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeStandardMixins ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsTrue (application.BaseClasses.HasItem (typeof (BaseType1)));

      BaseClassDefinition baseClass = DefinitionBuilder.GetMergedBaseClassDefinition (typeof (BaseType1), application);
      Assert.IsNotNull (baseClass);
      Assert.AreSame (baseClass, application.BaseClasses.Get (typeof (BaseType1)));
    }

    // [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeInterfaceMixins ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsFalse (application.BaseClasses.HasItem (typeof (BaseType2)));
      Assert.IsTrue (application.BaseClasses.HasItem (typeof (IBaseType2)));

      BaseClassDefinition interfaceDefinition = application.BaseClasses.Get (typeof (IBaseType2));
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

      Assert.IsTrue (interfaceDefinition.Members.HasItem (methodOfInterface));
      Assert.IsFalse (interfaceDefinition.Members.HasItem (methodOfClass));

      Assert.IsTrue (baseClass.Members.HasItem (methodOfClass));
      Assert.IsFalse (baseClass.Members.HasItem (methodOfInterface));

      Assert.Contains (typeof (IBaseType2), new List<Type> (baseClass.ImplementedInterfaces));
      Assert.IsTrue (baseClass.RequiredBaseCallTypes.HasItem (typeof (IBaseType2)));
    }

    // [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeSeveralInterfaceAndStandardMixins ()
    {
      Assert.Fail ();
    }

    [Test]
    public void InitializationMethod ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType1));
      MixinDefinition mixin = baseClass.Mixins.Get (typeof (BT1Mixin1));
      Assert.AreEqual (0, new List<MethodDefinition>(mixin.InitializationMethods).Count);

      baseClass = application.BaseClasses.Get (typeof (BaseType3));
      mixin = baseClass.Mixins.Get (typeof (BT3Mixin1));
      
      List<MethodDefinition> initializationMethods = new List<MethodDefinition> (mixin.InitializationMethods);
      Assert.AreEqual (1, initializationMethods.Count);

      MethodInfo methodInfo = typeof (BT3Mixin1).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (methodInfo);
      
      Assert.AreEqual (methodInfo, initializationMethods[0].MethodInfo);

      BaseType3 @this = new BaseType3();
      BaseType3 @base = new BaseType3();
      BT3Mixin1 mixinInstance = new BT3Mixin1 ();
      initializationMethods[0].MethodInfo.Invoke (mixinInstance, new object[] { @this, @base });
      Assert.AreSame (@this, mixinInstance.This);
      Assert.AreSame (@base, mixinInstance.Base);
    }

    [Test]
    public void FaceInterfaces ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType3));

      List<Type> requiredFaceTypes = new List<RequiredFaceTypeDefinition> (baseClass.RequiredFaceTypes).ConvertAll<Type>
          (delegate(RequiredFaceTypeDefinition def) { return def.Type; });
      Assert.Contains (typeof (IBaseType31), requiredFaceTypes);
      Assert.Contains (typeof (IBaseType32), requiredFaceTypes);
      Assert.Contains (typeof (IBaseType33), requiredFaceTypes);
      Assert.IsFalse (requiredFaceTypes.Contains (typeof (IBaseType34)));
      Assert.IsFalse (requiredFaceTypes.Contains (typeof (IBaseType35)));
    }

    [Test]
    public void BaseInterfaces ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses.Get (typeof (BaseType3));

      List<Type> requiredBaseCallTypes = new List<RequiredBaseCallTypeDefinition> (baseClass.RequiredBaseCallTypes).ConvertAll<Type>
          (delegate (RequiredBaseCallTypeDefinition def) { return def.Type; });
      Assert.Contains (typeof (IBaseType31), requiredBaseCallTypes);
      Assert.Contains (typeof (IBaseType33), requiredBaseCallTypes);
      Assert.Contains (typeof (IBaseType34), requiredBaseCallTypes);
      Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (IBaseType32)));
      Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (IBaseType35)));
      Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (INull)));
    }
  }
}
