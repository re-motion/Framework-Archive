using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Rubicon.Web.UI;

namespace Rubicon.Web.ExecutionEngine
{
  public interface IWxePage: IPage, IWxeTemplateControl
  {
    NameValueCollection GetPostBackCollection ();
    void ExecuteNextStep ();
    void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback);
    void ExecuteFunction (WxeFunction function);
    void ExecuteFunctionNoRepost (WxeFunction function, Control sender);
    void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget);
    bool IsReturningPostBack { get; }
    WxeFunction ReturningFunction { get; }

    [EditorBrowsable (EditorBrowsableState.Never)]
    HtmlForm HtmlForm { get; set; }

    WxeHandler WxeHandler { get; }
  }
}