﻿/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public partial class UserControlForm : WxePage
  {
    protected void PageButton_Click (object sender, EventArgs e)
    {
      PageLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      ViewStateLabel.Text = "#";
      ControlStateLabel.Text = "#";
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      ViewStateValue++;
      ViewStateLabel.Text = ViewStateValue.ToString ();

      ControlStateValue++;
      ControlStateLabel.Text = ControlStateValue.ToString ();
    }

    protected override void LoadControlState (object savedState)
    {
      var controlState = (Tuple<object, int>) savedState;
      base.LoadControlState (controlState.A);
      ControlStateValue = controlState.B;
    }

    protected override object SaveControlState ()
    {
      return new Tuple<object, int> (base.SaveControlState (), ControlStateValue);
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
  }
}
