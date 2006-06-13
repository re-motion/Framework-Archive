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

    [Test]
    public void IsStateless_WithStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string,Enum>();
      states.Add ("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates (states);

      Assert.IsFalse (context.IsStateless);
    }

    [Test]
    public void IsStateless_WithoutStates ()
    {
      SecurityContext context = CreateTestSecurityContext ();

      Assert.IsTrue (context.IsStateless);
    }

    [Test]
    public void ContainsState_ContextContainsDemandedState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates (states);

      Assert.IsTrue (context.ContainsState ("Confidentiality"));
    }

    [Test]
    public void ContainsState_ContextDoesNotContainDemandedState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates (states);

      Assert.IsFalse (context.ContainsState ("State"));
    }

    [Test]
    public void GetNumberOfStates_WithStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates (states);

      Assert.AreEqual (1, context.GetNumberOfStates());
    }

    [Test]
    public void GetNumberOfStates_WithoutStates ()
    {
      SecurityContext context = CreateTestSecurityContext ();

      Assert.AreEqual (0, context.GetNumberOfStates ());
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
