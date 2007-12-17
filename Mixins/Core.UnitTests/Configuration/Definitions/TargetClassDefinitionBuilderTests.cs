using System;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using System.Reflection;

namespace Rubicon.Mixins.UnitTests.Configuration.Definitions
{
  [TestFixture]
  public class TargetClassDefinitionBuilderTests
  {
    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "contains generic parameters", MatchType = MessageMatch.Contains)]
    public void ThrowsOnGenericTargetClass()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BT3Mixin3<,>)).Clear().EnterScope())
      {
        TargetClassDefinitionBuilder builder = new TargetClassDefinitionBuilder();
        builder.Build (new ClassContext (typeof (BT3Mixin3<,>)));
      }
    }

    [Test]
    public void TargetClassDefinitionKnowsItsContext()
    {
      ClassContext classContext = new ClassContext (typeof (BaseType1));
      MixinConfiguration configuration = new MixinConfiguration (null);
      configuration.AddClassContext (classContext);

      using (configuration.EnterScope())
      {
        TargetClassDefinition def = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsNotNull (def.ConfigurationContext);
        Assert.AreSame (classContext, def.ConfigurationContext);
      }
    }

    public interface IInterfaceWithAllMembers
    {
      void Method ();
      string Property { get; set; }
      event EventHandler Event;
    }

    public class ClassWithExplicitInterfaceImplementation : IInterfaceWithAllMembers
    {
      void IInterfaceWithAllMembers.Method ()
      {
        throw new Exception ("The method or operation is not implemented.");
      }

      string IInterfaceWithAllMembers.Property
      {
        get { throw new Exception ("The method or operation is not implemented."); }
        set { throw new Exception ("The method or operation is not implemented."); }
      }

      event EventHandler IInterfaceWithAllMembers.Event
      {
        add { throw new Exception ("The method or operation is not implemented."); }
        remove { throw new Exception ("The method or operation is not implemented."); }
      }
    }

    [Test]
    public void TargetClassHasExplicitInterfaceMembers ()
    {
      TargetClassDefinition cweii = TypeFactory.GetActiveConfiguration (typeof (ClassWithExplicitInterfaceImplementation),
          GenerationPolicy.ForceGeneration);
      Assert.AreEqual (7, cweii.Methods.Count);
      Assert.AreEqual (1, cweii.Properties.Count);
      Assert.AreEqual (1, cweii.Events.Count);

      BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      Assert.IsTrue (cweii.Methods.ContainsKey (typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
          "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.Method", bf)));

      Assert.IsTrue (
          cweii.Properties.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.Property", bf)));
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.get_Property", bf),
          cweii.Properties[
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.Property", bf)].GetMethod.
              MemberInfo);
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.set_Property", bf),
          cweii.Properties[
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.Property", bf)].SetMethod.
              MemberInfo);

      Assert.IsTrue (
          cweii.Events.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.Event", bf)));
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.add_Event", bf),
          cweii.Events[
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.Event", bf)].AddMethod.MemberInfo);
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.remove_Event", bf),
          cweii.Events[
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Rubicon.Mixins.UnitTests.Configuration.Definitions.TargetClassDefinitionBuilderTests.IInterfaceWithAllMembers.Event", bf)].RemoveMethod.
              MemberInfo);
      
    }

    [Test]
    public void HasOverriddenMembersTrue ()
    {
      TargetClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsTrue (bt1.HasOverriddenMembers ());
    }

    [Test]
    public void HasOverriddenMembersFalse ()
    {
      TargetClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMembersProtected),
          typeof (MixinWithAbstractMembers));
      Assert.IsFalse (bt1.HasOverriddenMembers ());
    }

    [Test]
    public void HasProtectedOverridersTrue ()
    {
      TargetClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMembersProtected),
          typeof (MixinWithAbstractMembers));
      Assert.IsTrue (bt1.HasProtectedOverriders ());
    }

    [Test]
    public void HasProtectedOverridersFalse ()
    {
      TargetClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsFalse (bt1.HasProtectedOverriders ());
    }

    [Test]
    public void IsAbstractTrue ()
    {
      TargetClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (AbstractBaseType), GenerationPolicy.ForceGeneration);
      Assert.IsTrue (bt1.IsAbstract);
    }

    [Test]
    public void IsAbstractFalse ()
    {
      TargetClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsFalse (bt1.IsAbstract);
    }
  }
}