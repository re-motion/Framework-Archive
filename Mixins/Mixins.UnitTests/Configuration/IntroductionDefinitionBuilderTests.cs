using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Configuration
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
      using (new MixinConfiguration (Assembly.GetExecutingAssembly()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

        Assert.IsTrue (mixin1.InterfaceIntroductions.HasItem (typeof (IBT1Mixin1)));
        InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];
        Assert.AreSame (mixin1, introducedInterface.Parent);
        Assert.AreSame (mixin1, introducedInterface.Implementer);
        Assert.IsTrue (baseClass.IntroducedInterfaces.HasItem (typeof (IBT1Mixin1)));
        Assert.AreSame (baseClass.IntroducedInterfaces[typeof (IBT1Mixin1)], introducedInterface);
        Assert.AreSame (baseClass, introducedInterface.BaseClass);
      }
    }

    [Test]
    public void IntroducedMembers ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
        InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];

        Assert.IsTrue (introducedInterface.IntroducedMethods.HasItem (typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")));
        Assert.IsTrue (introducedInterface.IntroducedProperties.HasItem (typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")));
        Assert.IsTrue (introducedInterface.IntroducedEvents.HasItem (typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")));

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
      using (new MixinConfiguration (typeof (BaseType1), typeof (MixinWithExplicitImplementation)))
      {
        BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IExplicit)));

        MethodInfo explicitMethod = typeof (MixinWithExplicitImplementation).GetMethod (
            "Mixins.UnitTests.SampleTypes.IExplicit.Explicit", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull (explicitMethod);

        MixinDefinition m1 = bt1.Mixins[typeof (MixinWithExplicitImplementation)];
        Assert.IsTrue (m1.Methods.HasItem (explicitMethod));
      }
    }

    [Test]
    public void ISerializableIsNotIntroduced ()
    {
      using (new MixinConfiguration (typeof (BaseType1), typeof (MixinImplementingISerializable)))
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
      using (new MixinConfiguration (typeof (BaseType1), typeof (MixinImplementingFullPropertiesWithPartialIntroduction)))
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
      using (new MixinConfiguration (typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin1A)))
      {
        TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      }
    }
  }
}
