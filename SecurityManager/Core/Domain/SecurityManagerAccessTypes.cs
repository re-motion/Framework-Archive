using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Security;
using Rubicon;

namespace Rubicon.SecurityManager.Domain
{
  [AccessType]
  public enum SecurityManagerAccessTypes
  {
    [PermanentGuid ("0348BE71-CFAF-4184-A3BF-C621B2611A29")]
    AssignRole = 0
  }
}