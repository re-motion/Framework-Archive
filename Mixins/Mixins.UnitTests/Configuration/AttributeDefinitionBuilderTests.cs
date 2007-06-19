using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Configuration
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
      BaseClassDefinition baseClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithLotsaAttributes),
          typeof (ClassWithLotsaAttributes));
      MixinDefinition mixin = baseClass.Mixins[typeof (ClassWithLotsaAttributes)];

      CheckAttributes (baseClass);
      CheckAttributes (mixin);

      CheckAttributes (baseClass.Methods[typeof (ClassWithLotsaAttributes).GetMethod ("Foo")]);
      CheckAttributes (mixin.Methods[typeof (ClassWithLotsaAttributes).GetMethod ("Foo")]);
    }

    private static void CheckAttributes (IAttributableDefinition attributableDefinition)
    {
      Assert.IsTrue (attributableDefinition.CustomAttributes.HasItem (typeof (TagAttribute)));
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
      BaseClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1));
      Assert.IsFalse (bt1.CustomAttributes.HasItem (typeof (SerializableAttribute)));
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
            TypeFactory.GetActiveConfiguration (typeof (ClassWithInternalAttribute)).CustomAttributes.HasItem (typeof (InternalStuffAttribute)));
      }
    }
  }
}
