using System;
using System.Collections.Generic;
using System.Reflection;
using Mixins.Context;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using Mixins.Validation.DefaultLog;
using NUnit.Framework;
using Mixins.Definitions;
using Mixins.Validation;
using Mixins.Validation.Rules;

namespace Mixins.UnitTests
{
  [TestFixture]
  public class ValidationTests
  {
    private class ThrowingRuleSet : IRuleSet
    {
      public void Install (ValidatingVisitor visitor)
      {
        visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (delegate { throw new InvalidOperationException (); }));
      }
    }

    private class DoubleImplementer : IBaseType2
    {
      public string IfcMethod ()
      {
        throw new Exception ("The method or operation is not implemented.");
      }
    }

    private class MixinWithClassBase : Mixin<BaseType1, BaseType1>
    {
    }


    private static ApplicationDefinition GetApplicationDefinitionForAssembly ()
    {
      ApplicationContext assemblyContext = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      return DefinitionBuilder.CreateApplicationDefinition (assemblyContext);
    }

    public static void Main ()
    {
      ValidationTests t = new ValidationTests ();
      t.ValidationDump ();
    }

    public static int TotalFailures (IList<DefaultValidationResult> results)
    {
      int sum = 0;
      foreach (DefaultValidationResult result in results)
      {
        sum += result.Failures.Count;
      }
      return sum;
    }

    public static int TotalWarnings (IList<DefaultValidationResult> results)
    {
      int sum = 0;
      foreach (DefaultValidationResult result in results)
      {
        sum += result.Warnings.Count;
      }
      return sum;
    }

    public static int TotalSuccesses (IList<DefaultValidationResult> results)
    {
      int sum = 0;
      foreach (DefaultValidationResult result in results)
      {
        sum += result.Successes.Count;
      }
      return sum;
    }

    public static bool HasFailure (string ruleName, DefaultValidationLog log)
    {
      foreach (DefaultValidationResult result in log.Results)
      {
        foreach (DefaultValidationResultItem item in result.Failures)
        {
          if (item.Rule.RuleName == ruleName)
          {
            return true;
          }
        }
      }
      return false;
    }

    public static bool HasWarning (string ruleName, DefaultValidationLog log)
    {
      foreach (DefaultValidationResult result in log.Results)
      {
        foreach (DefaultValidationResultItem item in result.Warnings)
        {
          if (item.Rule.RuleName == ruleName)
          {
            return true;
          }
        }
      }
      return false;
    }

    [Test]
    public void ValidationVisitsSomething ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly ();
      DefaultValidationLog log = Validator.Validate (application);

