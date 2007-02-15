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
      Permission permission = new Permission (_transaction);
      permission.Allowed = NaBoolean.True;

      Assert.AreEqual (true, permission.BinaryAllowed);
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedFalse ()
    {
      Permission permission = new Permission (_transaction);
      permission.Allowed = NaBoolean.False;

      Assert.AreEqual (false, permission.BinaryAllowed);
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedNull ()
    {
      Permission permission = new Permission (_transaction);
      permission.Allowed = NaBoolean.Null;

      Assert.AreEqual (false, permission.BinaryAllowed);
    }

    [Test]
    public void SetBinaryAllowed_FromTrue()
    {
      Permission permission = new Permission (_transaction);
      permission.BinaryAllowed = true;

      Assert.AreEqual (NaBoolean.True, permission.Allowed);
    }

    [Test]
    public void SetBinaryAllowed_FromFalse ()
    {
      Permission permission = new Permission (_transaction);
      permission.BinaryAllowed = false;

      Assert.AreEqual (NaBoolean.Null, permission.Allowed);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Permission permission = new Permission (transaction);

      permission.Index = 1;
      Assert.AreEqual (1, permission.Index);
    }
  }
}
