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
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      
      Assert.IsTrue(baseClass.Mixins.HasItem(typeof(BT1Mixin1)));
      Assert.IsTrue(baseClass.Mixins.HasItem(typeof(BT1Mixin2)));
    }

    [Test]
    public void IntroducedInterface ()
    {
      ApplicationDefinition application = GetApplicationDefinition();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsTrue (mixin1.InterfaceIntroductions.HasItem (typeof (IBT1Mixin1)));
      InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];
      Assert.AreSame (mixin1, introducedInterface.Implementer);
      Assert.IsTrue(baseClass.IntroducedInterfaces.HasItem(typeof(IBT1Mixin1)));
      Assert.AreSame(baseClass.IntroducedInterfaces[typeof (IBT1Mixin1)], introducedInterface);
      Assert.AreSame (baseClass, introducedInterface.BaseClass);
    }

    [Test]
    public void Methods ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      Assert.IsTrue (baseClass.Members.HasItem (baseMethod1));
      Assert.IsFalse (baseClass.Members.HasItem(mixinMethod1));
      MemberDefinition member = baseClass.Members[baseMethod1];

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (baseClass, member.DeclaringClass);

      Assert.IsTrue (baseClass.Members.HasItem (baseMethod2));
      Assert.AreNotSame (member, baseClass.Members[baseMethod2]);

      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsFalse (mixin1.Members.HasItem (baseMethod1));
      Assert.IsTrue(mixin1.Members.HasItem(mixinMethod1));
      member = mixin1.Members[mixinMethod1];

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
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      MemberDefinition overridden = baseClass.Members[baseMethod1];

      Assert.IsTrue (overridden.Overrides.HasItem (typeof(BT1Mixin1)));
      MemberDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];
      
      Assert.AreSame (overrider, mixin1.Members[mixinMethod1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame(overridden, overrider.Base);

      MemberDefinition notOverridden = baseClass.Members[baseMethod2];
      Assert.AreEqual (0, new List<MemberDefinition>(notOverridden.Overrides).Count);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.Overrides).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
    }

    [Test]
    public void OverrideNonVirtual ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType4), typeof (BT4Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType4)];
      MixinDefinition mixin = baseClass.Mixins[typeof(BT4Mixin1)];
      Assert.IsNotNull (mixin);
      MemberDefinition overrider = mixin.Members[typeof(BT4Mixin1).GetMethod("NonVirtualMethod")];
      Assert.IsNotNull(overrider);
      Assert.IsNotNull(overrider.Base);

      Assert.AreSame(baseClass, overrider.Base.DeclaringClass);

      List<MemberDefinition> overrides = new List<MemberDefinition> (baseClass.Members[typeof (BaseType4).GetMethod ("NonVirtualMethod")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    [ExpectedException(typeof( ConfigurationException), ExpectedMessage = "Could not find base member for overrider Mixins.UnitTests.SampleTypes.BT5Mixin1.Method.")]
    public void ThrowsWhenInexistingOverrideBase ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType5)];
      MixinDefinition mixin = baseClass.Mixins[typeof (BT5Mixin1)];
      Assert.IsNotNull (mixin);
      MemberDefinition overrider = mixin.Members[typeof (BT5Mixin1).GetMethod ("NonVirtualMethod")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (baseClass, overrider.Base.DeclaringClass);

      List<MemberDefinition> overrides = new List<MemberDefinition> (baseClass.Members[typeof (BaseType5).GetMethod ("NonVirtualMethod")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    public void MixinAppliedToInterface ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsFalse (application.BaseClasses.HasItem (typeof (BaseType2)));
      Assert.IsTrue (application.BaseClasses.HasItem (typeof (IBaseType2)));
      
      BaseClassDefinition baseClass = application.BaseClasses[typeof (IBaseType2)];
      Assert.IsTrue (baseClass.IsInterface);

      MethodInfo method = typeof (IBaseType2).GetMethod ("IfcMethod");
      Assert.IsNotNull (method);

      MemberDefinition member = baseClass.Members[method];
      Assert.IsNotNull (member);

      MixinDefinition mixin = baseClass.Mixins[typeof (BT2Mixin1)];
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
      Assert.AreSame (baseClass, application.BaseClasses[typeof (BaseType1)]);
    }

    // [Test]
    [Ignore ("Merge not yet implemented")]
    public void MergeInterfaceMixins ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      Assert.IsFalse (application.BaseClasses.HasItem (typeof (BaseType2)));
      Assert.IsTrue (application.BaseClasses.HasItem (typeof (IBaseType2)));

      BaseClassDefinition interfaceDefinition = application.BaseClasses[typeof (IBaseType2)];
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
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin = baseClass.Mixins[typeof (BT1Mixin1)];
      Assert.AreEqual (0, new List<MethodDefinition>(mixin.InitializationMethods).Count);

      baseClass = application.BaseClasses[typeof (BaseType3)];
      mixin = baseClass.Mixins[typeof (BT3Mixin1)];
      
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
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType3)];

      List<Type> requiredFaceTypes = new List<RequiredFaceTypeDefinition> (baseClass.RequiredFaceTypes).ConvertAll<Type>
          (delegate(RequiredFaceTypeDefinition def) { return def.Type; });
      Assert.Contains (typeof (IBaseType31), requiredFaceTypes);
      Assert.Contains (typeof (IBaseType32), requiredFaceTypes);
      Assert.Contains (typeof (IBaseType33), requiredFaceTypes);
      Assert.IsFalse (requiredFaceTypes.Contains (typeof (IBaseType34)));
      Assert.IsFalse (requiredFaceTypes.Contains (typeof (IBaseType35)));

      List<MixinDefinition> requirers = new List<MixinDefinition> (baseClass.RequiredFaceTypes[typeof (IBaseType31)].Requirers);
      Assert.Contains (baseClass.Mixins[typeof (BT3Mixin1)], requirers);
      Assert.Contains (baseClass.Mixins[typeof (BT3Mixin6<,>)], requirers);
      Assert.AreEqual (2, requirers.Count);
    }

    [Test]
    public void BaseInterfaces ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType3)];

      List<Type> requiredBaseCallTypes = new List<RequiredBaseCallTypeDefinition> (baseClass.RequiredBaseCallTypes).ConvertAll<Type>
          (delegate (RequiredBaseCallTypeDefinition def) { return def.Type; });
      Assert.Contains (typeof (IBaseType31), requiredBaseCallTypes);
      Assert.Contains (typeof (IBaseType33), requiredBaseCallTypes);
      Assert.Contains (typeof (IBaseType34), requiredBaseCallTypes);
      Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (IBaseType32)));
      Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (IBaseType35)));
      Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (INull)));

      List<MixinDefinition> requirers = new List<MixinDefinition> (baseClass.RequiredBaseCallTypes[typeof (IBaseType33)].Requirers);
      Assert.Contains (baseClass.Mixins[typeof (BT3Mixin3<,>)], requirers);
      Assert.AreEqual (1, requirers.Count);

    }

    [Test]
    public void Dependencies ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      MixinDefinition bt3Mixin1 = application.BaseClasses[typeof (BaseType3)].Mixins[typeof(BT3Mixin1)];
      
      Assert.IsTrue (bt3Mixin1.ThisDependencies.HasItem (typeof (IBaseType31)));
      List<ThisDependencyDefinition> bt3m1ThisDeps = new List<ThisDependencyDefinition> (bt3Mixin1.ThisDependencies);
      Assert.AreEqual (1, bt3m1ThisDeps.Count);

      Assert.IsTrue (bt3Mixin1.BaseDependencies.HasItem (typeof (IBaseType31)));
      List<BaseDependencyDefinition> bt3m1BaseDeps = new List<BaseDependencyDefinition> (bt3Mixin1.BaseDependencies);
      Assert.AreEqual (1, bt3m1BaseDeps.Count);

      MixinDefinition bt3Mixin2 = application.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin2)];
      Assert.IsTrue (bt3Mixin2.ThisDependencies.HasItem (typeof (IBaseType32)));
      List<ThisDependencyDefinition> bt3m2ThisDeps = new List<ThisDependencyDefinition> (bt3Mixin2.ThisDependencies);
      Assert.AreEqual (1, bt3m2ThisDeps.Count);

      List<BaseDependencyDefinition> bt3m2BaseDeps = new List<BaseDependencyDefinition> (bt3Mixin2.BaseDependencies);
      Assert.IsEmpty (bt3m2BaseDeps);

      MixinDefinition bt3Mixin6 = application.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin6<,>)];
      
      Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType31)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType32)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType33)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBT3Mixin4)));
      Assert.IsFalse (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType34)));

      Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBaseType31)].IsAggregateOnly);
      Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].IsAggregateOnly);

      Assert.AreEqual (application.BaseClasses[typeof (BaseType3)].RequiredFaceTypes[typeof (IBaseType31)], bt3Mixin6.ThisDependencies[typeof (IBaseType31)].RequiredType);

      Assert.AreSame (application.BaseClasses[typeof (BaseType3)], bt3Mixin6.ThisDependencies[typeof (IBaseType32)].GetImplementer ());
      Assert.AreSame (application.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin4)],
          bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].GetImplementer ());

      Assert.IsTrue (bt3Mixin6.BaseDependencies.HasItem (typeof (IBaseType34)));
      Assert.IsTrue (bt3Mixin6.BaseDependencies.HasItem (typeof (IBT3Mixin4)));
      Assert.IsFalse (bt3Mixin6.BaseDependencies.HasItem (typeof (IBaseType31)));
      Assert.IsFalse (bt3Mixin6.BaseDependencies.HasItem (typeof (IBaseType32)));
      Assert.IsFalse (bt3Mixin6.BaseDependencies.HasItem (typeof (IBaseType33)));

      Assert.AreEqual (application.BaseClasses[typeof (BaseType3)].RequiredBaseCallTypes[typeof (IBaseType34)], bt3Mixin6.BaseDependencies[typeof (IBaseType34)].RequiredType);

      Assert.AreSame (application.BaseClasses[typeof (BaseType3)], bt3Mixin6.BaseDependencies[typeof (IBaseType34)].GetImplementer ());
      Assert.AreSame (application.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin4)],
          bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].GetImplementer ());

      Assert.IsFalse (bt3Mixin6.BaseDependencies[typeof(IBT3Mixin4)].IsAggregateOnly);
      Assert.IsFalse (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].IsAggregateOnly);
    }

    [Test]
    [Ignore ("Todo")]
    public void CompleteInterfacesAndDependencies ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7));
      BaseClassDefinition bt3 = application.BaseClasses[typeof (BaseType3)];
      
      MixinDefinition m4 = bt3.Mixins[typeof (BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof (BT3Mixin7)];

      ThisDependencyDefinition d1 = m7.ThisDependencies[typeof (ICBaseType3)];
      Assert.IsNull (d1.GetImplementer());

      BaseDependencyDefinition d2 = m7.BaseDependencies[typeof (ICBaseType3)];
      Assert.IsNull (d2.GetImplementer ());

      Assert.IsTrue (d1.IsAggregateOnly);
      Assert.IsTrue (d2.IsAggregateOnly);

      Assert.Fail ("TODO: AggregatedDependencies");
    }
  }
}