      Assert.IsTrue (log.Results.Count > 1);
    }

    [Test]
    public void ValidationDump ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly();

      DefaultValidationLog log = Validator.Validate (application);
      ConsoleDumper.DumpLog (log);
    }

    [Test]
    public void ValidationResultDefinition ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly ();

      DefaultValidationLog log = Validator.Validate (application);
      Assert.AreSame (application, log.Results[0].Definition);
      Assert.AreEqual ("<application>", log.Results[0].Definition.FullName);
    }

    [Test]
    public void AllIsValid ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly ();
      DefaultValidationLog log = Validator.Validate (application);
      foreach (DefaultValidationResult result in log.Results)
      {
        Assert.AreEqual (0, result.Exceptions.Count);
        Assert.AreEqual (0, result.Failures.Count);
        Assert.AreEqual (0, result.Warnings.Count);
      }
    }

    [Test]
    public void AllIsVisitedOnce ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly ();
      DefaultValidationLog log = Validator.Validate (application);
      
      Dictionary<IVisitableDefinition, IVisitableDefinition> visitedDefinitions = new Dictionary<IVisitableDefinition, IVisitableDefinition> ();
      foreach (DefaultValidationResult result in log.Results)
      {
        Assert.IsNotNull (result.Definition);
        Assert.IsFalse(visitedDefinitions.ContainsKey(result.Definition));
        visitedDefinitions.Add (result.Definition, result.Definition);
      }

      Assert.IsTrue (visitedDefinitions.ContainsKey (application));
      
      BaseClassDefinition bt1 = application.BaseClasses[typeof (BaseType1)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1));
      BaseClassDefinition bt2 = application.BaseClasses[typeof (IBaseType2)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt2));
      BaseClassDefinition bt3 = application.BaseClasses[typeof (BaseType3)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3));

      MixinDefinition bt1m1 = bt1.Mixins[typeof (BT1Mixin1)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1m1));
      MixinDefinition bt1m2 = bt1.Mixins[typeof (BT1Mixin2)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1m2));
      MixinDefinition bt2m1 = bt2.Mixins[typeof (BT2Mixin1)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt2m1));
      MixinDefinition bt3m1 = bt3.Mixins[typeof (BT3Mixin1)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m1));
      MixinDefinition bt3m2 = bt3.Mixins[typeof (BT3Mixin2)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m2));
      MixinDefinition bt3m3 = bt3.Mixins[typeof (BT3Mixin3<,>)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m3));
      MixinDefinition bt3m4 = bt3.Mixins[typeof (BT3Mixin4)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m4));
      MixinDefinition bt3m5 = bt3.Mixins[typeof (BT3Mixin5)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m5));

      MemberDefinition m1 = bt1.Members[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (m1));
      MemberDefinition m2 = bt1.Members[typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) })];
      Assert.IsTrue (visitedDefinitions.ContainsKey (m2));
      MemberDefinition m3 = bt1m1.Members[typeof (BT1Mixin1).GetMethod ("VirtualMethod")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (m3));
      MemberDefinition m4 = bt1m1.Members[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")];
      Assert.IsTrue (visitedDefinitions.ContainsKey (m4));

      InterfaceIntroductionDefinition i1 = bt1m1.InterfaceIntroductions[typeof (IBT1Mixin1)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (i1));

      RequiredBaseCallTypeDefinition bc1 = bt3.RequiredBaseCallTypes[typeof (IBaseType34)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bc1));

      RequiredFaceTypeDefinition ft1 = bt3.RequiredFaceTypes[typeof(IBaseType32)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (ft1));

      ThisDependencyDefinition td1 = bt3m1.ThisDependencies[typeof (IBaseType31)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (td1));

      BaseDependencyDefinition bd1 = bt3m1.BaseDependencies[typeof (IBaseType31)];
      Assert.IsTrue (visitedDefinitions.ContainsKey (bd1));
    }

    [Test]
    public void HasDefaultRules ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly ();
      DefaultValidationLog log = Validator.Validate (application);
      Assert.IsTrue (log.Results[0].TotalRulesExecuted > 0);
    }

    [Test]
    public void CollectsUnexpectedExceptions ()
    {
      BaseClassDefinition bc = DefBuilder.Build (typeof (DateTime)).BaseClasses[typeof (DateTime)];
      DefaultValidationLog log = Validator.Validate (bc, new ThrowingRuleSet ());
      Assert.IsTrue (log.Results[0].Exceptions.Count > 0);
      Assert.IsTrue (log.Results[0].Exceptions[0].Exception is InvalidOperationException);
    }

    [Test]
    public void WarnsIfEmptyApplication ()
    {
      ApplicationDefinition application = new ApplicationDefinition ();
      DefaultValidationLog log = Validator.Validate (application);
      Assert.IsTrue (HasWarning ("Mixins.Validation.Rules.DefaultApplicationRules.ApplicationShouldContainAtLeastOneBaseClass", log));
      Assert.AreEqual (0, TotalFailures(log.Results));
    }

    [Test]
    public void FailsIfSealedBaseClass ()
    {
      BaseClassDefinition bc = DefBuilder.Build (typeof (DateTime)).BaseClasses[typeof (DateTime)];
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultBaseClassRules.BaseClassMustNotBeSealed", log));
      Assert.AreEqual (0, TotalWarnings (log.Results));
    }

    [Test]
    public void FailsIfOverriddenMethodNotVirtual ()
    {
      ApplicationDefinition application = DefBuilder.Build(typeof(BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType4)].Members[typeof(BaseType4).GetMethod("NonVirtualMethod")]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfMixinIsInterface ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (IBT1Mixin1));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType1)].Mixins[typeof (IBT1Mixin1)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMixinRules.MixinCannotBeInterface", log));
    }

    [Test]
    public void WarnsIfIntroducedInterfaceAlreadyImplemented ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType2), typeof (DoubleImplementer));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType2)].Mixins[typeof (DoubleImplementer)].InterfaceIntroductions[typeof (IBaseType2)]);

      Assert.IsTrue (HasWarning ("Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.InterfaceShouldNotBeImplementedTwice", log));
    }

    [Test]
    public void FailsIfRequiredFaceClassNotAvailable ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (BT3Mixin4));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType1)].RequiredFaceTypes[typeof (BaseType3)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceClassMustBeAssignableFromTargetType", log));
    }

    [Test]
    public void FailsIfRequiredFaceInterfaceNotAvailable ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (BT3Mixin2));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType1)].RequiredFaceTypes[typeof (IBaseType32)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceInterfaceMustBeIntroducedOrImplemented", log));
    }

    [Test]
    public void FailsIfRequiredBaseIsNotInterface ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (MixinWithClassBase));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType1)].RequiredBaseCallTypes[typeof (BaseType1)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredBaseCallTypeRules.BaseCallTypeMustBeInterface", log));
    }

    [Test]
    public void FailsIfRequiredBaseNotAvailable ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (BT3Mixin1));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType1)].RequiredBaseCallTypes[typeof (IBaseType31)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredBaseCallTypeRules.BaseCallTypeMustBeIntroducedOrImplemented", log));
    }

    [Test]
    public void FailsIfThisDependencyNotFulfilled ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin6<,>));
      DefaultValidationLog log = Validator.Validate(application.BaseClasses[typeof(BaseType3)].Mixins[typeof(BT3Mixin6<,>)].ThisDependencies[typeof(IBT3Mixin4)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultThisDependencyRules.DependencyMustBeSatisfied", log));
    }

    [Test]
    public void FailsIfBaseDependencyNotFulfilled ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin6<,>));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin6<,>)].BaseDependencies[typeof (IBT3Mixin4)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }

    [Test]
    public void FailsIfMixinNonPublic ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin2));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType5)].Mixins[typeof (BT5Mixin2)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void SucceedsIfSelfDependency ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin3));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType5)].Mixins[typeof (BT5Mixin3)].ThisDependencies[typeof(IBT5Mixin3)]);

      Assert.AreEqual (0, TotalFailures (log.Results));
      Assert.AreEqual (0, TotalWarnings (log.Results));

      log = Validator.Validate (application.BaseClasses[typeof (BaseType5)].Mixins[typeof (BT5Mixin3)].BaseDependencies[typeof (IBT5Mixin3)]);

      Assert.AreEqual (0, TotalFailures (log.Results));
      Assert.AreEqual (0, TotalWarnings (log.Results));
    }

    [Test]
    public void FailsIfCircularDependency ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType5), typeof (BT5MixinC1), typeof(BT5MixinC2));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses[typeof (BaseType5)].Mixins[typeof (BT5MixinC2)].ThisDependencies[typeof (IBT5MixinC1)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultThisDependencyRules.NoCircularDependencies", log));

      log = Validator.Validate (application.BaseClasses[typeof (BaseType5)].Mixins[typeof (BT5MixinC2)].BaseDependencies[typeof (IBT5MixinC1)]);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultBaseDependencyRules.NoCircularDependencies", log));
    }

    [Test]
    public void SucceedsIfAggregateDependencyIsFullyImplemented ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7));
      DefaultValidationLog log = Validator.Validate (application);

      Assert.AreEqual (0, TotalFailures(log.Results));
      Assert.AreEqual (0, TotalWarnings (log.Results));
    }

    [Test]
    public void FailsIfAggregateDependencyIsNotFullyImplemented ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin7));
      DefaultValidationLog log = Validator.Validate (application);

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultThisDependencyRules.DependencyMustBeSatisfied", log));
      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceInterfaceMustBeIntroducedOrImplemented", log));

      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
      Assert.IsTrue (HasFailure ("Mixins.Validation.Rules.DefaultRequiredBaseCallTypeRules.BaseCallTypeMustBeIntroducedOrImplemented", log));
    }
   
  }
}
