using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class IntroductionDefinitionBuilderTests
  {
    public class MixinImplementingISerializable : ISerializable, IDisposable
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

    [Test]
    public void IntroducedInterface ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

        Assert.IsTrue (mixin1.InterfaceIntroductions.ContainsKey (typeof (IBT1Mixin1)));
        InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];
        Assert.AreSame (mixin1, introducedInterface.Parent);
        Assert.AreSame (mixin1, introducedInterface.Implementer);
        Assert.IsTrue (baseClass.IntroducedInterfaces.ContainsKey (typeof (IBT1Mixin1)));
        Assert.AreSame (baseClass.IntroducedInterfaces[typeof (IBT1Mixin1)], introducedInterface);
        Assert.AreSame (baseClass, introducedInterface.BaseClass);
      }
    }

    public interface IIntroducedBase
    {
      void Foo ();
      string FooP { get; set; }
      event EventHandler FooE;
    }

    public class BaseIntroducer : IIntroducedBase
    {
      public void Foo ()
      {
        throw new NotImplementedException();
      }

      public string FooP
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public event EventHandler FooE;
    }

    public interface IIntroducedDerived : IIntroducedBase { }
    public class DerivedIntroducer : BaseIntroducer, IIntroducedDerived { }

    [Test]
    public void IntroducedInterfaceOverInheritance ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType1), typeof (DerivedIntroducer)))
      {
        BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IIntroducedDerived)));
        Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IIntroducedBase)));

        Assert.AreEqual (0, bt1.IntroducedInterfaces[typeof (IIntroducedDerived)].IntroducedMethods.Count);
        Assert.AreEqual (0, bt1.IntroducedInterfaces[typeof (IIntroducedDerived)].IntroducedProperties.Count);
        Assert.AreEqual (0, bt1.IntroducedInterfaces[typeof (IIntroducedDerived)].IntroducedEvents.Count);

        Assert.AreEqual (1, bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedMethods.Count);
        Assert.AreEqual (1, bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedProperties.Count);
        Assert.AreEqual (1, bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedEvents.Count);

        Assert.AreEqual (typeof (IIntroducedBase).GetMethod ("Foo"),
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedMethods[0].InterfaceMember);
        Assert.AreEqual (typeof (IIntroducedBase).GetProperty ("FooP"),
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedProperties[0].InterfaceMember);
        Assert.AreEqual (typeof (IIntroducedBase).GetEvent ("FooE"),
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedEvents[0].InterfaceMember);

        Assert.AreEqual (bt1.Mixins[typeof (DerivedIntroducer)],
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedMethods[0].ImplementingMember.DeclaringClass);
        Assert.AreEqual (bt1.Mixins[typeof (DerivedIntroducer)],
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedProperties[0].ImplementingMember.DeclaringClass);
        Assert.AreEqual (bt1.Mixins[typeof (DerivedIntroducer)],
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedEvents[0].ImplementingMember.DeclaringClass);
      }
    }

    public class ExplicitBaseIntroducer : IIntroducedBase
    {
      void IIntroducedBase.Foo ()
      {
        throw new NotImplementedException ();
      }

      string IIntroducedBase.FooP
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      event EventHandler IIntroducedBase.FooE {
        add { throw new NotImplementedException (); }
        remove { throw new NotImplementedException (); }
      }
    }

    public class ExplicitDerivedIntroducer : ExplicitBaseIntroducer, IIntroducedDerived { }

    [Test]
    public void ExplicitlyIntroducedInterfaceOverInheritance ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType1), typeof (ExplicitDerivedIntroducer)))
      {
        BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IIntroducedDerived)));
        Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IIntroducedBase)));

        Assert.AreEqual (0, bt1.IntroducedInterfaces[typeof (IIntroducedDerived)].IntroducedMethods.Count);
        Assert.AreEqual (0, bt1.IntroducedInterfaces[typeof (IIntroducedDerived)].IntroducedProperties.Count);
        Assert.AreEqual (0, bt1.IntroducedInterfaces[typeof (IIntroducedDerived)].IntroducedEvents.Count);

        Assert.AreEqual (1, bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedMethods.Count);
        Assert.AreEqual (1, bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedProperties.Count);
        Assert.AreEqual (1, bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedEvents.Count);

        Assert.AreEqual (typeof (IIntroducedBase).GetMethod ("Foo"),
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedMethods[0].InterfaceMember);
        Assert.AreEqual (typeof (IIntroducedBase).GetProperty ("FooP"),
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedProperties[0].InterfaceMember);
        Assert.AreEqual (typeof (IIntroducedBase).GetEvent ("FooE"),
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedEvents[0].InterfaceMember);

        Assert.AreEqual (bt1.Mixins[typeof (ExplicitDerivedIntroducer)],
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedMethods[0].ImplementingMember.DeclaringClass);
        Assert.AreEqual (bt1.Mixins[typeof (ExplicitDerivedIntroducer)],
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedProperties[0].ImplementingMember.DeclaringClass);
        Assert.AreEqual (bt1.Mixins[typeof (ExplicitDerivedIntroducer)],
            bt1.IntroducedInterfaces[typeof (IIntroducedBase)].IntroducedEvents[0].ImplementingMember.DeclaringClass);
      }
    }

    [Test]
    public void IntroducedMembers ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
        InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];

        Assert.IsTrue (introducedInterface.IntroducedMethods.ContainsKey (typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")));
        Assert.IsTrue (introducedInterface.IntroducedProperties.ContainsKey (typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")));
        Assert.IsTrue (introducedInterface.IntroducedEvents.ContainsKey (typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")));

        MethodIntroductionDefinition method = introducedInterface.IntroducedMethods[typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")];
        Assert.AreNotEqual (mixin1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")], method);
        Assert.AreSame (mixin1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")], method.ImplementingMember);
        Assert.AreSame (introducedInterface, method.DeclaringInterface);
        Assert.AreSame (introducedInterface, method.Parent);

        PropertyIntroductionDefinition property = introducedInterface.IntroducedProperties[typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")];
        Assert.AreNotEqual (mixin1.Properties[typeof (BT1Mixin1).GetProperty ("IntroducedProperty")], property);
        Assert.AreSame (mixin1.Properties[typeof (BT1Mixin1).GetProperty ("IntroducedProperty")], property.ImplementingMember);
        Assert.AreSame (introducedInterface, property.DeclaringInterface);
        Assert.AreSame (introducedInterface, method.Parent);

        EventIntroductionDefinition eventDefinition = introducedInterface.IntroducedEvents[typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")];
        Assert.AreNotEqual (mixin1.Events[typeof (BT1Mixin1).GetEvent ("IntroducedEvent")], eventDefinition);
        Assert.AreSame (mixin1.Events[typeof (BT1Mixin1).GetEvent ("IntroducedEvent")], eventDefinition.ImplementingMember);
        Assert.AreSame (introducedInterface, eventDefinition.DeclaringInterface);
        Assert.AreSame (introducedInterface, method.Parent);
      }
    }

    [Test]
    public void MixinCanImplementMethodsExplicitly ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType1), typeof (MixinWithExplicitImplementation)))
      {
        BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IExplicit)));

        MethodInfo explicitMethod = typeof (MixinWithExplicitImplementation).GetMethod (
            "Rubicon.Mixins.UnitTests.SampleTypes.IExplicit.Explicit", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull (explicitMethod);

        MixinDefinition m1 = bt1.Mixins[typeof (MixinWithExplicitImplementation)];
        Assert.IsTrue (m1.Methods.ContainsKey (explicitMethod));
      }
    }

    [Test]
    public void ISerializableIsNotIntroduced ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType1), typeof (MixinImplementingISerializable)))
      {
        Assert.IsNull (TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (MixinImplementingISerializable)]
            .InterfaceIntroductions[typeof (ISerializable)]);
        Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (MixinImplementingISerializable)]
            .InterfaceIntroductions[typeof (IDisposable)]);
      }
    }

    [Test]
    public void IntroducesGetMethod()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType1), typeof (MixinImplementingFullPropertiesWithPartialIntroduction)))
      {
        InterfaceIntroductionDefinition introduction = TypeFactory.GetActiveConfiguration (typeof (BaseType1))
            .IntroducedInterfaces[typeof (InterfaceWithPartialProperties)];
        PropertyIntroductionDefinition prop1 = introduction.IntroducedProperties[typeof (InterfaceWithPartialProperties).GetProperty ("Prop1")];
        PropertyIntroductionDefinition prop2 = introduction.IntroducedProperties[typeof (InterfaceWithPartialProperties).GetProperty ("Prop2")];
        Assert.IsTrue (prop1.IntroducesGetMethod);
        Assert.IsFalse (prop1.IntroducesSetMethod);
        Assert.IsTrue (prop2.IntroducesSetMethod);
        Assert.IsFalse (prop2.IntroducesGetMethod);
      }
    }

    class BT1Mixin1A : BT1Mixin1
    { }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two mixins introduce the same interface .* to base class .*",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnDoublyIntroducedInterface ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin1A)))
      {
        TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      }
    }

    [Test]
    public void InterfaceImplementedByBaseClassCannotBeIntroduced ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassImplementingSimpleInterface), typeof (MixinImplementingSimpleInterface)))
      {
        BaseClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (ClassImplementingSimpleInterface));
        Assert.IsTrue (definition.ImplementedInterfaces.Contains (typeof (ISimpleInterface)));
        Assert.IsFalse (definition.IntroducedInterfaces.ContainsKey (typeof (ISimpleInterface)));
        Assert.IsTrue (definition.Mixins[typeof (MixinImplementingSimpleInterface)].SuppressedInterfaceIntroductions.ContainsKey (
            typeof (ISimpleInterface)));
      }
    }

    [Test]
    public void SuppressedInterfacedIsNotIntroduced ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object), typeof (MixinSuppressingSimpleInterface)))
      {
        BaseClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (object));
        Assert.IsFalse (definition.ImplementedInterfaces.Contains (typeof (ISimpleInterface)));
        Assert.IsFalse (definition.IntroducedInterfaces.ContainsKey (typeof (ISimpleInterface)));
        Assert.IsTrue (definition.Mixins[typeof (MixinSuppressingSimpleInterface)].SuppressedInterfaceIntroductions.ContainsKey (
            typeof (ISimpleInterface)));
      }
    }

    [Test]
    public void ExplicitlySuppressedInterfaceIntroduction ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object), typeof (MixinSuppressingSimpleInterface)))
      {
        BaseClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (object));
        SuppressedInterfaceIntroductionDefinition suppressedDefinition =
            definition.Mixins[typeof (MixinSuppressingSimpleInterface)].SuppressedInterfaceIntroductions[typeof (ISimpleInterface)];
        Assert.IsTrue (suppressedDefinition.IsExplicitlySuppressed);
        Assert.IsFalse (suppressedDefinition.IsShadowed);
        Assert.AreSame (typeof (ISimpleInterface), suppressedDefinition.Type);
        Assert.AreSame (definition.Mixins[typeof (MixinSuppressingSimpleInterface)], suppressedDefinition.Parent);
      }
    }

    [Test]
    public void ImplicitlySuppressedInterfaceIntroduction ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassImplementingSimpleInterface), typeof (MixinImplementingSimpleInterface)))
      {
        BaseClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (ClassImplementingSimpleInterface));
        SuppressedInterfaceIntroductionDefinition suppressedDefinition =
            definition.Mixins[typeof (MixinImplementingSimpleInterface)].SuppressedInterfaceIntroductions[typeof (ISimpleInterface)];
        Assert.IsFalse (suppressedDefinition.IsExplicitlySuppressed);
        Assert.IsTrue (suppressedDefinition.IsShadowed);
        Assert.AreSame (typeof (ISimpleInterface), suppressedDefinition.Type);
        Assert.AreSame (definition.Mixins[typeof (MixinImplementingSimpleInterface)], suppressedDefinition.Parent);
      }
    }
  }
}
