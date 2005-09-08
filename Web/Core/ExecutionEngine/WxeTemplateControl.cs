using System;
using System.Web;
using System.Web.UI;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Web.UI.Globalization;
using Rubicon.Collections;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public interface IWxeTemplateControl: ITemplateControl
{
  NameObjectCollection Variables { get; }
  WxePageStep CurrentStep { get; }
  WxeFunction CurrentFunction { get; }
}

}
