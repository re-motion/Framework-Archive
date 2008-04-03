using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Remotion.Design;

namespace Remotion.Core.UnitTests.Design
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