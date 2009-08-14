// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeIsAssignableButDifferent ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.That (typeof (BaseType1).IsAssignableFrom (t), Is.True);
      Assert.That (t, Is.Not.EqualTo (typeof (BaseType1)));

      t = TypeFactory.GetConcreteType (typeof (BaseType2));
      Assert.That (typeof (BaseType2).IsAssignableFrom (t), Is.True);

      t = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.That (typeof (BaseType3).IsAssignableFrom (t), Is.True);

      t = TypeFactory.GetConcreteType (typeof (BaseType4));
      Assert.That (typeof (BaseType4).IsAssignableFrom (t), Is.True);

      t = TypeFactory.GetConcreteType (typeof (BaseType5));
      Assert.That (typeof (BaseType5).IsAssignableFrom (t), Is.True);

      Assert.That (ObjectFactory.Create<BaseType1> (ParamList.Empty), Is.Not.Null);
      Assert.That (ObjectFactory.Create<BaseType2> (ParamList.Empty), Is.Not.Null);
      Assert.That (ObjectFactory.Create<BaseType3> (ParamList.Empty), Is.Not.Null);
      Assert.That (ObjectFactory.Create<BaseType4> (ParamList.Empty), Is.Not.Null);
      Assert.That (ObjectFactory.Create<BaseType5> (ParamList.Empty), Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "constructor with the following signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated1 ()
    {
      ObjectFactory.Create<ClassWithCtors> (ParamList.Empty);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "constructor with the following signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated2 ()
    {
      ObjectFactory.Create<ClassWithCtors> (ParamList.Create (2.0));
    }

    [Test]
    public void ConstructorsAreReplicated3 ()
    {
      var c = ObjectFactory.Create<ClassWithCtors> (ParamList.Create (3));
      Assert.That (c.O, Is.EqualTo (3));
    }

    [Test]
    public void ConstructorsAreReplicated4 ()
    {
      var c = ObjectFactory.Create<ClassWithCtors> (ParamList.Create ("a"));
      Assert.That (c.O, Is.EqualTo ("a"));
    }

    [Test]
    public void ConstructorsAreReplicated5 ()
    {
      var nullMixin = new NullMixin ();
      var c = ObjectFactory.Create<ClassWithCtors> (ParamList.Create ("a"), nullMixin);
      Assert.That (c.O, Is.EqualTo ("a"));
      Assert.That (Mixin.Get<NullMixin> (c), Is.SameAs (nullMixin));
    }

    [Test]
    public void GeneratedTypeHasMixedTypeAttribute ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.That (generatedType.IsDefined (typeof (ConcreteMixedTypeAttribute), false), Is.True);

      var attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);
      Assert.That (attributes.Length, Is.EqualTo (1));
    }

    [Test]
    public void GeneratedTypeHasDebuggerDisplayAttribute ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.That (generatedType.IsDefined (typeof (DebuggerDisplayAttribute), false), Is.True);

      var attributes = (DebuggerDisplayAttribute[]) generatedType.GetCustomAttributes (typeof (DebuggerDisplayAttribute), false);
      Assert.That (attributes.Length, Is.EqualTo (1));
    }

    [Test]
    public void DebuggerDisplayAttribute_NotAddedIfExistsViaMixin ()
    {
      Type generatedType = CreateMixedType (typeof (NullTarget), typeof (MixinAddingDebuggerDisplay));
      var attributes = (DebuggerDisplayAttribute[]) generatedType.GetCustomAttributes (typeof (DebuggerDisplayAttribute), false);
      Assert.That (attributes.Length, Is.EqualTo (1));
      Assert.That (attributes[0].Value, Is.EqualTo ("Y"));
    }

    [Test]
    public void MixedTypeAttribute_GetsClassContext ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      var attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);
      ClassContext context = attributes[0].GetClassContext ();
      Assert.That (context, Is.EqualTo (MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (BaseType3))));
    }

    [Test]
    public void MixedTypeAttribute_GetsOrderedMixinTypes ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType7));
      var attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);

      // see MixinDependencySortTest.MixinDefinitionsAreSortedCorrectlySmall
      Assert.That (attributes[0].OrderedMixinTypes, Is.EqualTo (new[] { 
          typeof (BT7Mixin0), 
          typeof (BT7Mixin2), 
          typeof (BT7Mixin3), 
          typeof (BT7Mixin1), 
          typeof (BT7Mixin10), 
          typeof (BT7Mixin9), 
          typeof (BT7Mixin5) }));
    }

    [Test]
    public void MixedTypeAttribute_GetsClosedGenericMixinTypes ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      var attributes = (ConcreteMixedTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false);

      Assert.That (attributes[0].OrderedMixinTypes, List.Contains (typeof (BT3Mixin3<BaseType3, IBaseType33>)));
    }

    [Test]
    public void GeneratedTypeHasTypeInitializer ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.That (generatedType.GetConstructor (BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null), Is.Not.Null);
    }

    [Test]
    public void NameProviderIsUsedWhenTypeIsGenerated ()
    {
      var builder = new ConcreteTypeBuilder {Scope = SavedTypeBuilder.Scope};
      var repository = new MockRepository ();
      var nameProviderMock = repository.StrictMock<INameProvider> ();
      builder.TypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

      Expect.Call (nameProviderMock.GetNewTypeName (Arg<TargetClassDefinition>.Matches (tcd => tcd.Type == typeof (BaseType1)))).Return ("Foo");

      repository.ReplayAll ();

      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));

      Assert.That (generatedType.FullName, Is.EqualTo ("Foo"));

      repository.VerifyAll ();
    }

    [Test]
    public void NamesOfNestedTypesAreFlattened ()
    {
      var builder = new ConcreteTypeBuilder { Scope = SavedTypeBuilder.Scope };
      var repository = new MockRepository ();
      var nameProviderMock = repository.StrictMock<INameProvider> ();
      builder.TypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

      Expect.Call (nameProviderMock.GetNewTypeName (Arg<TargetClassDefinition>.Matches (tcd => tcd.Type == typeof (BaseType1)))).Return ("Foo+Bar");

      repository.ReplayAll ();

      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));

      Assert.That (generatedType.FullName, Is.EqualTo ("Foo/Bar"));

      repository.VerifyAll ();
    }

    [Test]
    public void AbstractBaseTypesLeadToAbstractConcreteTypes ()
    {
      Type concreteType = CreateMixedType (typeof (AbstractBaseType), typeof (MixinOverridingClassMethod));
      Assert.That (concreteType, Is.Not.Null);
      Assert.That (concreteType.IsAbstract, Is.True);
      MethodInfo[] abstractMethods = Array.FindAll (concreteType.GetMethods (), method => method.IsAbstract);
      string[] abstractMethodNames = Array.ConvertAll (abstractMethods, method => method.Name);
      Assert.That (abstractMethodNames, Is.EquivalentTo (new[] { "VirtualMethod", "get_VirtualProperty", "set_VirtualProperty",
          "add_VirtualEvent", "remove_VirtualEvent" }));
    }

    [Test]
    public void DeserializationConstructorGeneratedEvenIfBaseNotISerializable ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1));
      Assert.That (typeof (BaseType1).GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                  null, new[] { typeof (SerializationInfo), typeof (StreamingContext) }, null), Is.Null);
      Assert.That (concreteType.GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                  null, new[] { typeof (SerializationInfo), typeof (StreamingContext) }, null), Is.Not.Null);
    }

    [Test]
    public void CopiedAttributesAreNotReplicated ()
    {
      Type concreteType = CreateMixedType (typeof (ClassWithCopyCustomAttributes));
      Assert.That (concreteType, Is.Not.SameAs (typeof (ClassWithCopyCustomAttributes)));
      Assert.IsEmpty (concreteType.GetCustomAttributes (typeof (SampleCopyTemplateAttribute), true));
    }

    [Test]
    public void ValueTypeMixin ()
    {
      CreateMixedType (typeof (BaseType1), typeof (ValueTypeMixin));
    }

    [Test]
    public void ExtensionsFieldIsPrivate ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (BT1Mixin1));
      Assert.That (concreteType.GetField ("__extensions", BindingFlags.NonPublic | BindingFlags.Instance).Attributes, Is.EqualTo (FieldAttributes.Private));
    }

    [Test]
    public void FirstFieldIsPrivate ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (BT1Mixin1));
      Assert.That (concreteType.GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).Attributes, Is.EqualTo (FieldAttributes.Private));
    }

    [Test]
    public void ConfigurationFieldIsPrivate ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (BT1Mixin1));
      var classContextField = concreteType.GetField ("__classContext", BindingFlags.NonPublic | BindingFlags.Static);
      Assert.That (classContextField.Attributes, Is.EqualTo (FieldAttributes.Private | FieldAttributes.Static));
    }

    [Test]
    public void IMixinTarget ()
    {
      var mixinTarget = (IMixinTarget) ObjectFactory.Create<BaseType1> (ParamList.Empty);

      var expectedClassContext = MixinConfiguration.ActiveConfiguration.GetContext (
          typeof (BaseType1), 
          GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.That (mixinTarget.ClassContext, Is.Not.Null);
      Assert.That (mixinTarget.ClassContext, Is.EqualTo (expectedClassContext));

      object[] mixins = mixinTarget.Mixins;
      Assert.That (mixins, Is.Not.Null);
      Assert.That (mixins.Length, Is.EqualTo (mixinTarget.ClassContext.Mixins.Count));

      var mixinTypes = MixinTypeUtility.GetMixinTypesExact (typeof (BaseType1));
      Assert.That (mixins[0], Is.InstanceOfType (mixinTypes[0]));
    }
  }
}
