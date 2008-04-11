using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.MixinTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsMarkerInterface ()
    {
      MixinDefinition mixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);
      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
      Assert.IsTrue (typeof (IGeneratedMixinType).IsAssignableFrom (generatedType));
    }

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

    [Test]
    public void AttributesAreReplicatedIfNecessary ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithProtectedOverriderAndAttributes)).EnterScope())
      {
        MixinDefinition mixinDefinition =
            TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinWithProtectedOverriderAndAttributes)];
        Assert.IsNotNull (mixinDefinition);
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.AreNotSame (typeof (MixinWithProtectedOverrider), generatedType);

        object[] inheritableAttributes = generatedType.GetCustomAttributes (typeof (InheritableAttribute), true);
        object[] nonInheritableAttributes = generatedType.GetCustomAttributes (typeof (NonInheritableAttribute), true);
        
        Assert.AreEqual (1, inheritableAttributes.Length);
        Assert.That (inheritableAttributes, Is.EquivalentTo (typeof (MixinWithProtectedOverriderAndAttributes).GetCustomAttributes (typeof (InheritableAttribute), true)));

        Assert.AreEqual (1, nonInheritableAttributes.Length);
        Assert.That (nonInheritableAttributes, Is.EquivalentTo (typeof (MixinWithProtectedOverriderAndAttributes).GetCustomAttributes (typeof (NonInheritableAttribute), true)));
      }
    }

    [Test]
    public void CopyTemplatesAreNotReplicated ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithProtectedOverriderAndAttributes)).EnterScope())
      {
        MixinDefinition mixinDefinition =
            TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinWithProtectedOverriderAndAttributes)];
        Assert.IsNotNull (mixinDefinition);
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.AreNotSame (typeof (MixinWithProtectedOverrider), generatedType);

        object[] copiedAttributes = generatedType.GetCustomAttributes (typeof (SampleCopyTemplateAttribute), true);
        Assert.AreEqual (0, copiedAttributes.Length);
      }
    }
  }
}