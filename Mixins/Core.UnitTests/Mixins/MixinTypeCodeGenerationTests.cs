using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class MixinTypeCodeGenerationTests : MixinTestBase
  {
    [Test]
    public void OverrideMixinMethod ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      IMixinWithAbstractMembers comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedMethod-ClassOverridingMixinMembers.AbstractMethod-25",
          comAsIAbstractMixin.ImplementedMethod ());
    }

    [Test]
    public void OverrideMixinProperty ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      IMixinWithAbstractMembers comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedProperty-ClassOverridingMixinMembers.AbstractProperty",
          comAsIAbstractMixin.ImplementedProperty ());
    }

    [Test]
    public void OverrideMixinEvent ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      IMixinWithAbstractMembers comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedEvent", comAsIAbstractMixin.ImplementedEvent ());
    }

    [Test]
    public void DoubleOverride ()
    {
      ClassOverridingSingleMixinMethod com = CreateMixedObject<ClassOverridingSingleMixinMethod> (typeof (MixinOverridingClassMethod)).With ();
      IMixinOverridingClassMethod comAsIAbstractMixin = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("ClassOverridingSingleMixinMethod.AbstractMethod-25", comAsIAbstractMixin.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com.OverridableMethod (13));
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
    public void DefaultNameProviderIsGuid ()
    {
      Assert.AreSame (GuidNameProvider.Instance, ConcreteTypeBuilder.Current.MixinTypeNameProvider);
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
  }
}