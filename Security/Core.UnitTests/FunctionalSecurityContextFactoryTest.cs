using System;
using NUnit.Framework;
using Rubicon.Security.UnitTests.Core.SampleDomain;

namespace Rubicon.Security.UnitTests.Core
{
  [TestFixture]
  public class FunctionalSecurityContextFactoryTest
  {
    [Test]
    public void Initialize ()
    {
      ISecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));

      SecurityContext context = factory.CreateSecurityContext ();
      Assert.IsNotNull (context);
      Assert.AreEqual ("Rubicon.Security.UnitTests.Core.SampleDomain.SecurableObject, Rubicon.Security.UnitTests", context.Class);
    }

  }
}