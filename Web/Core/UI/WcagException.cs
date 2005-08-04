using System;
using System.Web.UI;

namespace Rubicon.Web.UI
{

public class WcagException: Exception
{
  public WcagException()
    : this ("An element on the page is not WCAG conform.", null)
  {
  }

  public WcagException (int priority)
    : base (string.Format ("An element on the page does comply with a priority {0} checkpoint.", priority))
  {
  }

  public WcagException (int priority, Control control)
    : base (string.Format ("Control {1} does comply with a priority {0} checkpoint.", priority, control.ID))
  {
  }

  public WcagException (int priority, Control control, string property)
    : base (string.Format ("Property {2} of Control {1} does comply with a priority {0} checkpoint.", priority, control.ID, property))
  {
  }

  public WcagException (string message, Exception innerException)
    : base (message, innerException)
  {
  }
}

}
