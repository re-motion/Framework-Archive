using System;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using System.Reflection;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class BaseClassDefinitionBuilderTests
  {
    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "contains generic parameters", MatchType = MessageMatch.Contains)]
    public void ThrowsOnGenericBaseClass()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BT3Mixin3<,>)))
      {
        BaseClassDefinitionBuilder builder = new BaseClassDefinitionBuilder();
        builder.Build (new ClassContext (typeof (BT3Mixin3<,>)));
      }
    }

    [Test]
    public void BaseClassKnowsItsContext()
    {
      ClassContext classContext = new ClassContext (typeof (BaseType1));
      using (MixinConfiguration.ScopedExtend(classContext))
      {
        BaseClassDefinition def = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
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
    public void BaseClassHasExplicitInterfaceMembers ()
    {
      BaseClassDefinition cweii = TypeFactory.GetActiveConfiguration (typeof (ClassWithExplicitInterfaceImplementation));
      Assert.AreEqual (7, cweii.Methods.Count);
      Assert.AreEqual (1, cweii.Properties.Count);
      Assert.AreEqual (1, cweii.Events.Count);

      BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      Assert.IsTrue (cweii.Methods.ContainsKey (typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
          "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.Method", bf)));

      Assert.IsTrue (
          cweii.Properties.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.Property", bf)));
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.get_Property", bf),
          cweii.Properties[
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.Property", bf)].GetMethod.
              MemberInfo);
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.set_Property", bf),
          cweii.Properties[
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.Property", bf)].SetMethod.
              MemberInfo);

      Assert.IsTrue (
          cweii.Events.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.Event", bf)));
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.add_Event", bf),
          cweii.Events[
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.Event", bf)].AddMethod.MemberInfo);
      Assert.AreEqual (
          typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
              "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.remove_Event", bf),
          cweii.Events[
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Rubicon.Mixins.UnitTests.Configuration.BaseClassDefinitionBuilderTests.IInterfaceWithAllMembers.Event", bf)].RemoveMethod.
              MemberInfo);
      
    }
  }
}
