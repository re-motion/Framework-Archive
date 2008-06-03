/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class TargetClassDefinitionBuilderTest
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
      TargetClassDefinitionCache.SetCurrent (null);
      ClassContext classContext = new ClassContext (typeof (BaseType1));
      MixinConfiguration configuration = new MixinConfiguration (null);
      configuration.ClassContexts.Add (classContext);

      using (configuration.EnterScope())
      {
        TargetClassDefinition def = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
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
      TargetClassDefinition cweii = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithExplicitInterfaceImplementation),
          GenerationPolicy.ForceGeneration);
      Assert.AreEqual (7, cweii.Methods.Count);
      Assert.AreEqual (1, cweii.Properties.Count);
      Assert.AreEqual (1, cweii.Events.Count);

      BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      Assert.IsTrue (cweii.Methods.ContainsKey (typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
          "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Method", bf)));

      Assert.IsTrue (
          cweii.Properties.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Property", bf)));
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.get_Property", bf),
          cweii.Properties[
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Property", bf)].GetMethod.
              MemberInfo);
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.set_Property", bf),
          cweii.Properties[
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Property", bf)].SetMethod.
              MemberInfo);

      Assert.IsTrue (
          cweii.Events.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Event", bf)));
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.add_Event", bf),
          cweii.Events[
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Event", bf)].AddMethod.MemberInfo);
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.remove_Event", bf),
          cweii.Events[
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Event", bf)].RemoveMethod.
              MemberInfo);
      
    }

    [Test]
    public void HasOverriddenMembersTrue ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
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
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsFalse (bt1.HasProtectedOverriders ());
    }

    [Test]
    public void IsAbstractTrue ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (AbstractBaseType), GenerationPolicy.ForceGeneration);
      Assert.IsTrue (bt1.IsAbstract);
    }

    [Test]
    public void IsAbstractFalse ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsFalse (bt1.IsAbstract);
    }
  }
}
