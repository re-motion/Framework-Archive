using System;
using NUnit.Framework;
using Remotion.Core.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class IntegrationTest : CodeGenerationBaseTest
  {
    [Test]
    public void AlphabeticOrdering ()
    {
      ClassWithMixinsAcceptingAlphabeticOrdering instance = ObjectFactory.Create<ClassWithMixinsAcceptingAlphabeticOrdering>().With();
      Assert.AreEqual (
          "MixinAcceptingAlphabeticOrdering1.ToString-MixinAcceptingAlphabeticOrdering2.ToString-ClassWithMixinsAcceptingAlphabeticOrdering.ToString",
          instance.ToString());
    }
  }
}