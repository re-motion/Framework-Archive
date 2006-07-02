using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.NullableValueTypes;

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
  }
}
