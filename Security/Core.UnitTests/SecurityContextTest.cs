using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Utilities;

using Rubicon.Security.UnitTests.Domain;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityContextTest
  {
    [Test]
    public void CreateSecurityContextWithAbstractRole ()
    {
      Enum[] abstractRoles = new Enum[] { TestAbstractRole.QualityEngineer, TestAbstractRole.Developer };
      SecurityContext context = new SecurityContext (typeof (File), "owner", "group", "client", null, abstractRoles);

      Assert.AreEqual (2, context.AbstractRoles.Length);
      Assert.Contains (new EnumWrapper (TestAbstractRole.QualityEngineer), context.AbstractRoles);
      Assert.Contains (new EnumWrapper (TestAbstractRole.Developer), context.AbstractRoles);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       "Enumerated Type 'Rubicon.Security.UnitTests.Domain.SimpleEnum' cannot be used as an abstract role. "
        + "Valid abstract roles must have the Rubicon.Security.AbstractRoleAttribute applied.\r\nParameter name: abstractRoles")]
    public void CreateSecurityContextWithInvalidAbstractRole ()
    {
      Enum[] abstractRoles = new Enum[] { SimpleEnum.First };
      SecurityContext context = new SecurityContext (typeof (File), "owner", "group", "client", null, abstractRoles);
    }

    [Test]
    public void CreateSecurityContextWithNullAbstractRoles ()
    {
      SecurityContext context = new SecurityContext (typeof (File), "owner", "group", "client", null, null);
      Assert.AreEqual (0, context.AbstractRoles.Length);
    }

    [Test]
    public void CreateSecurityContextWithState ()
    {
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum> ();
      testStates.Add ("Confidentiality", TestSecurityState.Public);
      testStates.Add ("State", TestSecurityState.Secret);

      SecurityContext context = new SecurityContext (typeof (File), "owner", "group", "client", testStates, null);

      Assert.AreEqual (new EnumWrapper (TestSecurityState.Public), context.GetState ("Confidentiality"));
      Assert.AreEqual (new EnumWrapper (TestSecurityState.Secret), context.GetState ("State"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "Enumerated Type 'Rubicon.Security.UnitTests.Domain.SimpleEnum' cannot be used as a security state. "
        + "Valid security states must have the Rubicon.Security.SecurityStateAttribute applied.\r\nParameter name: states")]
    public void CreateSecurityContextWithInvalidState ()
    {
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum> ();
      testStates.Add ("Confidentiality", TestSecurityState.Public);
      testStates.Add ("State", SimpleEnum.Second);

      SecurityContext context = new SecurityContext (typeof (File), "owner", "group", "client", testStates, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void CreateSecurityContextWithInvalidType ()
    {
      SecurityContext context = new SecurityContext (typeof (SimpleType), "owner", "group", "client", null, null);
    }

    [Test]
    public void GetClassName ()
    {
      SecurityContext context = new SecurityContext (typeof (File), "owner", "group", "client", null, null);
      Assert.AreEqual ("Rubicon.Security.UnitTests.Domain.File, Rubicon.Security.UnitTests", context.Class);
    }
  }
}
