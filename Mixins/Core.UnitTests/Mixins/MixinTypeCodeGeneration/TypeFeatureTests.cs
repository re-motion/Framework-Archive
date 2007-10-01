using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins.MixinTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTests : MixinBaseTest
  {
    [Test]
    public void GeneratedMixinTypeHasMixinTypeAttribute ()
    {
      MixinDefinition mixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
      Assert.IsTrue (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false));

      ConcreteMixinTypeAttribute[] attributes =
          (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.AreEqual (1, attributes.Length);
    }

    [Test]
    public void MixinTypeAttributeCanBeUsedToGetMixinDefinition ()
    {
      MixinDefinition mixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
      Assert.IsTrue (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false));

      ConcreteMixinTypeAttribute[] attributes =
          (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.AreSame (mixinDefinition, attributes[0].GetMixinDefinition ());
    }

    [Test]
    public void GeneratedMixinTypeHasTypeInitializer ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      Type generatedType = ((IMixinTarget) com).Mixins[0].GetType ();
      Assert.IsNotNull (generatedType.GetConstructor (BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
    }

    [Test]
    public void NameProviderIsUsedWhenTypeIsGenerated ()
    {
      MockRepository repository = new MockRepository ();
      INameProvider nameProviderMock = repository.CreateMock<INameProvider> ();
      ConcreteTypeBuilder.Current.MixinTypeNameProvider = nameProviderMock;

      MixinDefinition mixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      Expect.Call (nameProviderMock.GetNewTypeName (mixinDefinition)).Return ("Bra");

      repository.ReplayAll ();

      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);

      Assert.AreEqual ("Bra", generatedType.FullName);

      repository.VerifyAll ();
    }

    [Test]
    public void NamesOfNestedTypesAreFlattened ()
    {
      MockRepository repository = new MockRepository ();
      INameProvider nameProviderMock = repository.CreateMock<INameProvider> ();
      ConcreteTypeBuilder.Current.MixinTypeNameProvider = nameProviderMock;

      MixinDefinition mixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      Expect.Call (nameProviderMock.GetNewTypeName (mixinDefinition)).Return ("Bra+Oof");

      repository.ReplayAll ();

      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);

      Assert.AreEqual ("Bra/Oof", generatedType.FullName);

      repository.VerifyAll ();
    }
  }
}