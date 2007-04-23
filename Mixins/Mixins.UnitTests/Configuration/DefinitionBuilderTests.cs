using System;
using System.Collections.Generic;
using Mixins.Definitions.Building;
using NUnit.Framework;
using Mixins.Context;
using System.Reflection;
using Mixins.Definitions;
using Mixins.UnitTests.SampleTypes;
using System.Runtime.Serialization;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class DefinitionBuilderTests
  {
    private class MixinWithCustomInitializationMethod
    {
      [MixinInitializationMethod]
      public void Init([This]object @this)
      {
      }
    }

    private class MixinImplementingISerializable : ISerializable, IDisposable
    {
      public void Dispose ()
      {
        throw new Exception ("The method or operation is not implemented.");
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        throw new Exception ("The method or operation is not implemented.");
      }
    }

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
    public void IntroducedMembers ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
      InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];

      Assert.IsTrue (introducedInterface.IntroducedMembers.HasItem (typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")));
      Assert.IsTrue (introducedInterface.IntroducedMembers.HasItem (typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")));
      Assert.IsTrue (introducedInterface.IntroducedMembers.HasItem (typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")));

      MemberIntroductionDefinition method = introducedInterface.IntroducedMembers[typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")];
      Assert.AreNotEqual (mixin1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")], method);
      Assert.AreSame (mixin1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")], method.ImplementingMember);
      Assert.AreSame (mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)], method.DeclaringInterface);

      MemberIntroductionDefinition property = introducedInterface.IntroducedMembers[typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")];
      Assert.AreNotEqual (mixin1.Properties[typeof (BT1Mixin1).GetProperty ("IntroducedProperty")], property);
      Assert.AreSame (mixin1.Properties[typeof (BT1Mixin1).GetProperty ("IntroducedProperty")], property.ImplementingMember);
      Assert.AreSame (mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)], property.DeclaringInterface);

      MemberIntroductionDefinition eventDefinition = introducedInterface.IntroducedMembers[typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")];
      Assert.AreNotEqual (mixin1.Events[typeof (BT1Mixin1).GetEvent ("IntroducedEvent")], eventDefinition);
      Assert.AreSame (mixin1.Events[typeof (BT1Mixin1).GetEvent ("IntroducedEvent")], eventDefinition.ImplementingMember);
      Assert.AreSame (mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)], eventDefinition.DeclaringInterface);
    }

    [Test]
    public void ISerializableIsNotIntroduced ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (MixinImplementingISerializable));
      Assert.IsNull (application.BaseClasses[typeof (BaseType1)].Mixins[typeof (MixinImplementingISerializable)].InterfaceIntroductions[typeof (ISerializable)]);
      Assert.IsNotNull (application.BaseClasses[typeof (BaseType1)].Mixins[typeof (MixinImplementingISerializable)].InterfaceIntroductions[typeof (IDisposable)]);
    }

    [Test]
    public void Methods ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      Assert.IsTrue (baseClass.Methods.HasItem (baseMethod1));
      Assert.IsFalse (baseClass.Methods.HasItem(mixinMethod1));
      
      MemberDefinition member = baseClass.Methods[baseMethod1];

      Assert.IsTrue (new List<MemberDefinition> (baseClass.GetAllMembers ()).Contains (member));
      Assert.IsFalse (new List<MemberDefinition> (baseClass.Mixins[typeof(BT1Mixin1)].GetAllMembers ()).Contains (member));
      
      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (baseClass, member.DeclaringClass);

      Assert.IsTrue (baseClass.Methods.HasItem (baseMethod2));
      Assert.AreNotSame (member, baseClass.Methods[baseMethod2]);

      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsFalse (mixin1.Methods.HasItem (baseMethod1));
      Assert.IsTrue(mixin1.Methods.HasItem(mixinMethod1));
      member = mixin1.Methods[mixinMethod1];

      Assert.IsTrue (new List<MemberDefinition> (mixin1.GetAllMembers ()).Contains (member));

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (mixin1, member.DeclaringClass);
    }

    [Test]
    public void Properties ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];

      PropertyInfo baseProperty = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo indexedProperty1 = typeof (BaseType1).GetProperty ("Item", new Type[] {typeof (int) });
      PropertyInfo indexedProperty2 = typeof (BaseType1).GetProperty ("Item", new Type[] { typeof (string) });
      PropertyInfo mixinProperty = typeof (BT1Mixin1).GetProperty ("VirtualProperty", new Type[0]);

      Assert.IsTrue (baseClass.Properties.HasItem (baseProperty));
      Assert.IsTrue (baseClass.Properties.HasItem (indexedProperty1));
      Assert.IsTrue (baseClass.Properties.HasItem (indexedProperty2));
      Assert.IsFalse (baseClass.Properties.HasItem (mixinProperty));

      PropertyDefinition member = baseClass.Properties[baseProperty];

      Assert.IsTrue (new List<MemberDefinition> (baseClass.GetAllMembers ()).Contains (member));
      Assert.IsFalse (new List<MemberDefinition> (baseClass.Mixins[typeof (BT1Mixin1)].GetAllMembers ()).Contains (member));

      Assert.AreEqual ("VirtualProperty", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualProperty", member.FullName);
      Assert.IsTrue (member.IsProperty);
      Assert.IsFalse (member.IsMethod);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (baseClass, member.DeclaringClass);
      Assert.IsNotNull (member.GetMethod);
      Assert.IsNotNull (member.SetMethod);

      Assert.IsFalse (baseClass.Methods.HasItem (member.GetMethod.MethodInfo));
      Assert.IsFalse (baseClass.Methods.HasItem (member.SetMethod.MethodInfo));

      member = baseClass.Properties[indexedProperty1];
      Assert.AreNotSame (member, baseClass.Properties[indexedProperty2]);

      Assert.IsNotNull (member.GetMethod);
      Assert.IsNull (member.SetMethod);

      member = baseClass.Properties[indexedProperty2];

      Assert.IsNull (member.GetMethod);
      Assert.IsNotNull (member.SetMethod);

      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsFalse (mixin1.Properties.HasItem (baseProperty));
      Assert.IsTrue (mixin1.Properties.HasItem (mixinProperty));
      
      member = mixin1.Properties[mixinProperty];

      Assert.IsTrue (new List<MemberDefinition> (mixin1.GetAllMembers ()).Contains (member));

      Assert.AreEqual ("VirtualProperty", member.Name);
      Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualProperty", member.FullName);
      Assert.IsTrue (member.IsProperty);
      Assert.IsFalse (member.IsMethod);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (mixin1, member.DeclaringClass);

      Assert.IsNull (member.GetMethod);
      Assert.IsNotNull (member.SetMethod);
    }

    [Test]
    public void Events ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];

      EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
      EventInfo mixinEvent = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

      Assert.IsTrue (baseClass.Events.HasItem (baseEvent1));
      Assert.IsTrue (baseClass.Events.HasItem (baseEvent2));
      Assert.IsFalse (baseClass.Events.HasItem (mixinEvent));

      EventDefinition member = baseClass.Events[baseEvent1];

      Assert.IsTrue (new List<MemberDefinition> (baseClass.GetAllMembers ()).Contains (member));
      Assert.IsFalse (new List<MemberDefinition> (baseClass.Mixins[typeof (BT1Mixin1)].GetAllMembers ()).Contains (member));

      Assert.AreEqual ("VirtualEvent", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualEvent", member.FullName);
      Assert.IsTrue (member.IsEvent);
      Assert.IsFalse (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.AreSame (baseClass, member.DeclaringClass);
      Assert.IsNotNull (member.AddMethod);
      Assert.IsNotNull (member.RemoveMethod);

      Assert.IsFalse (baseClass.Methods.HasItem (member.AddMethod.MethodInfo));
      Assert.IsFalse (baseClass.Methods.HasItem (member.RemoveMethod.MethodInfo));

      member = baseClass.Events[baseEvent2];
      Assert.IsNotNull (member.AddMethod);
      Assert.IsNotNull (member.RemoveMethod);

      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsFalse (mixin1.Events.HasItem (baseEvent1));
      Assert.IsTrue (mixin1.Events.HasItem (mixinEvent));

      member = mixin1.Events[mixinEvent];

      Assert.IsTrue (new List<MemberDefinition> (mixin1.GetAllMembers ()).Contains (member));

      Assert.AreEqual ("VirtualEvent", member.Name);
      Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualEvent", member.FullName);
      Assert.IsTrue (member.IsEvent);
      Assert.IsFalse (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.AreSame (mixin1, member.DeclaringClass);

      Assert.IsNotNull (member.AddMethod);
      Assert.IsNotNull (member.RemoveMethod);
    }

    [Test]
    public void MethodOverrides()
    {
      ApplicationDefinition application = GetApplicationDefinition();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] {typeof (string)});
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      MethodDefinition overridden = baseClass.Methods[baseMethod1];

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin1)));
      MemberDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Methods[mixinMethod1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);

      MethodDefinition notOverridden = baseClass.Methods[baseMethod2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
    }

    [Test]
    public void PropertyOverrides()
    {
      ApplicationDefinition application = GetApplicationDefinition();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

      PropertyInfo baseProperty1 = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo baseProperty2 = typeof (BaseType1).GetProperty ("Item", new Type[] {typeof (string)});
      PropertyInfo mixinProperty1 = typeof (BT1Mixin1).GetProperty ("VirtualProperty");

      PropertyDefinition overridden = baseClass.Properties[baseProperty1];

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin1)));

      PropertyDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Properties[mixinProperty1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.SetMethod, overrider.SetMethod.Base);

      PropertyDefinition notOverridden = baseClass.Properties[baseProperty2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.GetMethod, overrider.GetMethod.Base);
    }

    [Test]
    public void EventOverrides ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

      EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
      EventInfo mixinEvent1 = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

      EventDefinition overridden = baseClass.Events[baseEvent1];

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin1)));

      EventDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Events[mixinEvent1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.RemoveMethod, overrider.RemoveMethod.Base);
      Assert.AreSame (overridden.AddMethod, overrider.AddMethod.Base);

      EventDefinition notOverridden = baseClass.Events[baseEvent2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides ()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.AddMethod, overrider.AddMethod.Base);
      Assert.AreSame (overridden.RemoveMethod, overrider.RemoveMethod.Base);
    }

    [Test]
    public void OverrideNonVirtualMethod ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType4), typeof (BT4Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType4)];
      MixinDefinition mixin = baseClass.Mixins[typeof(BT4Mixin1)];
      Assert.IsNotNull (mixin);

      MemberDefinition overrider = mixin.Methods[typeof(BT4Mixin1).GetMethod("NonVirtualMethod")];
      Assert.IsNotNull(overrider);
      Assert.IsNotNull(overrider.Base);

      Assert.AreSame(baseClass, overrider.Base.DeclaringClass);

      List<MethodDefinition> overrides = new List<MethodDefinition> (baseClass.Methods[typeof (BaseType4).GetMethod ("NonVirtualMethod")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    public void OverrideNonVirtualProperty ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType4), typeof (BT4Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType4)];
      MixinDefinition mixin = baseClass.Mixins[typeof (BT4Mixin1)];
      Assert.IsNotNull (mixin);

      PropertyDefinition overrider = mixin.Properties[typeof (BT4Mixin1).GetProperty ("NonVirtualProperty")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (baseClass, overrider.Base.DeclaringClass);

      List<PropertyDefinition> overrides = new List<PropertyDefinition> (baseClass.Properties[typeof (BaseType4).GetProperty ("NonVirtualProperty")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    public void OverrideNonVirtualEvent ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType4), typeof (BT4Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType4)];
      MixinDefinition mixin = baseClass.Mixins[typeof (BT4Mixin1)];
      Assert.IsNotNull (mixin);

      EventDefinition overrider = mixin.Events[typeof (BT4Mixin1).GetEvent ("NonVirtualEvent")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (baseClass, overrider.Base.DeclaringClass);

      List<EventDefinition> overrides = new List<EventDefinition> (baseClass.Events[typeof (BaseType4).GetEvent ("NonVirtualEvent")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    [ExpectedException(typeof( ConfigurationException), ExpectedMessage = "Could not find base member for overrider Mixins.UnitTests.SampleTypes.BT5Mixin1.Method.")]
    public void ThrowsWhenInexistingOverrideBaseMethod ()
    {
      DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin1));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Could not find base member for overrider Mixins.UnitTests.SampleTypes.BT5Mixin4.Property.")]
    public void ThrowsWhenInexistingOverrideBaseProperty ()
    {
      DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin4));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Could not find base member for overrider Mixins.UnitTests.SampleTypes.BT5Mixin5.Event.")]
    public void ThrowsWhenInexistingOverrideBaseEvent ()
    {
      DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin5));
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

      MemberDefinition member = baseClass.Methods[method];
      Assert.IsNotNull (member);

      MixinDefinition mixin = baseClass.Mixins[typeof (BT2Mixin1)];
      Assert.IsNotNull (mixin);
      Assert.AreSame (baseClass, mixin.BaseClass);
    }

    [Test]
    public void InitializationMethod ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin = baseClass.Mixins[typeof (BT1Mixin1)];
      Assert.AreEqual (0, mixin.InitializationMethods.Count);

      baseClass = application.BaseClasses[typeof (BaseType3)];
      mixin = baseClass.Mixins[typeof (BT3Mixin1)];
      
      Assert.AreEqual (1, mixin.InitializationMethods.Count);

      MethodInfo methodInfo = typeof (BT3Mixin1).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (methodInfo);

      List<MethodDefinition> initializationMethods = new List<MethodDefinition> (mixin.InitializationMethods);
      Assert.AreEqual (methodInfo, initializationMethods[0].MethodInfo);

      BaseType3 @this = new BaseType3();
      BaseType3 @base = new BaseType3();
      BT3Mixin1 mixinInstance = new BT3Mixin1 ();
      initializationMethods[0].MethodInfo.Invoke (mixinInstance, new object[] { @this, @base });
      Assert.AreSame (@this, mixinInstance.This);
      Assert.AreSame (@base, mixinInstance.Base);
    }

    [Test]
    public void CustomInitializationMethod ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (MixinWithCustomInitializationMethod));
      MixinDefinition mixin = application.BaseClasses[typeof (BaseType1)].Mixins[typeof (MixinWithCustomInitializationMethod)];
      Assert.IsNotNull (mixin);
      Assert.AreEqual (1, mixin.InitializationMethods.Count);
      Assert.IsTrue (mixin.InitializationMethods.HasItem (typeof (MixinWithCustomInitializationMethod).GetMethod ("Init")));
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

      List<MixinDefinition> requirers = new List<MixinDefinition> (baseClass.RequiredFaceTypes[typeof (IBaseType31)].FindRequiringMixins());
      Assert.Contains (baseClass.Mixins[typeof (BT3Mixin1)], requirers);
      Assert.Contains (baseClass.Mixins[typeof (BT3Mixin6<,>)], requirers);
      Assert.AreEqual (2, requirers.Count);

      Assert.IsFalse (baseClass.RequiredFaceTypes[typeof (IBaseType31)].IsEmptyInterface);
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

      List<MixinDefinition> requirers = new List<MixinDefinition> (baseClass.RequiredBaseCallTypes[typeof (IBaseType33)].FindRequiringMixins());
      Assert.Contains (baseClass.Mixins[typeof (BT3Mixin3<,>)], requirers);
      Assert.AreEqual (1, requirers.Count);
    }

    [Test]
    public void Dependencies ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      MixinDefinition bt3Mixin1 = application.BaseClasses[typeof (BaseType3)].Mixins[typeof(BT3Mixin1)];
      
      Assert.IsTrue (bt3Mixin1.ThisDependencies.HasItem (typeof (IBaseType31)));
      Assert.AreEqual (1, bt3Mixin1.ThisDependencies.Count);

      Assert.IsTrue (bt3Mixin1.BaseDependencies.HasItem (typeof (IBaseType31)));
      Assert.AreEqual (1, bt3Mixin1.BaseDependencies.Count);

      MixinDefinition bt3Mixin2 = application.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin2)];
      Assert.IsTrue (bt3Mixin2.ThisDependencies.HasItem (typeof (IBaseType32)));
      Assert.AreEqual (1, bt3Mixin2.ThisDependencies.Count);

      Assert.AreEqual (0, bt3Mixin2.BaseDependencies.Count);

      MixinDefinition bt3Mixin6 = application.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin6<,>)];
      
      Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType31)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType32)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType33)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBT3Mixin4)));
      Assert.IsFalse (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType34)));

      Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBaseType31)].IsAggregate);
      Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].IsAggregate);

      Assert.AreEqual (0, bt3Mixin6.ThisDependencies[typeof (IBaseType31)].AggregatedDependencies.Count);

      Assert.IsTrue (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.HasItem (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)]));
      Assert.IsNull (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].Aggregator);

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

      Assert.IsFalse (bt3Mixin6.BaseDependencies[typeof(IBT3Mixin4)].IsAggregate);
      Assert.IsFalse (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].IsAggregate);

      Assert.AreEqual (0, bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].AggregatedDependencies.Count);

      Assert.IsTrue (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.HasItem (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)]));
      Assert.IsNull (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].Aggregator);
    }

    [Test]
    public void CompleteInterfacesAndDependencies ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7));
      BaseClassDefinition bt3 = application.BaseClasses[typeof (BaseType3)];
      
      MixinDefinition m4 = bt3.Mixins[typeof (BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof (BT3Mixin7)];

      ThisDependencyDefinition d1 = m7.ThisDependencies[typeof (ICBaseType3BT3Mixin4)];
      Assert.IsNull (d1.GetImplementer());

      BaseDependencyDefinition d2 = m7.BaseDependencies[typeof (ICBaseType3BT3Mixin4)];
      Assert.IsNull (d2.GetImplementer ());

      Assert.IsTrue (d1.IsAggregate);
      Assert.IsTrue (d2.IsAggregate);

      Assert.IsTrue (d1.AggregatedDependencies[typeof(ICBaseType3)].IsAggregate);
      Assert.IsFalse (d1.AggregatedDependencies[typeof (ICBaseType3)]
                          .AggregatedDependencies[typeof(IBaseType31)].IsAggregate);
      Assert.AreSame (bt3, d1.AggregatedDependencies[typeof (ICBaseType3)]
                               .AggregatedDependencies[typeof (IBaseType31)].GetImplementer());

      Assert.IsFalse (d1.AggregatedDependencies[typeof (IBT3Mixin4)].IsAggregate);
      Assert.AreSame (m4, d1.AggregatedDependencies[typeof (IBT3Mixin4)].GetImplementer());

      Assert.AreSame (d1, d1.AggregatedDependencies[typeof (IBT3Mixin4)].Aggregator);

      Assert.IsTrue (d2.AggregatedDependencies[typeof (ICBaseType3)].IsAggregate);
      Assert.IsFalse (d2.AggregatedDependencies[typeof (ICBaseType3)]
                          .AggregatedDependencies[typeof (IBaseType31)].IsAggregate);
      Assert.AreSame (bt3, d2.AggregatedDependencies[typeof (ICBaseType3)]
                               .AggregatedDependencies[typeof (IBaseType31)].GetImplementer ());

      Assert.IsFalse (d2.AggregatedDependencies[typeof (IBT3Mixin4)].IsAggregate);
      Assert.AreSame (m4, d2.AggregatedDependencies[typeof (IBT3Mixin4)].GetImplementer ());

      Assert.AreSame (d2, d2.AggregatedDependencies[typeof (IBT3Mixin4)].Aggregator);

      Assert.IsTrue (bt3.RequiredFaceTypes[typeof (ICBaseType3)].IsEmptyInterface);

      Assert.IsTrue (bt3.RequiredFaceTypes.HasItem (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredFaceTypes.HasItem (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredFaceTypes.HasItem (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredFaceTypes.HasItem (typeof (IBT3Mixin4)));

      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (IBT3Mixin4)));
    }

    [Test]
    public void MixinIndicesCorrespondToPositionInArray()
    {
      BaseClassDefinition bt3 = GetApplicationDefinition ().BaseClasses[typeof (BaseType3)];
      for (int i = 0; i< bt3.Mixins.Count; ++i)
      {
        Assert.AreEqual (i, bt3.Mixins[i].MixinIndex);
      }
    }
  }
}
