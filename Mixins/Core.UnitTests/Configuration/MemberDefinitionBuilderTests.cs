using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class MemberDefinitionBuilderTests
  {
    [Test]
    public void Methods ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));

        MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
        MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] {typeof (string)});
        MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

        Assert.IsTrue (baseClass.Methods.ContainsKey (baseMethod1));
        Assert.IsFalse (baseClass.Methods.ContainsKey (mixinMethod1));

        MemberDefinition member = baseClass.Methods[baseMethod1];

        Assert.IsTrue (new List<MemberDefinition> (baseClass.GetAllMembers()).Contains (member));
        Assert.IsFalse (new List<MemberDefinition> (baseClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member));

        Assert.AreEqual ("VirtualMethod", member.Name);
        Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
        Assert.IsTrue (member.IsMethod);
        Assert.IsFalse (member.IsProperty);
        Assert.IsFalse (member.IsEvent);
        Assert.AreSame (baseClass, member.DeclaringClass);
        Assert.AreSame (baseClass, member.Parent);

        Assert.IsTrue (baseClass.Methods.ContainsKey (baseMethod2));
        Assert.AreNotSame (member, baseClass.Methods[baseMethod2]);

        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

        Assert.IsFalse (mixin1.Methods.ContainsKey (baseMethod1));
        Assert.IsTrue (mixin1.Methods.ContainsKey (mixinMethod1));
        member = mixin1.Methods[mixinMethod1];

        Assert.IsTrue (new List<MemberDefinition> (mixin1.GetAllMembers()).Contains (member));

        Assert.AreEqual ("VirtualMethod", member.Name);
        Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualMethod", member.FullName);
        Assert.IsTrue (member.IsMethod);
        Assert.IsFalse (member.IsProperty);
        Assert.IsFalse (member.IsEvent);
        Assert.AreSame (mixin1, member.DeclaringClass);
      }
    }

    [Test]
    public void Properties ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));

        PropertyInfo baseProperty = typeof (BaseType1).GetProperty ("VirtualProperty");
        PropertyInfo indexedProperty1 = typeof (BaseType1).GetProperty ("Item", new Type[] {typeof (int)});
        PropertyInfo indexedProperty2 = typeof (BaseType1).GetProperty ("Item", new Type[] {typeof (string)});
        PropertyInfo mixinProperty = typeof (BT1Mixin1).GetProperty ("VirtualProperty", new Type[0]);

        Assert.IsTrue (baseClass.Properties.ContainsKey (baseProperty));
        Assert.IsTrue (baseClass.Properties.ContainsKey (indexedProperty1));
        Assert.IsTrue (baseClass.Properties.ContainsKey (indexedProperty2));
        Assert.IsFalse (baseClass.Properties.ContainsKey (mixinProperty));

        PropertyDefinition member = baseClass.Properties[baseProperty];

        Assert.IsTrue (new List<MemberDefinition> (baseClass.GetAllMembers()).Contains (member));
        Assert.IsFalse (new List<MemberDefinition> (baseClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member));

        Assert.AreEqual ("VirtualProperty", member.Name);
        Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualProperty", member.FullName);
        Assert.IsTrue (member.IsProperty);
        Assert.IsFalse (member.IsMethod);
        Assert.IsFalse (member.IsEvent);
        Assert.AreSame (baseClass, member.DeclaringClass);
        Assert.IsNotNull (member.GetMethod);
        Assert.IsNotNull (member.SetMethod);

        Assert.IsFalse (baseClass.Methods.ContainsKey (member.GetMethod.MethodInfo));
        Assert.IsFalse (baseClass.Methods.ContainsKey (member.SetMethod.MethodInfo));

        member = baseClass.Properties[indexedProperty1];
        Assert.AreNotSame (member, baseClass.Properties[indexedProperty2]);

        Assert.IsNotNull (member.GetMethod);
        Assert.IsNull (member.SetMethod);

        member = baseClass.Properties[indexedProperty2];

        Assert.IsNull (member.GetMethod);
        Assert.IsNotNull (member.SetMethod);

        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

        Assert.IsFalse (mixin1.Properties.ContainsKey (baseProperty));
        Assert.IsTrue (mixin1.Properties.ContainsKey (mixinProperty));

        member = mixin1.Properties[mixinProperty];

        Assert.IsTrue (new List<MemberDefinition> (mixin1.GetAllMembers()).Contains (member));

        Assert.AreEqual ("VirtualProperty", member.Name);
        Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualProperty", member.FullName);
        Assert.IsTrue (member.IsProperty);
        Assert.IsFalse (member.IsMethod);
        Assert.IsFalse (member.IsEvent);
        Assert.AreSame (mixin1, member.DeclaringClass);

        Assert.IsNull (member.GetMethod);
        Assert.IsNotNull (member.SetMethod);
      }
    }

    [Test]
    public void Events ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));

        EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
        EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
        EventInfo mixinEvent = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

        Assert.IsTrue (baseClass.Events.ContainsKey (baseEvent1));
        Assert.IsTrue (baseClass.Events.ContainsKey (baseEvent2));
        Assert.IsFalse (baseClass.Events.ContainsKey (mixinEvent));

        EventDefinition member = baseClass.Events[baseEvent1];

        Assert.IsTrue (new List<MemberDefinition> (baseClass.GetAllMembers()).Contains (member));
        Assert.IsFalse (new List<MemberDefinition> (baseClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member));

        Assert.AreEqual ("VirtualEvent", member.Name);
        Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualEvent", member.FullName);
        Assert.IsTrue (member.IsEvent);
        Assert.IsFalse (member.IsMethod);
        Assert.IsFalse (member.IsProperty);
        Assert.AreSame (baseClass, member.DeclaringClass);
        Assert.IsNotNull (member.AddMethod);
        Assert.IsNotNull (member.RemoveMethod);

        Assert.IsFalse (baseClass.Methods.ContainsKey (member.AddMethod.MethodInfo));
        Assert.IsFalse (baseClass.Methods.ContainsKey (member.RemoveMethod.MethodInfo));

        member = baseClass.Events[baseEvent2];
        Assert.IsNotNull (member.AddMethod);
        Assert.IsNotNull (member.RemoveMethod);

        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

        Assert.IsFalse (mixin1.Events.ContainsKey (baseEvent1));
        Assert.IsTrue (mixin1.Events.ContainsKey (mixinEvent));

        member = mixin1.Events[mixinEvent];

        Assert.IsTrue (new List<MemberDefinition> (mixin1.GetAllMembers()).Contains (member));

        Assert.AreEqual ("VirtualEvent", member.Name);
        Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualEvent", member.FullName);
        Assert.IsTrue (member.IsEvent);
        Assert.IsFalse (member.IsMethod);
        Assert.IsFalse (member.IsProperty);
        Assert.AreSame (mixin1, member.DeclaringClass);

        Assert.IsNotNull (member.AddMethod);
        Assert.IsNotNull (member.RemoveMethod);
      }
    }

    class Base<T>
    {
      public virtual void Method(T t)
      {
      }

      public virtual T Property
      {
        get { return default(T);}
        set { }
      }

      public virtual event Func<T> Event;
    }

    class Derived : Base<int>
    {
      public virtual new void Method (int t)
      {
      }

      public virtual new int Property
      {
        get { return default (int); }
        set { }
      }

      public virtual new event Func<int> Event;
    }

    class ExtraDerived : Derived
    {
      public virtual new void Method (int t)
      {
      }

      public virtual new int Property
      {
        get { return default (int); }
        set { }
      }

      public virtual new event Func<int> Event;
    }

    class DerivedWithOverrides : ExtraDerived
    {
      public override void Method (int t)
      {
      }

      public override int Property
      {
        get { return default (int); }
        set { }
      }

      public override event Func<int> Event;
    }

    class ExtraExtraDerived : DerivedWithOverrides
    {
      public new void Method (int t)
      {
      }

      public new int Property
      {
        get { return default (int); }
        set { }
      }

      public new event Func<int> Event;
    }

    [Test]
    public void ShadowedMembersExplicitlyRetrievedButOverriddenNot()
    {
      BaseClassDefinition d = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ExtraExtraDerived));
      BindingFlags bf = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

      Assert.IsTrue (d.Methods.ContainsKey (typeof (ExtraExtraDerived).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (DerivedWithOverrides).GetMethod ("Method", bf)));
      Assert.IsFalse (d.Methods.ContainsKey (typeof (ExtraDerived).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (Derived).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (Base<int>).GetMethod ("Method", bf)));

      Assert.IsTrue (d.Properties.ContainsKey (typeof (ExtraExtraDerived).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (DerivedWithOverrides).GetProperty ("Property", bf)));
      Assert.IsFalse (d.Properties.ContainsKey (typeof (ExtraDerived).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (Derived).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (Base<int>).GetProperty ("Property", bf)));

      Assert.IsTrue (d.Events.ContainsKey (typeof (ExtraExtraDerived).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (DerivedWithOverrides).GetEvent ("Event", bf)));
      Assert.IsFalse (d.Events.ContainsKey (typeof (ExtraDerived).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (Derived).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (Base<int>).GetEvent ("Event", bf)));

      Assert.AreEqual (18, new List<MemberDefinition> (d.GetAllMembers ()).Count);
    }

    [Test]
    public void ShadowedMixinMembersExplicitlyRetrieved()
    {
      MixinDefinition d = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin2)).Mixins[typeof (BT3Mixin2)];

      Assert.IsTrue (d.Properties.ContainsKey (typeof (BT3Mixin2).GetProperty ("This")));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.AreEqual (10, new List<MemberDefinition> (d.GetAllMembers ()).Count);
    }

    [Test]
    public void ProtectedInternalMembers ()
    {
      BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (ClassWithInheritedMethod));
      Assert.IsTrue (baseClass.Methods.ContainsKey (typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod",
          BindingFlags.Instance | BindingFlags.NonPublic)));
    }
  }
}
