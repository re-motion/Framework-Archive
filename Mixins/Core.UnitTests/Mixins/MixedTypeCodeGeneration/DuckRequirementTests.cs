using System;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins.MixedTypeCodeGeneration
{
  [TestFixture]
  public class DuckRequirementTests : MixinBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsRequiredDuckInterfaces ()
    {
      ClassFulfillingAllMemberRequirementsDuck cfrd = ObjectFactory.Create<ClassFulfillingAllMemberRequirementsDuck> ().With ();
      Assert.IsTrue (cfrd is IMixinRequiringAllMembersRequirements);
      MixinRequiringAllMembersFace mixin = Mixin.Get<MixinRequiringAllMembersFace> (cfrd);
      Assert.IsNotNull (mixin);
      Assert.AreEqual (42, mixin.PropertyViaThis);
    }

    [Test]
    public void RequiredFaceInterfaceViaDuck ()
    {
      ClassFulfillingAllMemberRequirementsExplicitly cfamre = ObjectFactory.Create<ClassFulfillingAllMemberRequirementsExplicitly> ().With ();
      MixinRequiringAllMembersFace mixin = Mixin.Get<MixinRequiringAllMembersFace> (cfamre);
      Assert.IsNotNull (mixin);
      Assert.AreEqual (37, mixin.PropertyViaThis);
    }

    [Test]
    public void RequiredBaseInterfaceViaDuck ()
    {
      ClassFulfillingAllMemberRequirements cfamr = ObjectFactory.Create<ClassFulfillingAllMemberRequirements> ().With ();
      MixinRequiringAllMembersBase mixin = Mixin.Get<MixinRequiringAllMembersBase> (cfamr);
      Assert.IsNotNull (mixin);
      Assert.AreEqual (11, mixin.PropertyViaBase);
    }

    [Test]
    public void ThisCallToDuckInterface ()
    {
      BaseTypeWithDuckFaceMixin duckFace = ObjectFactory.Create<BaseTypeWithDuckFaceMixin> ().With ();
      Assert.AreEqual ("DuckFaceMixin.CallMethodsOnThis-DuckFaceMixin.MethodImplementedOnBase-BaseTypeWithDuckFaceMixin.ProtectedMethodImplementedOnBase",
          Mixin.Get<DuckFaceMixin> (duckFace).CallMethodsOnThis ());
    }

  }
}