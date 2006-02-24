using System;
using System.Runtime.Serialization;

namespace Rubicon.Web.ExecutionEngine
{

[Serializable]
public class WxeTimeoutException: WxeException
{
  private string _functionToken;

  public WxeTimeoutException (string message, string functionToken)
    : base (message)
  {
  }

  public WxeTimeoutException (string message, string functionToken, Exception innerException)
    : base (message, innerException)
  {
    _functionToken = functionToken;
  }

  public WxeTimeoutException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }

  public string FunctionToken
  {
    get { return _functionToken; }
  }
}

}
