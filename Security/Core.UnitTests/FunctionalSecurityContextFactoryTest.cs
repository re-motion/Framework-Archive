using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests
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
      Assert.AreEqual ("Rubicon.Security.UnitTests.SampleDomain.SecurableObject, Rubicon.Security.UnitTests", context.Class);
    }

  }
}