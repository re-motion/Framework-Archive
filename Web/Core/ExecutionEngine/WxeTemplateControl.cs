using System;
using Rubicon.Collections;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.ExecutionEngine
{

public interface IWxeTemplateControl: ITemplateControl
{
  NameObjectCollection Variables { get; }
  WxePageStep CurrentStep { get; }
  WxeFunction CurrentFunction { get; }
}

}
