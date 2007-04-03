using System;
using System.Collections.Generic;
using System.Reflection;
using Mixins.Context;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.Definitions;
using Mixins.Validation;

namespace Mixins.UnitTests
{
  [TestFixture]
  public class ValidationTests
  {
    private static ApplicationDefinition GetApplicationDefinitionForAssembly ()
    {
      ApplicationContext assemblyContext = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      return DefinitionBuilder.CreateApplicationDefinition (assemblyContext);
    }

    private static DefaultValidationLog Verify (ApplicationDefinition application)
    {
      DefaultValidationLog log = new DefaultValidationLog ();
      ValidatingVisitor visitor = new ValidatingVisitor (log);
      application.Accept (visitor);
      return log;
    }

    [Test]
    public void ValidationVisitsSomething ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly ();
      DefaultValidationLog log = Verify (application);

      List<DefaultValidationLog.ValidationData> results = new List<DefaultValidationLog.ValidationData> (log.Results);
      Assert.IsNotEmpty (results);
      Assert.IsTrue (results.Count > 1);
    }

    // [Test]
    public void ValidationDump ()
    {
      ApplicationDefinition application = GetApplicationDefinitionForAssembly();

      DefaultValidationLog log = Verify(application);
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
      DefaultValidationLog log = Verify (application);
      foreach (DefaultValidationLog.ValidationData result in log.Results)
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
      DefaultValidationLog log = Verify (application);
      
      Dictionary<IVisitableDefinition, IVisitableDefinition> visitedDefinitions = new Dictionary<IVisitableDefinition, IVisitableDefinition> ();
      foreach (DefaultValidationLog.ValidationData result in log.Results)
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
      List<DefaultValidationLog.ValidationData> results = new List<DefaultValidationLog.ValidationData> (log.Results);
      Assert.IsTrue (results[0].TotalRulesExecuted > 0);
    }
  }
}
