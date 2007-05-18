using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mixins.Definitions;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class MemberDefinitionBuilderTests
  {
    private static ApplicationDefinition GetApplicationDefinition ()
    {
      return DefBuilder.Build ();
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
      Assert.IsFalse (baseClass.Methods.HasItem (mixinMethod1));

      MemberDefinition member = baseClass.Methods[baseMethod1];

      Assert.IsTrue (new List<MemberDefinition> (baseClass.GetAllMembers ()).Contains (member));
      Assert.IsFalse (new List<MemberDefinition> (baseClass.Mixins[typeof (BT1Mixin1)].GetAllMembers ()).Contains (member));

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (baseClass, member.DeclaringClass);
      Assert.AreSame (baseClass, member.Parent);

      Assert.IsTrue (baseClass.Methods.HasItem (baseMethod2));
      Assert.AreNotSame (member, baseClass.Methods[baseMethod2]);

      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsFalse (mixin1.Methods.HasItem (baseMethod1));
      Assert.IsTrue (mixin1.Methods.HasItem (mixinMethod1));
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
      PropertyInfo indexedProperty1 = typeof (BaseType1).GetProperty ("Item", new Type[] { typeof (int) });
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
  }
}
