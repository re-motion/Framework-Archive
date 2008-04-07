using System;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.SampleTypes;

namespace Remotion.Mixins.UnitTests.CodeGeneration
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