using System;
using System.Web.UI;
using Rubicon.Web.UI;

namespace Rubicon.Web.UnitTests.UI
{
/// <summary> Provides programmatically accessible feedback on the WCAG conformance </summary>
public class WcagHelperMock: WcagHelper
{
  private bool _hasError;
  private bool _hasWarning;
  private int _priority;
  private Control _control;
  private string _property;

	public WcagHelperMock()
	{
    Reset();
	}

  public void Reset()
  {
    _hasError = false;
    _hasWarning = false;
    _priority = 0;
    _control = null;
    _property = null;
  }

  public override void HandleError (int priority)
  {
    HandleError (priority, null, null);
  }

  public override void HandleError (int priority, Control control)
  {
    HandleError (priority, control, null);
  }

  public override void HandleError (int priority, Control control, string property)
  {
    _hasError = true;
    _hasWarning = false;
    _priority = priority;
    _control = control;
    _property = property;
  }
 
  public override void HandleWarning (int priority)
  {
    HandleWarning (priority, null, null);
  }

  public override void HandleWarning (int priority, Control control)
  {
    HandleWarning (priority, control, null);
  }

  public override void HandleWarning (int priority, Control control, string property)
  {
    _hasError = false;
    _hasWarning = true;
    _priority = priority;
    _control = control;
    _property = property;
  }
 
  public override void HandleError (string message)
  {
    throw new NotSupportedException ("Not supported for testing purposes. Use the HandleError (int, [Control, [string]]) overload instead.");
  }
 
  public override void HandleWarning (string message)
  {
    throw new NotSupportedException ("Not supported for testing purposes. Use the HandleWarning (int, [Control, [string]]) overload instead.");
  }

  public bool HasError
  {
    get { return _hasError; }
  }

  public bool HasWarning
  {
    get { return _hasWarning; }
  }

  public int Priority
  {
    get { return _priority; }
  }

  public Control Control
  {
    get { return _control; }
  }

  public string Property
  {
    get { return _property; }
  }

}
}
