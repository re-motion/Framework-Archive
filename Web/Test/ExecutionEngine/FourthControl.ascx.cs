using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public partial class FourthControl : WxeUserControl
  {
    protected void ExecuteNextStep_Click (object sender, EventArgs e)
    {
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
      ExecuteNextStep ();
    }

    protected void ExecuteThirdUserControlButton_Click (object sender, EventArgs e)
    {
      throw new InvalidOperationException ("This event handler should never be called.");
    }

    protected override void OnInitComplete (EventArgs e)
    {
      base.OnInitComplete (e);
      Page.RegisterRequiresControlState (this);
      ViewStateLabel.Text = "#";
      ControlStateLabel.Text = "#";
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (ControlStateValue == 0)
      {
        Assertion.IsTrue (IsPostBack);
        Assertion.IsFalse (IsUserControlPostBack);
      }
      else
      {
        Assertion.IsTrue (IsPostBack);
        Assertion.IsTrue (IsUserControlPostBack);
      }
      Assertion.IsTrue (WxePage.CurrentFunction is ShowUserControlFormFunction);
      Assertion.IsTrue (CurrentFunction is ShowFourthUserControlFormFunction);
      Assertion.IsTrue (WxePage.Variables != this.Variables);

      ViewStateValue++;
      ViewStateLabel.Text = ViewStateValue.ToString ();

      ControlStateValue++;
      ControlStateLabel.Text = ControlStateValue.ToString ();
    }

    protected override void LoadControlState (object savedState)
    {
      var controlState = (Tuple<object, int, Type>) savedState;
      base.LoadControlState (controlState.A);
      ControlStateValue = controlState.B;
      Assertion.IsTrue (controlState.C == typeof (FourthControl), "Expected ControlState from 'FourthControl' but was '{0}'.", controlState.C.Name);
    }

    protected override object SaveControlState ()
    {
      return new Tuple<object, int, Type> (base.SaveControlState (), ControlStateValue, typeof (FourthControl));
    }

    protected override void LoadViewState (object savedState)
    {
      Assertion.IsNotNull (savedState, "Missing ViewState.");

      var  statePair =  (Tuple<object, Type>) savedState;
      base.LoadViewState (statePair.A);

      Assertion.IsTrue (statePair.B == typeof (FourthControl), "Expected ViewState from 'FourthControl' but was '{0}'.", statePair.B.Name);
    }

    protected override object SaveViewState ()
    {
      return new Tuple<object, Type> (base.SaveViewState (), typeof (FourthControl));
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
  }
}