using System;

namespace Rubicon.Web.ExecutionEngine
{
  public class WxeExecuteNextStepException: Exception
  {
    public WxeExecuteNextStepException()
        : base ("This exception does not indicate an error. It is used to roll back the call stack. It is recommended to disable breaking on this exeption type while debugging.")
    {
    }
  }
}