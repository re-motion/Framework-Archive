using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{

public class SmartControlToStringConverter: ControlToStringConverter
{
  public SmartControlToStringConverter ()
    : base (typeof (ISmartControl))
  {
  }
}

}
