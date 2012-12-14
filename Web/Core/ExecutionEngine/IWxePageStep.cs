using System.Web.UI;

namespace Rubicon.Web.ExecutionEngine
{
  public interface IWxePageStep
  {
    void ExecuteFunction(IWxePage wxePage, WxeFunction function);
    void ExecuteFunctionNoRepost(IWxePage page, WxeFunction function, Control sender, bool usesEventTarget);
    string PageToken { get; }
  }
}