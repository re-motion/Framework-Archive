using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Security;

namespace Rubicon.SecurityManager.UnitTests.TestDomain
{
  [AbstractRole]
  public enum ProjectRole
  {
    QualityManager,
    Developer
  }

  [AbstractRole]
  public enum UndefinedAbstractRoles
  {
    Undefined
  }
}
