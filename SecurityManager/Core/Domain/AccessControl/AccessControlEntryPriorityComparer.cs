using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public class AccessControlEntryPriorityComparer : IComparer<AccessControlEntry>
  {
    public int Compare (AccessControlEntry x, AccessControlEntry y)
    {
      if (x == null && y == null)
        return 0;

      if (x == null)
        return -1;

      if (y == null)
        return 1;

      return x.ActualPriority.CompareTo (y.ActualPriority);
    }
  }
}
