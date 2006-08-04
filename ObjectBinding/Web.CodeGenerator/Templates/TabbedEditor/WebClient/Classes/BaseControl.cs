using System;
using Rubicon.Web.UI.Globalization;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;

namespace $PROJECT_ROOTNAMESPACE$.Classes
{
  [WebMultiLingualResources ("$PROJECT_ROOTNAMESPACE$.Globalization.Global")]
  public class BaseControl: DataEditUserControl
  {
    public IWxePage WxePage 
    {
      get { return Page as IWxePage; }
    }
  }
}
