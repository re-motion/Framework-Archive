using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryValidationResultTest
  {
    [Test]
    public void IsValid_Valid ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult ();

      Assert.IsTrue (result.IsValid);
      Assert.IsFalse (result.IsSpecificClientMissing);
    }

    [Test]
    public void IsValid_IsSpecificClientMissing ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult ();

      result.SetSpecificClientMissing ();

      Assert.IsFalse (result.IsValid);
      Assert.IsTrue (result.IsSpecificClientMissing);
    }

  }
}