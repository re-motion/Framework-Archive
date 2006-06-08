using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Utilities;

using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Security.UnitTests.TestDomain;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityContextTest
  {
    [Test]
    public void CreateSecurityContextWithAbstractRole ()
    {
      Enum[] abstractRoles = new Enum[] { TestAbstractRole.QualityEngineer, TestAbstractRole.Developer };
      SecurityContext context = CreateTestSecurityContextWithAbstractRoles (abstractRoles);

      Assert.AreEqual (2, context.AbstractRoles.Length);
      Assert.Contains (new EnumWrapper (TestAbstractRole.QualityEngineer), context.AbstractRoles);
      Assert.Contains (new EnumWrapper (TestAbstractRole.Developer), context.AbstractRoles);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       "Enumerated Type 'Rubicon.Security.UnitTests.SampleDomain.SimpleEnum' cannot be used as an abstract role. "
        + "Valid abstract roles must have the Rubicon.Security.AbstractRoleAttribute applied.\r\nParameter name: abstractRoles")]
    public void CreateSecurityContextWithInvalidAbstractRole ()
    {
      // SimpleEnum does not have AbstractRoleAttribute
      Enum[] abstractRoles = new Enum[] { SimpleEnum.First };
      CreateTestSecurityContextWithAbstractRoles (abstractRoles);
    }

    [Test]
    public void CreateSecurityContextWithNullAbstractRoles ()
    {
      SecurityContext context = CreateTestSecurityContextWithAbstractRoles (null);
      Assert.AreEqual (0, context.AbstractRoles.Length);
    }

    [Test]
    public void CreateSecurityContextWithState ()
    {
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum> ();
      testStates.Add ("Confidentiality", TestSecurityState.Public);
      testStates.Add ("State", TestSecurityState.Secret);

      SecurityContext context = CreateTestSecurityContextWithStates (testStates);

      Assert.AreEqual (new EnumWrapper (TestSecurityState.Public), context.GetState ("Confidentiality"));
      Assert.AreEqual (new EnumWrapper (TestSecurityState.Secret), context.GetState ("State"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       "Enumerated Type 'Rubicon.Security.UnitTests.SampleDomain.SimpleEnum' cannot be used as a security state. "
        + "Valid security states must have the Rubicon.Security.SecurityStateAttribute applied.\r\nParameter name: states")]
    public void CreateSecurityContextWithInvalidState ()
    {
      // SimpleEnum does not have SecurityStateAttribute
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum> ();
      testStates.Add ("Confidentiality", TestSecurityState.Public);
      testStates.Add ("State", SimpleEnum.Second);

      CreateTestSecurityContextWithStates (testStates);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void CreateSecurityContextWithInvalidType ()
    {
      CreateTestSecurityContextForType (typeof (SimpleType));
    }

    [Test]
    public void GetClassName ()
    {
      SecurityContext context = CreateTestSecurityContext ();
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File, Rubicon.Security.UnitTests.TestDomain", context.Class);
    }

    private SecurityContext CreateTestSecurityContextForType (Type type)
    {
      return CreateTestSecurityContext (type, null, null);
    }

    private SecurityContext CreateTestSecurityContextWithStates (IDictionary<string, Enum> states)
    {
      return CreateTestSecurityContext (states, null);
    }

    private SecurityContext CreateTestSecurityContextWithAbstractRoles (ICollection<Enum> abstractRoles)
    {
      return CreateTestSecurityContext (null, abstractRoles);
    }

    private SecurityContext CreateTestSecurityContext ()
    {
      return CreateTestSecurityContext (null, null);
    }

    private SecurityContext CreateTestSecurityContext (IDictionary<string, Enum> states, ICollection<Enum> abstractRoles)
    {
      return CreateTestSecurityContext (typeof (File), states, abstractRoles);
    }

    private SecurityContext CreateTestSecurityContext (Type type, IDictionary <string, Enum> states, ICollection<Enum> abstractRoles)
    {
      return new SecurityContext (type, "owner", "group", "client", states, abstractRoles);
    }
  }
}
