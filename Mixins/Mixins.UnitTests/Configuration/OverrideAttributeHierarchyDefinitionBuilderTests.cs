using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class OverrideAttributeHierarchyDefinitionBuilderTests
  {
    [Test]
    public void BaseWithOverrideAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      BaseClassDefinition def1 = DefBuilder.Build (typeof (TargetForOverridesAndShadowing), typeof (BaseWithOverrideAttributes))
          .BaseClasses[typeof (TargetForOverridesAndShadowing)];
      MixinDefinition mix1 = def1.Mixins[typeof (BaseWithOverrideAttributes)];
      Assert.IsNotNull (mix1);

      Assert.AreEqual (1, def1.Methods[method].Overrides.Count);
      Assert.AreSame (mix1, def1.Methods[method].Overrides[typeof (BaseWithOverrideAttributes)].DeclaringClass);

      Assert.AreEqual (1, def1.Properties[property].Overrides.Count);
      Assert.AreSame (mix1, def1.Properties[property].Overrides[typeof (BaseWithOverrideAttributes)].DeclaringClass);

      Assert.AreEqual (1, def1.Events[eve].Overrides.Count);
      Assert.AreSame (mix1, def1.Events[eve].Overrides[typeof (BaseWithOverrideAttributes)].DeclaringClass);
    }

    [Test]
    public void DerivedWithOverridesWithoutAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      BaseClassDefinition def1 = DefBuilder.Build (typeof (TargetForOverridesAndShadowing), typeof (DerivedWithoutOverrideAttributes))
          .BaseClasses[typeof (TargetForOverridesAndShadowing)];
      MixinDefinition mix1 = def1.Mixins[typeof (DerivedWithoutOverrideAttributes)];
      Assert.IsNotNull (mix1);

      Assert.AreEqual (1, def1.Methods[method].Overrides.Count);
      Assert.AreSame (mix1, def1.Methods[method].Overrides[typeof (DerivedWithoutOverrideAttributes)].DeclaringClass);

      Assert.AreEqual (1, def1.Properties[property].Overrides.Count);
      Assert.AreSame (mix1, def1.Properties[property].Overrides[typeof (DerivedWithoutOverrideAttributes)].DeclaringClass);

      Assert.AreEqual (1, def1.Events[eve].Overrides.Count);
      Assert.AreSame (mix1, def1.Events[eve].Overrides[typeof (DerivedWithoutOverrideAttributes)].DeclaringClass);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Mixin .* overrides method .* twice", MatchType = MessageMatch.Regex)]
    public void DerivedWithNewAdditionalOverrides ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      BaseClassDefinition def1 = DefBuilder.Build (typeof (TargetForOverridesAndShadowing), typeof (DerivedNewWithAdditionalOverrideAttributes))
          .BaseClasses[typeof (TargetForOverridesAndShadowing)];
    }

    [Test]
    public void BaseWithoutAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      BaseClassDefinition def1 = DefBuilder.Build (typeof (TargetForOverridesAndShadowing), typeof (BaseWithoutOverrideAttributes))
          .BaseClasses[typeof (TargetForOverridesAndShadowing)];
      MixinDefinition mix1 = def1.Mixins[typeof (BaseWithoutOverrideAttributes)];
      Assert.IsNotNull (mix1);

      Assert.AreEqual (0, def1.Methods[method].Overrides.Count);
      Assert.AreEqual (0, def1.Properties[property].Overrides.Count);
      Assert.AreEqual (0, def1.Events[eve].Overrides.Count);
    }

    [Test]
    public void DerivedWithNewAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      BaseClassDefinition def1 = DefBuilder.Build (typeof (TargetForOverridesAndShadowing), typeof (DerivedNewWithOverrideAttributes))
          .BaseClasses[typeof (TargetForOverridesAndShadowing)];
      MixinDefinition mix1 = def1.Mixins[typeof (DerivedNewWithOverrideAttributes)];
      Assert.IsNotNull (mix1);

      Assert.AreEqual (1, def1.Methods[method].Overrides.Count);
      Assert.AreSame (mix1, def1.Methods[method].Overrides[typeof (DerivedNewWithOverrideAttributes)].DeclaringClass);
      Assert.AreEqual (1, def1.Properties[property].Overrides.Count);
      Assert.AreSame (mix1, def1.Properties[property].Overrides[typeof (DerivedNewWithOverrideAttributes)].DeclaringClass);
      Assert.AreEqual (1, def1.Events[eve].Overrides.Count);
      Assert.AreSame (mix1, def1.Events[eve].Overrides[typeof (DerivedNewWithOverrideAttributes)].DeclaringClass);
    }

    [Test]
    public void DerivedNewWithoutAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      BaseClassDefinition def1 = DefBuilder.Build (typeof (TargetForOverridesAndShadowing), typeof (DerivedNewWithoutOverrideAttributes))
          .BaseClasses[typeof (TargetForOverridesAndShadowing)];
      MixinDefinition mix1 = def1.Mixins[typeof (DerivedNewWithoutOverrideAttributes)];
      Assert.IsNotNull (mix1);

      Assert.AreEqual (0, def1.Methods[method].Overrides.Count);
      Assert.AreEqual (0, def1.Properties[property].Overrides.Count);
      Assert.AreEqual (0, def1.Events[eve].Overrides.Count);
    }
  }
}
