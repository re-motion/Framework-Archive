using System;
using System.Web.UI;

namespace Rubicon.Web.UnitTests.AspNetFramework
{

  public class ControlMock : Control
  {
    // types

    // static members and constants

    // member fields

    private string _valueInViewState;
#if ! NET11
    private string _valueInControlState;
#endif

    // construction and disposing

    public ControlMock ()
    {
    }

    // methods and properties

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

#if ! NET11
      Page.RegisterRequiresControlState (this);
#endif
    }

    public string ValueInViewState
    {
      get { return _valueInViewState; }
      set { _valueInViewState = value; }
    }

#if ! NET11
    public string ValueInControlState
    {
      get { return _valueInControlState; }
      set { _valueInControlState = value; }
    }
#endif
    
    protected override void LoadViewState (object savedState)
    {
      _valueInViewState = (string) savedState;
    }

    protected override object SaveViewState()
    {
      return _valueInViewState;
    }

#if ! NET11
    protected override void LoadControlState (object savedState)
    {
      _valueInControlState = (string) savedState;
    }
  
    protected override object SaveControlState ()
    {
      return _valueInControlState;
    }
#endif
  }

}
