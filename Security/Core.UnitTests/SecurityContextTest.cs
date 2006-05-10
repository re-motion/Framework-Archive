using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

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
      SecurityContext context = new SecurityContext (
          "Rubicon.Security.UnitTests.SecurityContextTest", "owner", "group", "client", null, abstractRoles);

      Assert.AreEqual (2, context.AbstractRoles.Length);
      Assert.Contains ("Rubicon.Security.UnitTests.Domain.TestAbstractRole.QualityEngineer, Rubicon.Security.UnitTests", context.AbstractRoles);
      Assert.Contains ("Rubicon.Security.UnitTests.Domain.TestAbstractRole.Developer, Rubicon.Security.UnitTests", context.AbstractRoles);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       "Enumerated Type 'Rubicon.Security.UnitTests.Domain.SimpleEnum' cannot be used as an abstract role. "
        + "Valid abstract roles must have the Rubicon.Security.AbstractRoleAttribute applied.\r\nParameter name: abstractRoles")]
    public void CreateSecurityContextWithInvalidAbstractRole ()
    {
      Enum[] abstractRoles = new Enum[] { SimpleEnum.First };
      SecurityContext context = new SecurityContext (
          "Rubicon.Security.UnitTests.SecurityContextTest", "owner", "group", "client", null, abstractRoles);
    }

    [Test]
    public void CreateSecurityContextWithNullAbstractRoles ()
    {
      SecurityContext context = new SecurityContext (
          "Rubicon.Security.UnitTests.SecurityContextTest", "owner", "group", "client", null, null);

      Assert.AreEqual (0, context.AbstractRoles.Length);
    }

    [Test]
    public void CreateSecurityContextWithState ()
    {
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum> ();
      testStates.Add ("Confidentiality", TestSecurityState.Public);
      testStates.Add ("State", TestSecurityState.Secret);

      SecurityContext context = new SecurityContext (
          "Rubicon.Security.UnitTests.SecurityContextTest", "owner", "group", "client", testStates, null);

      Assert.AreEqual (TestSecurityState.Public, context.GetState ("Confidentiality"));
      Assert.AreEqual (TestSecurityState.Secret, context.GetState ("State"));
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

      SecurityContext context = new SecurityContext (
          "Rubicon.Security.UnitTests.SecurityContextTest", "owner", "group", "client", testStates, null);
    }
  }
}
