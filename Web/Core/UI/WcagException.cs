using System;
using System.Web.UI;

namespace Rubicon.Web.UI
{

public class WaiException: Exception
{
  public WaiException()
    : this ("An element on the page is not WAI conform.", null)
  {
  }

  public WaiException (int priority)
    : base (string.Format ("An element on the page does comply with a priority {0} checkpoint.", priority))
  {
  }

  public WaiException (int priority, Control control)
    : base (string.Format ("Control {1} does comply with a priority {0} checkpoint.", priority, control.ID))
  {
  }

  public WaiException (int priority, Control control, string property)
    : base (string.Format ("Property {2} of Control {1} does comply with a priority {0} checkpoint.", priority, control.ID, property))
  {
  }

  public WaiException (string message, Exception innerException)
    : base (message, innerException)
  {
  }
}

}
