using System;
using System.Collections.Generic;
using System.Reflection;
using Mixins.Context;
using Mixins.Definitions.Building;
using Mixins.UnitTests.Configuration.ValidationSampleTypes;
using Mixins.UnitTests.Mixins;
using Mixins.UnitTests.SampleTypes;
using Mixins.Validation;
using NUnit.Framework;
using Mixins.Definitions;

namespace Mixins.UnitTests.Configuration
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

    private IEnumerable<IVisitableDefinition> BuildAllBaseDefinitions ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
      {
        ApplicationContext applicationContext = MixinConfiguration.ActiveContext;
        BaseClassDefinitionBuilder builder = new BaseClassDefinitionBuilder ();
        foreach (ClassContext context in applicationContext.ClassContexts)
          yield return builder.Build (context);
      }
    }

    [Test]
    public void ValidationVisitsSomething ()
    {
      IEnumerable<IVisitableDefinition> definitions = BuildAllBaseDefinitions();
      DefaultValidationLog log = Validator.Validate (definitions);

      Assert.IsTrue (log.ResultCount > 1);
    }

    [Test]
    public void ValidationDump ()
    {
      IEnumerable<IVisitableDefinition> definitions = BuildAllBaseDefinitions();
      DefaultValidationLog log = Validator.Validate (definitions);
      ConsoleDumper.DumpValidationResults (log.GetResults());
    }

    [Test]
    public void ValidationResultDefinition ()
    {
      List<IVisitableDefinition> definitions = new List<IVisitableDefinition> (BuildAllBaseDefinitions ());
      DefaultValidationLog log = Validator.Validate (definitions);

      IEnumerator<ValidationResult> results = log.GetResults().GetEnumerator();
      Assert.IsTrue (results.MoveNext());
      ValidationResult firstResult = results.Current;
      Assert.AreSame (definitions[0], firstResult.Definition);
    }

    [Test]
    public void AllIsValid ()
    {
      IEnumerable<IVisitableDefinition> definitions = BuildAllBaseDefinitions();
      DefaultValidationLog log = Validator.Validate (definitions);
      Assert.AreEqual (0, log.GetNumberOfFailures());
      Assert.AreEqual (0, log.GetNumberOfWarnings());
      Assert.AreEqual (0, log.GetNumberOfUnexpectedExceptions());
    }

    [Test]
    public void AllIsVisitedOnce ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly()))
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

        RequiredBaseCallTypeDefinition bc1 = bt3.RequiredBaseCallTypes[typeof (IBaseType34)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bc1));
        RequiredBaseCallMethodDefinition bcm1 = bc1.BaseCallMethods[typeof (IBaseType34).GetMethod ("IfcMethod")];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bcm1));

        RequiredFaceTypeDefinition ft1 = bt3.RequiredFaceTypes[typeof (IBaseType32)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (ft1));

        ThisDependencyDefinition td1 = bt3m1.ThisDependencies[typeof (IBaseType31)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (td1));

        BaseDependencyDefinition bd1 = bt3m1.BaseDependencies[typeof (IBaseType31)];
        Assert.IsTrue (visitedDefinitions.ContainsKey (bd1));
      }
    }

    [Test]
    public void HasDefaultRules ()
    {
      IEnumerable<IVisitableDefinition> definitions = BuildAllBaseDefinitions();
      DefaultValidationLog log = Validator.Validate (definitions);
      Assert.IsTrue (log.GetNumberOfRulesExecuted() > 0);
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
      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultBaseClassRules.BaseClassMustNotBeSealed", log));
      Assert.AreEqual (0, log.GetNumberOfWarnings());
    }

    [Test]
    public void FailsIfOverriddenMethodNotVirtual ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Methods[typeof (BaseType4).GetMethod ("NonVirtualMethod")]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfOverriddenMethodFinal ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithFinalMethod), typeof (MixinForFinalMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustNotBeFinal", log));
    }

    [Test]
    public void FailsIfOverriddenPropertyMethodNotVirtual ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Properties[typeof (BaseType4).GetProperty ("NonVirtualProperty")]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfOverriddenEventMethodNotVirtual ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Events[typeof (BaseType4).GetEvent ("NonVirtualEvent")]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void WarnsIfPropertyOverrideAddsMethods ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseWithGetterOnly), typeof (MixinOverridingSetterOnly));
      DefaultValidationLog log =
          Validator.Validate (definition.Properties[typeof (BaseWithGetterOnly).GetProperty ("Property")].Overrides[0]);

      Assert.IsTrue (HasWarning ("Mixins.Validation.Rules.DefaultPropertyRules.NewMemberAddedByOverride", log));
    }

    [Test]
    public void FailsIfMixinIsInterface ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (IBT1Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (IBT1Mixin1)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMixinRules.MixinCannotBeInterface", log));
    }

    [Test]
    public void WarnsIfIntroducedInterfaceAlreadyImplemented ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (DoubleImplementer));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (DoubleImplementer)].InterfaceIntroductions[typeof (IBaseType2)]);

      Assert.IsTrue (HasWarning ("Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.InterfaceWillShadowBaseClassInterface", log));
    }

    [Test]
    public void FailsIfIntroducedInterfaceNotVisible ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (MixinIntroducingInternalInterface));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.IntroducedInterfaceMustBePublic", log));
    }

    [Test]
    public void FailsIfRequiredFaceClassNotAvailable ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithClassThisDependency));
      DefaultValidationLog log = Validator.Validate (definition.RequiredFaceTypes[typeof (BaseType3)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceClassMustBeAssignableFromTargetType", log));
    }

    [Test]
    public void FailsIfRequiredFaceInterfaceNotAvailable ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (BT3Mixin2));
      DefaultValidationLog log = Validator.Validate (definition.RequiredFaceTypes[typeof (IBaseType32)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceInterfaceMustBeIntroducedOrImplemented", log));
    }

    [Test]
    public void FailsIfRequiredFaceTypeNotVisible ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithInvisibleThisDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.RequiredFaceTypeMustBePublic", log));
    }

    [Test]
    public void FailsIfThisDependencyNotFulfilled ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedThisDependency));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedThisDependency)].
              ThisDependencies[typeof (IServiceProvider)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultThisDependencyRules.DependencyMustBeSatisfied", log));
    }

    [Test]
    public void FailsIfMixinNonPublic ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin2));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (BT5Mixin2)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void SucceedsIfAggregateThisDependencyIsFullyImplemented ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Face));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.AreEqual (0, log.GetNumberOfFailures());
      Assert.AreEqual (0, log.GetNumberOfWarnings());
    }

    [Test]
    public void FailsIfAggregateThisDependencyIsNotFullyImplemented ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Face));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultThisDependencyRules.DependencyMustBeSatisfied", log));
      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceInterfaceMustBeIntroducedOrImplemented", log));
    }

    [Test]
    public void SucceedsIfAggregateBaseDependencyIsFullyImplemented ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.AreEqual (0, log.GetNumberOfFailures());
      Assert.AreEqual (0, log.GetNumberOfWarnings());
    }

    [Test]
    public void FailsIfImplementingIMixinTarget ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinImplementingIMixinTarget));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.IMixinTargetCannotBeIntroduced", log));
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixin ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinAddingBT1Attribute));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultAttributeRules.AllowMultipleRequired", log));
      Assert.AreEqual (2, log.GetNumberOfFailures());
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixinToMember ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinAddingBT1AttributeToMember));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultAttributeRules.AllowMultipleRequired", log));
      Assert.AreEqual (2, log.GetNumberOfFailures());
    }

    [Test]
    public void SucceedsIfDuplicateAttributeAddedByMixinAllowsMultiple ()
    {
      BaseClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseTypeWithAllowMultiple), typeof (MixinAddingAllowMultipleToClassAndMember));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.AreEqual (0, log.GetNumberOfFailures());
      Assert.AreEqual (0, log.GetNumberOfWarnings());
    }

    [Test]
    public void SucceedsIfNestedPublicMixin ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.PublicNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.AreEqual (0, log.GetNumberOfFailures());
      Assert.AreEqual (0, log.GetNumberOfWarnings());
    }

    [Test]
    public void FailsIfNestedPublicMixinInNonPublic ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.PublicNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNestedPrivateMixin ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.InternalNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNestedPrivateMixinInNonPublic ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.InternalNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfOverriddenMixinMethodNotVirtual ()
    {
      BaseClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethod), typeof (MixinWithNonVirtualMethodToBeOverridden));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfAbstractMixinMethodHasNoOverride ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (AbstractMixin));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.AbstractMethodMustBeOverridden", log));
    }

    [Test]
    public void FailsIfCrossOverridesOnSameMethods ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethod), typeof (MixinOverridingSameClassMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.NoCircularOverrides", log));
    }

    [Test]
    public void SucceedsIfCrossOverridesNotOnSameMethods ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethod), typeof (MixinOverridingClassMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.AreEqual (0, log.GetNumberOfFailures());
      Assert.AreEqual (0, log.GetNumberOfWarnings());
    }

    [Test]
    public void FailsIfBaseClassDefinitionIsInterface ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (IBaseType2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultBaseClassRules.BaseClassMustNotBeAnInterface", log));
    }

    [Test]
    public void FailsIfMixinMethodIsOverriddenWhichHasNoThisProperty ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethod), typeof (AbstractMixinWithoutBase));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase", log));
    }

    [Test]
    public void ValidationException ()
    {
      BaseClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethod), typeof (AbstractMixinWithoutBase));
      DefaultValidationLog log = Validator.Validate (definition);

      ValidationException exception = new ValidationException (log);
      Assert.AreEqual ("Some parts of the mixin configuration could not be validated." + Environment.NewLine + "Mixins.UnitTests.Configuration."
          + "ValidationSampleTypes.AbstractMixinWithoutBase.AbstractMethod (Mixins.UnitTests.Configuration.ValidationSampleTypes."
          + "AbstractMixinWithoutBase -> Mixins.UnitTests.SampleTypes.ClassOverridingMixinMethod): There were 1 errors, 0 warnings, and 0 unexpected "
          + "exceptions. First error: OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase" + Environment.NewLine + "See Log.GetResults() "
          + "for a full list of issues.", exception.Message);

      Assert.AreSame (log, exception.ValidationLog);
    }
  }
}
