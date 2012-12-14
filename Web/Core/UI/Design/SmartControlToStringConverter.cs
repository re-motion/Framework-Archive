using System;

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
