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
    public void Initialize_WithSecurityContext ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));
      ISecurityContextFactory factory = new FunctionalSecurityContextFactory (context);

      Assert.AreSame (context, factory.GetSecurityContext ());
    }

    [Test]
    public void Initialize_WithType ()
    {
      ISecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));

      SecurityContext context = factory.GetSecurityContext ();
      Assert.IsNotNull (context);
      Assert.AreEqual ("Rubicon.Security.UnitTests.SampleDomain.SecurableObject, Rubicon.Security.UnitTests", context.Class);
    }

  }
}