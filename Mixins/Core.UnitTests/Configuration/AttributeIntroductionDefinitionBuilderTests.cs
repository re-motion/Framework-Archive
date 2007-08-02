using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class AttributeIntroductionDefinitionBuilderTests
  {
    [Test]
    public void MixinsIntroduceAttributes ()
    {
      BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      Assert.AreEqual (2, bt1.CustomAttributes.Count);
      Assert.IsTrue (bt1.CustomAttributes.ContainsKey (typeof (BT1Attribute)));
      Assert.IsTrue (bt1.CustomAttributes.ContainsKey (typeof (DefaultMemberAttribute)));

      MixinDefinition mixin1 = bt1.Mixins[typeof (BT1Mixin1)];
      Assert.AreEqual (1, mixin1.CustomAttributes.Count);
      Assert.IsTrue (mixin1.CustomAttributes.ContainsKey (typeof (BT1M1Attribute)));

      MixinDefinition mixin2 = bt1.Mixins[typeof (BT1Mixin2)];
      Assert.AreEqual (0, mixin2.CustomAttributes.Count);

      Assert.AreEqual (1, bt1.IntroducedAttributes.Count);
      Assert.AreSame (mixin1.CustomAttributes[0], bt1.IntroducedAttributes[0].Attribute);
      Assert.AreSame (bt1, bt1.IntroducedAttributes[0].Parent);
      Assert.AreEqual (mixin1.CustomAttributes[0].FullName, bt1.IntroducedAttributes[0].FullName);
      Assert.AreEqual (mixin1.CustomAttributes[0].AttributeType, bt1.IntroducedAttributes[0].AttributeType);
    }

    [Test]
    public void MixinsIntroduceAttributesToMembers ()
    {
      BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      MethodDefinition member = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      Assert.AreEqual (1, member.CustomAttributes.Count);
      Assert.AreEqual (1, member.IntroducedAttributes.Count);

      MixinDefinition mixin = bt1.Mixins[typeof (BT1Mixin1)];
      MethodDefinition mixinMember = mixin.Methods[typeof (BT1Mixin1).GetMethod ("VirtualMethod")];

      Assert.IsTrue (member.IntroducedAttributes.ContainsKey (typeof (BT1M1Attribute)));
      Assert.AreEqual (1, mixinMember.CustomAttributes.Count);
      Assert.AreSame (mixinMember.CustomAttributes[0], member.IntroducedAttributes[0].Attribute);
      Assert.AreSame (member, member.IntroducedAttributes[0].Parent);
      Assert.AreEqual (mixinMember.CustomAttributes[0].FullName, member.IntroducedAttributes[0].FullName);
      Assert.AreEqual (mixinMember.CustomAttributes[0].AttributeType, member.IntroducedAttributes[0].AttributeType);
    }

    [Test]
    public void MultipleAttributes ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
          typeof (BaseTypeWithAllowMultiple), typeof (MixinAddingAllowMultipleToClassAndMember), typeof (MixinAddingAllowMultipleToClassAndMember2));
      Assert.AreEqual (2, definition.IntroducedAttributes.GetItemCount (typeof (MultiAttribute)));
      Assert.AreEqual (1, definition.CustomAttributes.GetItemCount (typeof (MultiAttribute)));
    }

    [Test]
    public void MultipleAttributesOnMembers ()
    {
      BaseClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseTypeWithAllowMultiple),
          typeof (MixinAddingAllowMultipleToClassAndMember), typeof (MixinAddingAllowMultipleToClassAndMember2));
      MethodDefinition member = bt1.Methods[typeof (BaseTypeWithAllowMultiple).GetMethod ("Foo")];

      Assert.AreEqual (1, member.CustomAttributes.Count);
      Assert.AreEqual (2, member.Overrides.Count);
      Assert.AreEqual (2, member.IntroducedAttributes.Count);

      MixinDefinition mixin1 = bt1.Mixins[typeof (MixinAddingAllowMultipleToClassAndMember)];
      MethodDefinition mixinMember1 = mixin1.Methods[typeof (MixinAddingAllowMultipleToClassAndMember).GetMethod ("Foo")];
      Assert.AreEqual (1, mixinMember1.CustomAttributes.Count);

      MixinDefinition mixin2 = bt1.Mixins[typeof (MixinAddingAllowMultipleToClassAndMember2)];
      MethodDefinition mixinMember2 = mixin2.Methods[typeof (MixinAddingAllowMultipleToClassAndMember).GetMethod ("Foo")];
      Assert.AreEqual (1, mixinMember2.CustomAttributes.Count);

      Assert.IsTrue (member.IntroducedAttributes.ContainsKey (typeof (MultiAttribute)));
      Assert.AreEqual (2, member.IntroducedAttributes.GetItemCount (typeof (MultiAttribute)));

      List<AttributeDefinition> attributes =
          new List<AttributeIntroductionDefinition> (member.IntroducedAttributes[typeof (MultiAttribute)])
          .ConvertAll<AttributeDefinition> (delegate (AttributeIntroductionDefinition intro) { return intro.Attribute; });
      Assert.That (attributes, Is.EquivalentTo (new AttributeDefinition[] { mixinMember1.CustomAttributes[0], mixinMember2.CustomAttributes[0] }));
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NonInheritedAttribute : Attribute
    {
    }

    [NonInherited]
    class MixinAddingNonInheritedAttribute
    {
    }

    [Test]
    public void NonInheritedAttributesAreNotIntroduced ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingNonInheritedAttribute));
      Assert.AreEqual (0, definition.IntroducedAttributes.GetItemCount (typeof (NonInheritedAttribute)));
    }

    [Test]
    public void WithNonMultipleInheritedAttributesTheBaseClassWins ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingBT1Attribute));
      Assert.AreEqual (0, definition.IntroducedAttributes.GetItemCount (typeof (BT1Attribute)));
      Assert.AreEqual (1, definition.CustomAttributes.GetItemCount (typeof (BT1Attribute)));
    }

    [Test]
    public void WithNonMultipleInheritedAttributesOnMemberTheBaseClassWins ()
    {
      MethodDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingBT1AttributeToMember)).Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      Assert.AreEqual (0, definition.IntroducedAttributes.GetItemCount (typeof (BT1Attribute)));
      Assert.AreEqual (1, definition.CustomAttributes.GetItemCount (typeof (BT1Attribute)));
    }
  }
}