using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Rubicon.Mixins.UnitTests.Configuration
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

      BaseClassDefinition def1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),typeof (BaseWithOverrideAttributes));
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

      BaseClassDefinition def1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
          typeof (DerivedWithoutOverrideAttributes));
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
      BaseClassDefinition def1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
          typeof (DerivedNewWithAdditionalOverrideAttributes));
    }

    [Test]
    public void BaseWithoutAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      BaseClassDefinition def1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
          typeof (BaseWithoutOverrideAttributes));
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

      BaseClassDefinition def1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
          typeof (DerivedNewWithOverrideAttributes));
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

      BaseClassDefinition def1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
          typeof (DerivedNewWithoutOverrideAttributes));
      MixinDefinition mix1 = def1.Mixins[typeof (DerivedNewWithoutOverrideAttributes)];
      Assert.IsNotNull (mix1);

      Assert.AreEqual (0, def1.Methods[method].Overrides.Count);
      Assert.AreEqual (0, def1.Properties[property].Overrides.Count);
      Assert.AreEqual (0, def1.Events[eve].Overrides.Count);
    }
  }
}
