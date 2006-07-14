using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rubicon.Security.UnitTests.SampleDomain;
using System.Security.Principal;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class GlobalAccessTypeCacheKeyTest
  {
    [Test]
    public void Initialize_WithContextAndIPrincipal ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));
      IPrincipal user = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      GlobalAccessTypeCacheKey key = new GlobalAccessTypeCacheKey (context, user);

      Assert.AreSame (context, key.Context);
      Assert.AreEqual ("user", key.UserName);
    }

    [Test]
    public void Initialize_WithContextAndUserName ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));
      GlobalAccessTypeCacheKey key = new GlobalAccessTypeCacheKey (context, "user");

      Assert.AreSame (context, key.Context);
      Assert.AreEqual ("user", key.UserName);
    }

    [Test]
    public void Equals_FullyQualified ()
    {
      Dictionary<string, Enum> leftStates = new Dictionary<string, Enum> ();
      leftStates.Add ("State", TestSecurityState.Public);
      leftStates.Add ("Confidentiality", TestSecurityState.Public);
      Enum[] leftAbstractRoles = new Enum[] { TestAbstractRole.Developer, TestAbstractRole.QualityEngineer };
      SecurityContext leftContext = new SecurityContext (typeof (SecurableObject), "owner", "ownerGroup", "ownerClient", leftStates, leftAbstractRoles);
      GlobalAccessTypeCacheKey left = new GlobalAccessTypeCacheKey (leftContext, "user");

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum> ();
      rightStates.Add ("Confidentiality", TestSecurityState.Public);
      rightStates.Add ("State", TestSecurityState.Public);
      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRole.QualityEngineer, TestAbstractRole.Developer };
      SecurityContext rightContext = new SecurityContext (typeof (SecurableObject), "owner", "ownerGroup", "ownerClient", rightStates, rightAbstractRoles);
      GlobalAccessTypeCacheKey right = new GlobalAccessTypeCacheKey (rightContext, "user");

      Assert.IsTrue (left.Equals (right));
    }

    [Test]
    public void Equals_WithNull ()
    {
      GlobalAccessTypeCacheKey key = new GlobalAccessTypeCacheKey (new SecurityContext (typeof (SecurableObject)), "user");

      Assert.IsFalse (key.Equals (null));
    }

    [Test]
    public void Equals_WithEqual ()
    {
      GlobalAccessTypeCacheKey key = new GlobalAccessTypeCacheKey (new SecurityContext (typeof (SecurableObject)), "user");

      Assert.IsTrue (key.Equals (key));
    }

    [Test]
    public void Equals_WithSameSecurityContextAndEqualUserName ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));
      GlobalAccessTypeCacheKey left = new GlobalAccessTypeCacheKey (context, "user");
      GlobalAccessTypeCacheKey right = new GlobalAccessTypeCacheKey (context, "user");

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithSameSecurityContextAndDifferentUserName ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));
      GlobalAccessTypeCacheKey left = new GlobalAccessTypeCacheKey (context, "user1");
      GlobalAccessTypeCacheKey right = new GlobalAccessTypeCacheKey (context, "user2");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDiffentSecurityContextAndEqualUserName ()
    {
      GlobalAccessTypeCacheKey left = new GlobalAccessTypeCacheKey (new SecurityContext (typeof (SecurableObject)), "user");
      GlobalAccessTypeCacheKey right = new GlobalAccessTypeCacheKey (new SecurityContext (typeof (SecurableObjectWithSecuredInstanceMethods)), "user");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));
      GlobalAccessTypeCacheKey left = new GlobalAccessTypeCacheKey (context, "user");
      GlobalAccessTypeCacheKey right = new GlobalAccessTypeCacheKey (context, "user");

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      GlobalAccessTypeCacheKey key = new GlobalAccessTypeCacheKey (new SecurityContext (typeof (SecurableObject)), "user");

      Assert.IsFalse (key.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      GlobalAccessTypeCacheKey key = new GlobalAccessTypeCacheKey (new SecurityContext (typeof (SecurableObject)), "user");

      Assert.IsFalse (key.Equals (new object ()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      GlobalAccessTypeCacheKey left = new GlobalAccessTypeCacheKey (new SecurityContext (typeof (SecurableObject)), "user");
      GlobalAccessTypeCacheKey right = new GlobalAccessTypeCacheKey (new SecurityContext (typeof (SecurableObject)), "user");

      Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
    }

  }
}