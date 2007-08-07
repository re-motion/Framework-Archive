using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes;
using Rubicon.Mixins.UnitTests.Mixins;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ValidationTests
  {
    public static void Main ()
    {
      new MixinSerializationTests().RespectsISerializable();
    }

    public static bool HasFailure (string ruleName, IValidationLog log)
    {
      foreach (ValidationResult result in log.GetResults())
      {
        foreach (ValidationResultItem item in result.Failures)
        {
          if (item.Rule.RuleName == ruleName)
            return true;
        }
      }
      return false;
    }

    public static bool HasWarning (string ruleName, IValidationLog log)
    {
      foreach (ValidationResult result in log.GetResults())
      {
        foreach (ValidationResultItem item in result.Warnings)
        {
          if (item.Rule.RuleName == ruleName)
            return true;
        }
      }
      return false;
    }

    private void AssertSuccess (IValidationLog log)
    {
      Assert.AreEqual (0, log.GetNumberOfFailures ());
      Assert.AreEqual (0, log.GetNumberOfWarnings ());
      Assert.AreEqual (0, log.GetNumberOfUnexpectedExceptions ());
    }

    [Test]
    public void ValidationVisitsSomething ()
    {
      IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
      Assert.IsTrue (log.ResultCount > 1);
    }

    [Test]
    public void ValidationDump ()
    {
      IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
      ConsoleDumper.DumpValidationResults (log.GetResults ());
    }

    [Test]
    public void ValidationResultDefinition ()
    {
      IValidationLog log = MixinConfiguration.ActiveContext.Validate ();

      IEnumerator<ValidationResult> results = log.GetResults().GetEnumerator();
      Assert.IsTrue (results.MoveNext());
      ValidationResult firstResult = results.Current;
      Assert.IsNotNull (firstResult.Definition);
    }

    [Test]
    public void AllIsValid ()
    {
      IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
      AssertSuccess (log);
    }

    [Test]
    public void AllIsVisitedOnce ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        IValidationLog log = MixinConfiguration.ActiveContext.Validate();

        Dictionary<IVisitableDefinition, IVisitableDefinition> visitedDefinitions = new Dictionary<IVisitableDefinition, IVisitableDefinition>();
        foreach (ValidationResult result in log.GetResults())
        {
          Assert.IsNotNull (result.Definition);
          Assert.IsFalse (visitedDefinitions.ContainsKey (result.Definition));
          visitedDefinitions.Add (result.Definition, result.Definition);
        }

        BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt1));
        BaseClassDefinition bt3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3));
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt3));

        MixinDefinition bt1m1 = bt1.Mixins[typeof (BT1Mixin1)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt1m1));
        MixinDefinition bt1m2 = bt1.Mixins[typeof (BT1Mixin2)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt1m2));
        MixinDefinition bt3m1 = bt3.Mixins[typeof (BT3Mixin1)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m1));
        MixinDefinition bt3m2 = bt3.Mixins[typeof (BT3Mixin2)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m2));
        MixinDefinition bt3m3 = bt3.GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m3));
        MixinDefinition bt3m4 = bt3.Mixins[typeof (BT3Mixin4)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m4));
        MixinDefinition bt3m5 = bt3.Mixins[typeof (BT3Mixin5)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m5));

        MethodDefinition m1 = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (m1));
        MethodDefinition m2 = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] {typeof (string)})];
        Assert.IsTrue (visitedDefinitions.ContainsKey (m2));
        MethodDefinition m3 = bt1m1.Methods[typeof (BT1Mixin1).GetMethod ("VirtualMethod")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (m3));
        MethodDefinition m4 = bt1m1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (m4));

        PropertyDefinition p1 = bt1.Properties[typeof (BaseType1).GetProperty ("VirtualProperty")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (p1));
        MethodDefinition m5 = p1.GetMethod;
        Assert.IsTrue (visitedDefinitions.ContainsKey (m5));
        MethodDefinition m6 = p1.SetMethod;
        Assert.IsTrue (visitedDefinitions.ContainsKey (m6));
        PropertyDefinition p2 = bt1m1.Properties[typeof (BT1Mixin1).GetProperty ("VirtualProperty")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (p2));

        EventDefinition e1 = bt1.Events[typeof (BaseType1).GetEvent ("VirtualEvent")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (e1));
        MethodDefinition m7 = e1.AddMethod;
        Assert.IsTrue (visitedDefinitions.ContainsKey (m7));
        MethodDefinition m8 = e1.RemoveMethod;
        Assert.IsTrue (visitedDefinitions.ContainsKey (m8));
        EventDefinition e2 = bt1m1.Events[typeof (BT1Mixin1).GetEvent ("VirtualEvent")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (e2));

        InterfaceIntroductionDefinition i1 = bt1m1.InterfaceIntroductions[typeof (IBT1Mixin1)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (i1));
        MethodIntroductionDefinition im1 = i1.IntroducedMethods[typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (im1));
        PropertyIntroductionDefinition im2 = i1.IntroducedProperties[typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (im2));
        EventIntroductionDefinition im3 = i1.IntroducedEvents[typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (im3));

        AttributeDefinition a1 = bt1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (a1));
        AttributeDefinition a2 = bt1m1.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (a2));
        AttributeDefinition a3 = m1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (a3));
        AttributeDefinition a4 = p1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (a4));
        AttributeDefinition a5 = e1.CustomAttributes.GetFirstItem (typeof (BT1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (a5));
        AttributeDefinition a6 = im1.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (a6));
        AttributeDefinition a7 = im2.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (a7));
        AttributeDefinition a8 = im3.ImplementingMember.CustomAttributes.GetFirstItem (typeof (BT1M1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (a8));

        AttributeIntroductionDefinition ai1 = bt1.IntroducedAttributes.GetFirstItem (typeof (BT1M1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (ai1));
        AttributeIntroductionDefinition ai2 = m1.IntroducedAttributes.GetFirstItem (typeof (BT1M1Attribute));
        Assert.IsTrue (visitedDefinitions.ContainsKey (ai2));

        RequiredBaseCallTypeDefinition bc1 = bt3.RequiredBaseCallTypes[typeof (IBaseType34)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bc1));
        RequiredMethodDefinition bcm1 = bc1.Methods[typeof (IBaseType34).GetMethod ("IfcMethod")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bcm1));

        RequiredFaceTypeDefinition ft1 = bt3.RequiredFaceTypes[typeof (IBaseType32)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (ft1));
        RequiredMethodDefinition fm1 = ft1.Methods[typeof (IBaseType32).GetMethod ("IfcMethod")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (fm1));

        ThisDependencyDefinition td1 = bt3m1.ThisDependencies[typeof (IBaseType31)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (td1));

        BaseDependencyDefinition bd1 = bt3m1.BaseDependencies[typeof (IBaseType31)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bd1));
      }
    }

    [Test]
    public void HasDefaultRules ()
    {
      IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
      Assert.IsTrue (log.GetNumberOfRulesExecuted () > 0);
    }

    [Test]
    public void CollectsUnexpectedExceptions ()
    {
      BaseClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc, new ThrowingRuleSet());
      Assert.IsTrue (log.GetNumberOfUnexpectedExceptions() > 0);
      List<ValidationResult> results = new List<ValidationResult> (log.GetResults());
      Assert.IsTrue (results[0].Exceptions[0].Exception is InvalidOperationException);
    }

    [Test]
    public void FailsIfSealedBaseClass ()
    {
      BaseClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultBaseClassRules.BaseClassMustNotBeSealed", log));
      Assert.AreEqual (0, log.GetNumberOfWarnings());
    }

    [Test]
    public void SucceedsIfAbstractBaseClass ()
    {
      BaseClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (MixinWithAbstractMembers));
      DefaultValidationLog log = Validator.Validate (bc);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfOverriddenMethodNotVirtual ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Methods[typeof (BaseType4).GetMethod ("NonVirtualMethod")]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfOverriddenBaseMethodAbstract ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (AbstractBaseType), typeof (BT1Mixin1));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.AbstractBaseClassMethodMustNotBeOverridden", log));
    }

    [Test]
    public void FailsIfOverriddenMethodFinal ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithFinalMethod), typeof (MixinForFinalMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustNotBeFinal", log));
    }

    [Test]
    public void FailsIfOverriddenPropertyMethodNotVirtual ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Properties[typeof (BaseType4).GetProperty ("NonVirtualProperty")]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfOverriddenEventMethodNotVirtual ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Events[typeof (BaseType4).GetEvent ("NonVirtualEvent")]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void WarnsIfPropertyOverrideAddsMethods ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseWithGetterOnly), typeof (MixinOverridingSetterOnly));
      DefaultValidationLog log =
          Validator.Validate (definition.Properties[typeof (BaseWithGetterOnly).GetProperty ("Property")].Overrides[0]);

      Assert.IsTrue (HasWarning ("Rubicon.Mixins.Validation.Rules.DefaultPropertyRules.NewMemberAddedByOverride", log));
    }

    [Test]
    public void FailsIfMixinIsInterface ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (IBT1Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (IBT1Mixin1)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotBeInterface", log));
    }

    [Test]
    public void WarnsIfIntroducedInterfaceIsShadowed ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (DoubleImplementer));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasWarning ("Rubicon.Mixins.Validation.Rules.DefaultSuppressedInterfaceIntroductionRules.InterfaceIsShadowedByBaseClass", log));
    }

    [Test]
    public void FailsIfIntroducedInterfaceNotVisible ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (MixinIntroducingInternalInterface));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.IntroducedInterfaceMustBePublic", log));
    }

    [Test]
    public void FailsIfRequiredFaceClassNotAvailable ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassLookingLikeBaseType3), typeof (MixinWithClassThisDependency));
      DefaultValidationLog log = Validator.Validate (definition.RequiredFaceTypes[typeof (BaseType3)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceClassMustBeAssignableFromTargetType", log));
    }

    [Test]
    public void FailsIfRequiredFaceTypeNotVisible ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithInvisibleThisDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.RequiredFaceTypeMustBePublic", log));
    }

    [Test]
    public void FailsIfRequiredBaseTypeNotVisible ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithInvisibleBaseDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultRequiredBaseCallTypeRules.RequiredBaseCallTypeMustBePublic", log));
    }

    [Test]
    public void SucceedsIfEmptyThisDependencyNotFulfilled ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedEmptyThisDependency));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedEmptyThisDependency)].
              ThisDependencies[typeof (IEmptyInterface)]);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfEmptyBaseDependencyNotFulfilled ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedEmptyBaseDependency));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedEmptyBaseDependency)].
              BaseDependencies[typeof (IEmptyInterface)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }

    [Test]
    public void SucceedsIfCircularThisDependency ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithCircularThisDependency1), typeof (MixinWithCircularThisDependency2));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithCircularThisDependency1)]);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfDuckThisDependency ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck),
          typeof (MixinRequiringAllMembersFace));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfDuckBaseDependency ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck),
          typeof (MixinRequiringAllMembersBase));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfAggregateThisDependencyIsFullyImplemented ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Face));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfAggregateBaseDependencyIsFullyImplemented ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfEmptyAggregateThisDependencyIsNotAvailable ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (object), typeof (MixinWithUnsatisfiedEmptyAggregateThisDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfEmptyAggregateBaseDependencyIsNotAvailable ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (object), typeof (MixinWithUnsatisfiedEmptyAggregateBaseDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }

    [Test]
    public void FailsIfRequiredBaseMethodIsExplit ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersBase)))
      {
        BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
            typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersBase));
        DefaultValidationLog log = Validator.Validate (definition);

        Assert.IsTrue (
            HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultRequiredMethodRules.RequiredBaseCallMethodMustBePublicOrProtected", log));
      }
    }

    [Test]
    public void SucceedsIfRequiredFaceMethodIsExplit ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersFace)))
      {
        BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
            typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersFace));
        DefaultValidationLog log = Validator.Validate (definition);

        AssertSuccess (log);
      }
    }

    [Test]
    public void FailsIfImplementingIMixinTarget ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinImplementingIMixinTarget));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.IMixinTargetCannotBeIntroduced", log));
    }

    [Test]
    public void SucceedsIfBaseClassWinsWhenDefiningAttributes ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1),
          typeof (MixinAddingBT1Attribute));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess(log);
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixin ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (MixinAddingBT1Attribute),
           typeof (MixinAddingBT1Attribute2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure (
          "Rubicon.Mixins.Validation.Rules.DefaultAttributeIntroductionRules.AllowMultipleRequiredIfAttributeIntroducedMultipleTimes", log));
      Assert.AreEqual (2, log.GetNumberOfFailures ());
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixinToMember ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithVirtualMethod),
          typeof (MixinAddingBT1AttributeToMember), typeof (MixinAddingBT1AttributeToMember2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (
          HasFailure (
              "Rubicon.Mixins.Validation.Rules.DefaultAttributeIntroductionRules.AllowMultipleRequiredIfAttributeIntroducedMultipleTimes", log));
      Assert.AreEqual (2, log.GetNumberOfFailures());
    }

    [Test]
    public void SucceedsIfDuplicateAttributeAddedByMixinAllowsMultiple ()
    {
      BaseClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseTypeWithAllowMultiple), typeof (MixinAddingAllowMultipleToClassAndMember));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfMixinNonPublic ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin2));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (BT5Mixin2)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void SucceedsIfNestedPublicMixin ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.PublicNested));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfNestedPublicMixinInNonPublic ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.PublicNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNestedPrivateMixin ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.InternalNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNestedPrivateMixinInNonPublic ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.InternalNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfOverriddenMixinMethodNotVirtual ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinWithNonVirtualMethodToBeOverridden));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfAbstractMixinMethodHasNoOverride ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithAbstractMembers));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.AbstractMixinMethodMustBeOverridden", log));
    }

    [Test]
    public void FailsIfCrossOverridesOnSameMethods ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinOverridingSameClassMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.NoCircularOverrides", log));
    }

    [Test]
    public void SucceedsIfCrossOverridesNotOnSameMethods ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinOverridingClassMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfBaseClassDefinitionIsInterface ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (IBaseType2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultBaseClassRules.BaseClassMustNotBeAnInterface", log));
    }

    [Test]
    public void FailsIfMixinMethodIsOverriddenWhichHasNoThisProperty ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod), typeof (AbstractMixinWithoutBase));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMethodRules.OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase", log));
    }

    [Test]
    public void SucceedsIfOverridingMembersAreProtected ()
    {
      BaseClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
      Assert.IsTrue (definition.Mixins[0].HasProtectedOverriders());
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfNoPublicOrProtectedCtorInBaseClass ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithPrivateCtor),
          typeof (NullMixin));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultBaseClassRules.BaseClassMustHavePublicOrProtectedCtor", log));
    }

    [Test]
    public void FailsIfNoPublicOrProtectedDefaultCtorInMixinClassWithOverriddenMembers ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinWithPrivateCtorAndVirtualMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor",
          log));
    }

    [Test]
    public void SucceedsIfNoPublicOrProtectedDefaultCtorInMixinClassWithoutOverriddenMembers ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (object),
          typeof (MixinWithPrivateCtorAndVirtualMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void ValidationException ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod), typeof (AbstractMixinWithoutBase));
      DefaultValidationLog log = Validator.Validate (definition);

      ValidationException exception = new ValidationException (log);
      Assert.AreEqual ("Some parts of the mixin configuration could not be validated." + Environment.NewLine + "Rubicon.Mixins.UnitTests.Configuration."
          + "ValidationSampleTypes.AbstractMixinWithoutBase.AbstractMethod (Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes."
          + "AbstractMixinWithoutBase -> Rubicon.Mixins.UnitTests.SampleTypes.ClassOverridingSingleMixinMethod): There were 1 errors, 0 warnings, and 0 unexpected "
          + "exceptions. First error: OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase" + Environment.NewLine + "See Log.GetResults() "
          + "for a full list of issues.", exception.Message);

      Assert.AreSame (log, exception.ValidationLog);
    }

    [Test]
    public void Merge ()
    {
      IValidationLog sourceLog = new DefaultValidationLog ();
      Exception exception = new Exception ();

      BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      BaseClassDefinition bt2 = TypeFactory.GetActiveConfiguration (typeof (BaseType2));
      BaseClassDefinition bt3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3));
      BaseClassDefinition bt4 = TypeFactory.GetActiveConfiguration (typeof (BaseType4));

      sourceLog.ValidationStartsFor (bt1);
      sourceLog.Succeed (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "Success", "Success"));
      sourceLog.Warn (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "Warn", "Warn"));
      sourceLog.Fail (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "Fail", "Fail"));
      sourceLog.UnexpectedException (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "Except", "Except"), exception);
      sourceLog.ValidationEndsFor (bt1);

      sourceLog.ValidationStartsFor (bt4);
      sourceLog.Succeed (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "Success2", "Success2"));
      sourceLog.Warn (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "Warn2", "Warn2"));
      sourceLog.Fail (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "Fail2", "Fail2"));
      sourceLog.UnexpectedException (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "Except2", "Except2"), exception);
      sourceLog.ValidationEndsFor (bt4);

      IValidationLog resultLog = new DefaultValidationLog ();
      resultLog.ValidationStartsFor (bt2);
      resultLog.Succeed (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "0", "0"));
      resultLog.Warn (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "1", "1"));
      resultLog.Fail (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "2", "2"));
      resultLog.UnexpectedException (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "3", "3"), exception);
      resultLog.ValidationEndsFor (bt2);

      resultLog.ValidationStartsFor (bt1);
      resultLog.Succeed (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "4", "4"));
      resultLog.Warn (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "5", "5"));
      resultLog.Fail (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "6", "6"));
      resultLog.UnexpectedException (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "7", "7"), exception);
      resultLog.ValidationEndsFor (bt1);

      resultLog.ValidationStartsFor (bt3);
      resultLog.Succeed (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "8", "8"));
      resultLog.Warn (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "9", "9"));
      resultLog.Fail (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "10", "10"));
      resultLog.UnexpectedException (new DelegateValidationRule<BaseClassDefinition> (delegate { }, "11", "11"), exception);
      resultLog.ValidationEndsFor (bt3);

      resultLog.MergeIn (sourceLog);
      Assert.AreEqual (5, resultLog.GetNumberOfSuccesses ());
      Assert.AreEqual (5, resultLog.GetNumberOfWarnings ());
      Assert.AreEqual (5, resultLog.GetNumberOfFailures ());
      Assert.AreEqual (5, resultLog.GetNumberOfUnexpectedExceptions ());

      List<ValidationResult> results = new List<ValidationResult> (resultLog.GetResults ());

      Assert.AreEqual (4, results.Count);

      Assert.AreEqual (bt2, results[0].Definition);
      Assert.AreEqual (1, results[0].Successes.Count);
      Assert.AreEqual (1, results[0].Failures.Count);
      Assert.AreEqual (1, results[0].Warnings.Count);
      Assert.AreEqual (1, results[0].Exceptions.Count);

      Assert.AreEqual (bt1, results[1].Definition);
      
      Assert.AreEqual (2, results[1].Successes.Count);
      Assert.AreEqual ("4", results[1].Successes[0].Message);
      Assert.AreEqual ("Success", results[1].Successes[1].Message);

      Assert.AreEqual (2, results[1].Warnings.Count);
      Assert.AreEqual ("5", results[1].Warnings[0].Message);
      Assert.AreEqual ("Warn", results[1].Warnings[1].Message);
      
      Assert.AreEqual (2, results[1].Failures.Count);
      Assert.AreEqual ("6", results[1].Failures[0].Message);
      Assert.AreEqual ("Fail", results[1].Failures[1].Message);

      Assert.AreEqual (2, results[1].Exceptions.Count);
      Assert.AreEqual (exception, results[1].Exceptions[0].Exception);
      Assert.AreEqual (exception, results[1].Exceptions[1].Exception);

      Assert.AreEqual (bt3, results[2].Definition);
      Assert.AreEqual (1, results[2].Successes.Count);
      Assert.AreEqual (1, results[2].Failures.Count);
      Assert.AreEqual (1, results[2].Warnings.Count);
      Assert.AreEqual (1, results[2].Exceptions.Count);

      Assert.AreEqual (bt4, results[3].Definition);

      Assert.AreEqual (1, results[3].Successes.Count);
      Assert.AreEqual ("Success2", results[3].Successes[0].Message);

      Assert.AreEqual (1, results[3].Warnings.Count);
      Assert.AreEqual ("Warn2", results[3].Warnings[0].Message);

      Assert.AreEqual (1, results[3].Failures.Count);
      Assert.AreEqual ("Fail2", results[3].Failures[0].Message);

      Assert.AreEqual (1, results[3].Exceptions.Count);
      Assert.AreEqual (exception, results[3].Exceptions[0].Exception);

    }
  }
}
