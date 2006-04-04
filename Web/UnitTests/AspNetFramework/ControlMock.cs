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

    // construction and disposing

    public ControlMock ()
    {
    }

    // methods and properties

    public string ValueInViewState
    {
      get { return _valueInViewState; }
      set { _valueInViewState = value; }
    }

    protected override void LoadViewState (object savedState)
    {
      Pair values = (Pair) savedState;
      base.LoadViewState (values.First);
      _valueInViewState = (string) values.Second;
    }

    protected override object SaveViewState()
    {
      Pair values = new Pair ();
      values.First = base.SaveViewState ();
      values.Second = _valueInViewState;

      return values;
    }

  }

}
