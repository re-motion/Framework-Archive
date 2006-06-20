using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  public static class AccessControlObjectAssert
  {
    public static void ContainsGroup (string groupName, IEnumerable<Group> groups)
    {
      foreach (Group group in groups)
      {
        if (group.Name == groupName)
          return;
      }

      Assert.Fail ("The list does not contain the group '{0}'.", groupName);
    }
  }
}
