using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.Design;

namespace Rubicon.Core.UnitTests.Design
{
  public class StubDesignModeHelper: DesignModeHelperBase
  {
    public StubDesignModeHelper (IDesignerHost designerHost)
        : base (designerHost)
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