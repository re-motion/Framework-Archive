using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  public static class AccessControlObjectAssert
  {
    public static void ContainsGroup (string groupUniqueIdentifier, IEnumerable<Group> groups)
    {
      foreach (Group group in groups)
      {
        if (group.UniqueIdentifier == groupUniqueIdentifier)
          return;
      }

      Assert.Fail ("The list does not contain the group '{0}'.", groupUniqueIdentifier);
    }
  }
}
