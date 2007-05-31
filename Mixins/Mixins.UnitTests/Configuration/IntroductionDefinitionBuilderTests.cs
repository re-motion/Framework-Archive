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

    [Test]
    public void IntroducedInterface ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsTrue (mixin1.InterfaceIntroductions.HasItem (typeof (IBT1Mixin1)));
      InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];
      Assert.AreSame (mixin1, introducedInterface.Parent);
      Assert.AreSame (mixin1, introducedInterface.Implementer);
      Assert.IsTrue (baseClass.IntroducedInterfaces.HasItem (typeof (IBT1Mixin1)));
      Assert.AreSame (baseClass.IntroducedInterfaces[typeof (IBT1Mixin1)], introducedInterface);
      Assert.AreSame (baseClass, introducedInterface.BaseClass);
    }

    private ApplicationDefinition GetApplicationDefinition()
    {
      ApplicationContext assemblyContext = ApplicationContextBuilder.BuildFromAssemblies (Assembly.GetExecutingAssembly ());
      return DefinitionBuilder.CreateApplicationDefinition (assemblyContext);
    }

    [Test]
    public void IntroducedMembers ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
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

    [Test]
    public void MixinCanImplementMethodsExplicitly ()
    {
      ApplicationDefinition configuration = DefBuilder.Build (typeof (BaseType1), typeof (MixinWithExplicitImplementation));
      BaseClassDefinition bt1 = configuration.BaseClasses[typeof (BaseType1)];
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IExplicit)));

      MethodInfo explicitMethod = typeof (MixinWithExplicitImplementation).GetMethod (
          "Mixins.UnitTests.SampleTypes.IExplicit.Explicit", BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.IsNotNull (explicitMethod);

      MixinDefinition m1 = bt1.Mixins[typeof (MixinWithExplicitImplementation)];
      Assert.IsTrue (m1.Methods.HasItem (explicitMethod));
    }

    [Test]
    public void ISerializableIsNotIntroduced ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (MixinImplementingISerializable));
      Assert.IsNull (application.BaseClasses[typeof (BaseType1)].Mixins[typeof (MixinImplementingISerializable)].InterfaceIntroductions[typeof (ISerializable)]);
      Assert.IsNotNull (application.BaseClasses[typeof (BaseType1)].Mixins[typeof (MixinImplementingISerializable)].InterfaceIntroductions[typeof (IDisposable)]);
    }
  }
}
