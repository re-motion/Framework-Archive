using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;
using Rubicon.SecurityManager.Domain.AccessControl;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class PermissionTest : DomainTest
  {
    private ClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransaction ();
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedTrue ()
    {
      Permission permission = Permission.NewObject (_transaction);
      permission.Allowed = true;

      Assert.IsTrue (permission.BinaryAllowed);
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedFalse ()
    {
      Permission permission = Permission.NewObject (_transaction);
      permission.Allowed = false;

      Assert.IsFalse (permission.BinaryAllowed);
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedNull ()
    {
      Permission permission = Permission.NewObject (_transaction);
      permission.Allowed = null;

      Assert.IsFalse (permission.BinaryAllowed);
    }

    [Test]
    public void SetBinaryAllowed_FromTrue()
    {
      Permission permission = Permission.NewObject (_transaction);
      permission.BinaryAllowed = true;

      Assert.AreEqual (true, permission.Allowed);
    }

    [Test]
    public void SetBinaryAllowed_FromFalse ()
    {
      Permission permission = Permission.NewObject (_transaction);
      permission.BinaryAllowed = false;

      Assert.IsNull (permission.Allowed);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Permission permission = Permission.NewObject (transaction);

      permission.Index = 1;
      Assert.AreEqual (1, permission.Index);
    }
  }
}
