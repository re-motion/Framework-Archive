using System;
using System.Runtime.Serialization;
using Rubicon.Mixins;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using NUnit.Framework;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class TypeFactoryTests
  {
    [Test]
    public void GetActiveConfiguration()
    {
      using (MixinConfiguration.ScopedEmpty())
      {
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType2)));
        Assert.IsNull (TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
        Assert.IsNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType2)));

        using (MixinConfiguration.ScopedExtend(typeof (BaseType1)))
        {
          Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
          Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType2)));
          Assert.AreSame (
              TargetClassDefinitionCache.Current.GetTargetClassDefinition (new ClassContext (typeof (BaseType1))),
              TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
          Assert.IsNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
          Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
          Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType2)));

          using (MixinConfiguration.ScopedExtend(typeof (BaseType2)))
          {
            Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType1)));
            Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (BaseType2)));

            Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
            Assert.AreSame (
                TargetClassDefinitionCache.Current.GetTargetClassDefinition (new ClassContext (typeof (BaseType1))),
                TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
            Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
            Assert.AreSame (
                TargetClassDefinitionCache.Current.GetTargetClassDefinition (new ClassContext (typeof (BaseType2))),
                TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
          }
        }
      }
    }

    [Uses (typeof (NullMixin))]
    public class GenericTypeWithMixin<T>
    { }

    [Test]
    public void GetActiveConfigurationWithGenericTypes ()
    {
      TargetClassDefinition def = TypeFactory.GetActiveConfiguration (typeof (GenericTypeWithMixin<int>));
      Assert.AreEqual (typeof (GenericTypeWithMixin<int>), def.Type);
      Assert.IsTrue (def.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void MixinExtendingSpecificGenericType ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (GenericClassExtendedByMixin<int>), typeof (MixinExtendingSpecificGenericClass)))
      {
        TargetClassDefinition targetClassDefinition =
            TypeFactory.GetActiveConfiguration (typeof (GenericClassExtendedByMixin<int>), GenerationPolicy.GenerateOnlyIfConfigured);
        Assert.IsNotNull (targetClassDefinition);
        Assert.IsTrue (targetClassDefinition.Mixins.ContainsKey (typeof (MixinExtendingSpecificGenericClass)));

        Assert.IsNull (TypeFactory.GetActiveConfiguration (typeof (GenericClassExtendedByMixin<string>), GenerationPolicy.GenerateOnlyIfConfigured));
        Assert.IsNull (TypeFactory.GetActiveConfiguration (typeof (GenericClassExtendedByMixin<>), GenerationPolicy.GenerateOnlyIfConfigured));
      }
    }

    [Test]
    public void NoDefinitionGeneratedIfNoConfigByDefault()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (object)));
      Assert.IsNull (TypeFactory.GetActiveConfiguration (typeof (object)));
    }

    [Test]
    public void NoNewDefinitionGeneratedForGeneratedTypeByDefault ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      TargetClassDefinition definition = TypeFactory.GetActiveConfiguration (generatedType);
      Assert.AreSame (definition, TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
    }

    [Test]
    public void DefinitionGeneratedIfNoConfigViaPolicy ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (object)));
      TargetClassDefinition configuration = TypeFactory.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      Assert.IsNotNull (configuration);
      Assert.AreEqual (typeof (object), configuration.Type);
    }

    [Test]
    public void NewDefinitionGeneratedForGeneratedTypeViaPolicy ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      TargetClassDefinition newDefinition = TypeFactory.GetActiveConfiguration (generatedType, GenerationPolicy.ForceGeneration);
      TargetClassDefinition baseDefinition = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      Assert.AreNotSame (baseDefinition, newDefinition);
      Assert.AreSame (baseDefinition.Type, newDefinition.Type.BaseType);
    }

    [Test]
    public void ForcedDefinitionsAreCached ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (object)));
      TargetClassDefinition d1 = TypeFactory.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      TargetClassDefinition d2 = TypeFactory.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      Assert.AreSame (d1, d2);
    }

    [Test]
    public void ForcedGenerationIsNotPersistent ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (object)));
      TargetClassDefinition d1 = TypeFactory.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      TargetClassDefinition d2 = TypeFactory.GetActiveConfiguration (typeof (object), GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.IsNotNull (d1);
      Assert.IsNull (d2);
    }

    [Test]
    public void NoTypeGeneratedIfNoConfigByDefault ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (object)));
      Assert.AreSame (typeof (object), TypeFactory.GetConcreteType (typeof (object)));
    }

    [Test]
    public void NoTypeGeneratedIfGeneratedTypeIsGivenByDefault ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreSame (concreteType, TypeFactory.GetConcreteType (concreteType));
    }

    [Test]
    public void TypeGeneratedIfNoConfigViaPolicy ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (object)));
      Type concreteType = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
      Assert.AreNotSame (typeof (object), concreteType);
      Assert.AreSame (typeof (object), concreteType.BaseType);
    }

    [Test]
    public void TypeGeneratedIfGeneratedTypeIsGivenViaPolicy ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Type concreteType2 = TypeFactory.GetConcreteType (concreteType, GenerationPolicy.ForceGeneration);
      Assert.AreNotSame (concreteType, concreteType2);
      Assert.AreSame (concreteType, concreteType2.BaseType);
    }

    [Test]
    public void InitializeUnconstructedInstance ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType3));
      BaseType3 bt3 = (BaseType3) FormatterServices.GetSafeUninitializedObject (concreteType);
      TypeFactory.InitializeUnconstructedInstance (bt3 as IMixinTarget);
      BT3Mixin1 bt3m1 = Mixin.Get<BT3Mixin1> (bt3);
      Assert.IsNotNull (bt3m1, "Mixin must have been created");
      Assert.AreSame (bt3, bt3m1.This, "Mixin must have been initialized");
    }
  }
}
