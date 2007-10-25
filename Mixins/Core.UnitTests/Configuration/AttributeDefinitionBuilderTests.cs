using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class AttributeDefinitionBuilderTests
  {
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TagAttribute : Attribute
    {
      public int Named;

      public TagAttribute () { }

      public TagAttribute (string s) { }
    }

    [Tag]
    [Tag ("Class!", Named = 5)]
    private class ClassWithLotsaAttributes
    {
      [Tag]
      [Tag ("Class!", Named = 5)]
      public void Foo ()
      {
      }
    }

    [Test]
    public void Attributes ()
    {
      TargetClassDefinition targetClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithLotsaAttributes),
          typeof (ClassWithLotsaAttributes));
      MixinDefinition mixin = targetClass.Mixins[typeof (ClassWithLotsaAttributes)];

      CheckAttributes (targetClass);
      CheckAttributes (mixin);

      CheckAttributes (targetClass.Methods[typeof (ClassWithLotsaAttributes).GetMethod ("Foo")]);
      CheckAttributes (mixin.Methods[typeof (ClassWithLotsaAttributes).GetMethod ("Foo")]);
    }

    private static void CheckAttributes (IAttributableDefinition attributableDefinition)
    {
      Assert.IsTrue (attributableDefinition.CustomAttributes.ContainsKey (typeof (TagAttribute)));
      Assert.AreEqual (2, attributableDefinition.CustomAttributes.GetItemCount (typeof (TagAttribute)));

      List<AttributeDefinition> attributes = new List<AttributeDefinition> (attributableDefinition.CustomAttributes);
      List<AttributeDefinition> attributes2 = new List<AttributeDefinition> (attributableDefinition.CustomAttributes[typeof (TagAttribute)]);
      foreach (AttributeDefinition attribute in attributes2)
      {
        Assert.IsTrue (attributes.Contains (attribute));
      }

      AttributeDefinition attribute1 = attributes.Find (
          delegate (AttributeDefinition a)
          {
            Assert.AreEqual (typeof (TagAttribute), a.AttributeType);
            return a.Data.Constructor.Equals (typeof (TagAttribute).GetConstructor (Type.EmptyTypes));
          });
      Assert.IsNotNull (attribute1);
      Assert.AreEqual (0, attribute1.Data.ConstructorArguments.Count);
      Assert.AreEqual (0, attribute1.Data.NamedArguments.Count);
      Assert.AreSame (attributableDefinition, attribute1.DeclaringDefinition);

      AttributeDefinition attribute2 = attributes.Find (
          delegate (AttributeDefinition a)
          {
            Assert.AreEqual (typeof (TagAttribute), a.AttributeType);
            return a.Data.Constructor.Equals (typeof (TagAttribute).GetConstructor (new Type[] { typeof (string) }));
          });
      Assert.IsNotNull (attribute2);
      Assert.AreEqual (1, attribute2.Data.ConstructorArguments.Count);
      Assert.AreEqual (typeof (string), attribute2.Data.ConstructorArguments[0].ArgumentType);
      Assert.AreEqual ("Class!", attribute2.Data.ConstructorArguments[0].Value);
      Assert.AreEqual (1, attribute2.Data.NamedArguments.Count);
      Assert.AreEqual (typeof (TagAttribute).GetField ("Named"), attribute2.Data.NamedArguments[0].MemberInfo);
      Assert.AreEqual (typeof (int), attribute2.Data.NamedArguments[0].TypedValue.ArgumentType);
      Assert.AreEqual (5, attribute2.Data.NamedArguments[0].TypedValue.Value);
      Assert.AreSame (attributableDefinition, attribute2.DeclaringDefinition);
      Assert.AreSame (attributableDefinition, attribute2.Parent);
    }

    [Test]
    public void SerializableAttributeIsIgnored ()
    {
      TargetClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1));
      Assert.IsFalse (bt1.CustomAttributes.ContainsKey (typeof (SerializableAttribute)));
    }

    [Test]
    public void ExtendsAttributeIsIgnored ()
    {
      MixinDefinition bt1m1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsFalse (bt1m1.CustomAttributes.ContainsKey (typeof (ExtendsAttribute)));
    }

    [Test]
    public void UsesAttributeIsIgnored ()
    {
      TargetClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType3));
      Assert.IsFalse (bt1.CustomAttributes.ContainsKey (typeof (UsesAttribute)));
    }

    [Test]
    public void OverrideAttributeIsIgnored ()
    {
      MixinDefinition bt1m1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsFalse (bt1m1.Methods[typeof (BT1Mixin1).GetMethod("VirtualMethod")].CustomAttributes.ContainsKey (typeof (OverrideTargetAttribute)));
    }

    class InternalStuffAttribute : Attribute { }

    [InternalStuff]
    class ClassWithInternalAttribute { }

    [Test]
    public void InternalAttributesAreIgnored()
    {
      using (MixinConfiguration.ScopedExtend(typeof (ClassWithInternalAttribute)))
      {
        Assert.IsFalse (
            TypeFactory.GetActiveConfiguration (typeof (ClassWithInternalAttribute)).CustomAttributes.ContainsKey (typeof (InternalStuffAttribute)));
      }
    }

    [Test]
    public void CopyAttributes_OnClass ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinIndirectlyAddingAttribute)))
      {
        MixinDefinition definition = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingAttribute)];
        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)));

        List<AttributeDefinition> attributes =
            new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.AreEqual (1, attributes.Count);
        Assert.AreEqual (typeof (AttributeWithParameters), attributes[0].AttributeType);
        Assert.AreEqual (typeof (AttributeWithParameters).GetConstructor (new Type[] { typeof (int), typeof (string) }),
            attributes[0].Data.Constructor);
        Assert.AreEqual (definition, attributes[0].DeclaringDefinition);

        Assert.AreEqual (2, attributes[0].Data.ConstructorArguments.Count);
        Assert.AreEqual (typeof (int), attributes[0].Data.ConstructorArguments[0].ArgumentType);
        Assert.AreEqual (1, attributes[0].Data.ConstructorArguments[0].Value);
        Assert.AreEqual (typeof (string), attributes[0].Data.ConstructorArguments[1].ArgumentType);
        Assert.AreEqual ("bla", attributes[0].Data.ConstructorArguments[1].Value);

        Assert.AreEqual (2, attributes[0].Data.NamedArguments.Count);

        Assert.AreEqual (typeof (AttributeWithParameters).GetField ("Field"), attributes[0].Data.NamedArguments[0].MemberInfo);
        Assert.AreEqual (typeof (int), attributes[0].Data.NamedArguments[0].TypedValue.ArgumentType);
        Assert.AreEqual (5, attributes[0].Data.NamedArguments[0].TypedValue.Value);

        Assert.AreEqual (typeof (AttributeWithParameters).GetProperty ("Property"), attributes[0].Data.NamedArguments[1].MemberInfo);
        Assert.AreEqual (typeof (int), attributes[0].Data.NamedArguments[1].TypedValue.ArgumentType);
        Assert.AreEqual (4, attributes[0].Data.NamedArguments[1].TypedValue.Value);
      }
    }

    [Test]
    public void CopyAttributes_OnMember ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinIndirectlyAddingAttribute)))
      {
        MethodDefinition definition = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinIndirectlyAddingAttribute)]
            .Methods[typeof (MixinIndirectlyAddingAttribute).GetMethod ("ToString")];

        Assert.IsFalse (definition.CustomAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)));
        Assert.IsTrue (definition.CustomAttributes.ContainsKey (typeof (AttributeWithParameters)));

        List<AttributeDefinition> attributes =
            new List<AttributeDefinition> (definition.CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.AreEqual (1, attributes.Count);
        Assert.AreEqual (typeof (AttributeWithParameters), attributes[0].AttributeType);
        Assert.AreEqual (typeof (AttributeWithParameters).GetConstructor (new Type[] { typeof (int) }), attributes[0].Data.Constructor);

        Assert.AreEqual (1, attributes[0].Data.ConstructorArguments.Count);
        Assert.AreEqual (typeof (int), attributes[0].Data.ConstructorArguments[0].ArgumentType);
        Assert.AreEqual (4, attributes[0].Data.ConstructorArguments[0].Value);

        Assert.AreEqual (0, attributes[0].Data.NamedArguments.Count);
      }
    }

    [IgnoreForMixinConfiguration]
    public class MixinWithAmbiguousSource
    {
      private void Source () { }
      private void Source (int i) { }

      [OverrideTarget]
      [CopyCustomAttributes (typeof (MixinWithAmbiguousSource), "Source")]
      protected new string ToString()
      {
        return "";
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + "Rubicon.Mixins.UnitTests.Configuration.AttributeDefinitionBuilderTests+MixinWithAmbiguousSource.ToString specifies an ambiguous attribute "
        + "source: The source member string Source matches several members on type "
        + "Rubicon.Mixins.UnitTests.Configuration.AttributeDefinitionBuilderTests+MixinWithAmbiguousSource.")]
    public void CopyAttributes_Ambiguous ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinWithAmbiguousSource)))
      {
        TypeFactory.GetActiveConfiguration (typeof (NullTarget));
      }
    }

    [IgnoreForMixinConfiguration]
    public class MixinWithUnknownSource
    {
      [OverrideTarget]
      [CopyCustomAttributes (typeof (MixinWithUnknownSource), "Source")]
      protected new string ToString ()
      {
        return "";
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + "Rubicon.Mixins.UnitTests.Configuration.AttributeDefinitionBuilderTests+MixinWithUnknownSource.ToString specifies an unknown attribute "
        + "source Rubicon.Mixins.UnitTests.Configuration.AttributeDefinitionBuilderTests+MixinWithUnknownSource.Source.")]
    public void CopyAttributes_Unknown ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinWithUnknownSource)))
      {
        TypeFactory.GetActiveConfiguration (typeof (NullTarget));
      }
    }

    [IgnoreForMixinConfiguration]
    public class MixinWithInvalidSourceType
    {
      [OverrideTarget]
      [CopyCustomAttributes (typeof (MixinWithInvalidSourceType))]
      protected new string ToString ()
      {
        return "";
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + "Rubicon.Mixins.UnitTests.Configuration.AttributeDefinitionBuilderTests+MixinWithInvalidSourceType.ToString specifies an attribute source "
        + "Rubicon.Mixins.UnitTests.Configuration.AttributeDefinitionBuilderTests+MixinWithInvalidSourceType of a different member kind.")]
    public void CopyAttributes_Invalid ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinWithInvalidSourceType)))
      {
        TypeFactory.GetActiveConfiguration (typeof (NullTarget));
      }
    }

    [IgnoreForMixinConfiguration]
    [CopyCustomAttributes(typeof (MixinWithSelfSource))]
    public class MixinWithSelfSource
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The CopyCustomAttributes attribute on "
        + "Rubicon.Mixins.UnitTests.Configuration.AttributeDefinitionBuilderTests.MixinWithSelfSource specifies itself as an attribute source.")]
    public void CopyAttributes_Self ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinWithSelfSource)))
      {
        TypeFactory.GetActiveConfiguration (typeof (NullTarget));
      }
    }
  }
}
