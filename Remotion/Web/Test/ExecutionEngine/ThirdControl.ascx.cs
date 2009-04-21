// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
  public partial class ThirdControl : WxeUserControl
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

    protected void ExecuteFourthUserControlButton_Click (object sender, EventArgs e)
    {
      if (!WxePage.IsReturningPostBack)
      {
        ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Executed";
        ExecuteFunction (new ShowFourthUserControlFormFunction (), (Control) sender, null);
      }
      else
      {
        ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Returned";
      }
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
      Assertion.IsTrue (CurrentFunction is ShowThirdUserControlFormFunction);
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
      Assertion.IsTrue (controlState.C == typeof (ThirdControl), "Expected ControlState from 'ThirdControl' but was '{0}'.", controlState.C.Name);
    }

    protected override object SaveControlState ()
    {
      return new Tuple<object, int, Type> (base.SaveControlState (), ControlStateValue, typeof (ThirdControl));
    }

    protected override void LoadViewState (object savedState)
    {
      Assertion.IsNotNull (savedState, "Missing ViewState.");

      var  statePair =  (Tuple<object, Type>) savedState;
      base.LoadViewState (statePair.A);

      Assertion.IsTrue (statePair.B == typeof (ThirdControl), "Expected ViewState from 'ThirdControl' but was '{0}'.", statePair.B.Name);
    }

    protected override object SaveViewState ()
    {
      return new Tuple<object, Type> (base.SaveViewState (), typeof (ThirdControl));
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
  }
}