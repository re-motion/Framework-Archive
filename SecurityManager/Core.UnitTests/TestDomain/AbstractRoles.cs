using System;
using Rubicon.Security;

namespace Rubicon.SecurityManager.UnitTests.TestDomain
{
  [AbstractRole]
  public enum ProjectRoles
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
