using System;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{
  public class WxeMethodStep: WxeStep
  {
    private WxeMethod _method;
    private WxeMethodWithContext _methodWithContext;

    public WxeMethodStep (WxeMethod method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      _method = method;
    }

    public WxeMethodStep (WxeMethodWithContext method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      _methodWithContext = method;
    }

    public override void Execute (WxeContext context)
    {
      if (_method != null)
        _method ();
      else
        _methodWithContext (context);
    }
  }
}