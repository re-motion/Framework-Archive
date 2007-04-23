using System;
using System.ComponentModel;
using Rubicon.Design;

namespace Rubicon.Core.UnitTests.Design
{
  public class StubDesignModeHelper: DesignModeHelperBase
  {
    public StubDesignModeHelper (ISite site)
        : base (site)
    {
    }

    public override string GetProjectPath()
    {
      throw new NotImplementedException();
    }

    public override System.Configuration.Configuration GetConfiguration()
    {
      throw new NotImplementedException();
    }
  }
}