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

    [Test]
    public void ValidationVisitsSomething ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly ();
      DefaultValidationLog log = Validator.Validate (application);

      Assert.IsTrue (log.Results.Count > 1);
    }

    // [Test]
    public void ValidationDump ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly();

      DefaultValidationLog log = Validator.Validate (application);
      ConsoleDumper.DumpLog (log);
    }

    public static void Main ()
    {
      ValidationTests t = new ValidationTests();
      t.ValidationDump ();
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
      
      BaseClassDefinition bt1 = application.BaseClasses.Get (typeof (BaseType1));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1));
      BaseClassDefinition bt2 = application.BaseClasses.Get (typeof (IBaseType2));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt2));
      BaseClassDefinition bt3 = application.BaseClasses.Get (typeof (BaseType3));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3));

      MixinDefinition bt1m1 = bt1.Mixins.Get (typeof (BT1Mixin1));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1m1));
      MixinDefinition bt1m2 = bt1.Mixins.Get (typeof (BT1Mixin2));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt1m2));
      MixinDefinition bt2m1 = bt2.Mixins.Get (typeof (BT2Mixin1));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt2m1));
      MixinDefinition bt3m1 = bt3.Mixins.Get (typeof (BT3Mixin1));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m1));
      MixinDefinition bt3m2 = bt3.Mixins.Get (typeof (BT3Mixin2));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m2));
      MixinDefinition bt3m3 = bt3.Mixins.Get (typeof (BT3Mixin3<,>));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m3));
      MixinDefinition bt3m4 = bt3.Mixins.Get (typeof (BT3Mixin4));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m4));
      MixinDefinition bt3m5 = bt3.Mixins.Get (typeof (BT3Mixin5));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bt3m5));

      MemberDefinition m1 = bt1.Members.Get (typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes));
      Assert.IsTrue (visitedDefinitions.ContainsKey (m1));
      MemberDefinition m2 = bt1.Members.Get (typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) }));
      Assert.IsTrue (visitedDefinitions.ContainsKey (m2));
      MemberDefinition m3 = bt1m1.Members.Get (typeof (BT1Mixin1).GetMethod ("VirtualMethod"));
      Assert.IsTrue (visitedDefinitions.ContainsKey (m3));
      MemberDefinition m4 = bt1m1.Members.Get (typeof (BT1Mixin1).GetMethod ("IntroducedMethod"));
      Assert.IsTrue (visitedDefinitions.ContainsKey (m4));

      InterfaceIntroductionDefinition i1 = bt1m1.InterfaceIntroductions.Get (typeof (IBT1Mixin1));
      Assert.IsTrue (visitedDefinitions.ContainsKey (i1));

      RequiredBaseCallTypeDefinition bc1 = bt3.RequiredBaseCallTypes.Get (typeof (IBaseType34));
      Assert.IsTrue (visitedDefinitions.ContainsKey (bc1));

      RequiredFaceTypeDefinition ft1 = bt3.RequiredFaceTypes.Get(typeof(IBaseType32));
      Assert.IsTrue (visitedDefinitions.ContainsKey (ft1));
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
      BaseClassDefinition bc = new BaseClassDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc, new ThrowingRuleSet());
      Assert.IsTrue (log.Results[0].Exceptions.Count > 0);
      Assert.IsTrue (log.Results[0].Exceptions[0].Exception is InvalidOperationException);
    }

    [Test]
    public void WarnsIfEmptyApplication ()
    {
      ApplicationDefinition application = new ApplicationDefinition ();
      DefaultValidationLog log = Validator.Validate (application);
      Assert.IsTrue (log.Results[0].Warnings.Count > 0);
      Assert.AreEqual ("Mixins.Validation.Rules.DefaultApplicationRules.ApplicationShouldContainAtLeastOneBaseClass", log.Results[0].Warnings[0].Rule.RuleName);
    }

    [Test]
    public void FailsIfSealedBaseClass ()
    {
      BaseClassDefinition bc = new BaseClassDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (log.Results[0].Failures.Count > 0);
      Assert.AreEqual ("Mixins.Validation.Rules.DefaultBaseClassRules.BaseClassMustNotBeSealed", log.Results[0].Failures[0].Rule.RuleName);
    }

    [Test]
    public void FailsIfOverriddenMethodNotVirtual ()
    {
      ApplicationDefinition application = DefBuilder.Build(typeof(BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses.Get(typeof (BaseType4)).Members.Get(typeof(BaseType4).GetMethod("NonVirtualMethod")));

      Assert.IsTrue (log.Results[0].Failures.Count > 0);
      Assert.AreEqual ("Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log.Results[0].Failures[0].Rule.RuleName);
    }

    [Test]
    public void FailsIfMixinIsInterface ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (IBT1Mixin1));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses.Get (typeof (BaseType1)).Mixins.Get(typeof (IBT1Mixin1)));

      Assert.IsTrue (log.Results[0].Failures.Count > 0);
      Assert.AreEqual ("Mixins.Validation.Rules.DefaultMixinRules.MixinCannotBeInterface", log.Results[0].Failures[0].Rule.RuleName);
    }

    [Test]
    public void WarnsIfIntroducedInterfaceAlreadyImplemented ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType2), typeof (DoubleImplementer));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses.Get (typeof (BaseType2)).Mixins.Get (typeof (DoubleImplementer)).InterfaceIntroductions.Get (typeof (IBaseType2)));

      Assert.IsTrue (log.Results[0].Warnings.Count > 0);
      Assert.AreEqual ("Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.InterfaceShouldNotBeImplementedTwice", log.Results[0].Warnings[0].Rule.RuleName);
    }

    [Test]
    public void FailsIfRequiredFaceClassNotAvailable ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (BT3Mixin4));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses.Get (typeof (BaseType1)).RequiredFaceTypes.Get (typeof (BaseType3)));

      Assert.AreEqual ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceClassMustBeAssignableFromTargetType", log.Results[0].Failures[0].Rule.RuleName);
    }

    [Test]
    public void FailsIfRequiredFaceInterfaceNotAvailable ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (BT3Mixin2));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses.Get (typeof (BaseType1)).RequiredFaceTypes.Get (typeof (IBaseType32)));

      Assert.AreEqual ("Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceInterfaceMustBeIntroducedOrImplemented", log.Results[0].Failures[0].Rule.RuleName);
    }

    [Test]
    public void FailsIfRequiredBaseIsNotInterface ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (MixinWithClassBase));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses.Get (typeof (BaseType1)).RequiredBaseCallTypes.Get (typeof (BaseType1)));

      Assert.AreEqual ("Mixins.Validation.Rules.DefaultRequiredBaseCallTypeRules.BaseCallTypeMustBeInterface", log.Results[0].Failures[0].Rule.RuleName);
    }

    [Test]
    public void FailsIfRequiredBaseNotAvailable ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType1), typeof (BT3Mixin1));
      DefaultValidationLog log = Validator.Validate (application.BaseClasses.Get (typeof (BaseType1)).RequiredBaseCallTypes.Get (typeof (IBaseType31)));

      Assert.AreEqual ("Mixins.Validation.Rules.DefaultRequiredBaseCallTypeRules.BaseCallTypeMustBeIntroducedOrImplemented", log.Results[0].Failures[0].Rule.RuleName);
    }
  }
}
