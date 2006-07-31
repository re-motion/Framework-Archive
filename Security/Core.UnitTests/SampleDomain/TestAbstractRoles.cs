using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  [AbstractRole]
  public enum TestAbstractRoles
  {
    QualityEngineer,
    Developer,
    Manager
  }
}
