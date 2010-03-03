// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Text;
using System.Web.UI;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{
  [WebMultiLingualResources ("OBWTest.Globalization.SingleBocTestBasePage")]
  public partial class IndividualControlTestForm : TestBasePage
  {
    private IDataEditControl _dataEditControl;
    private bool _isCurrentObjectSaved = false;

    public IndividualControlTestForm ()
    {
      MasterPageFile = (Global.PreferQuirksModeRendering) ? "~/QuirksMode.Master" : "~/StandardMode.Master";
    }

    protected override void RegisterEventHandlers ()
    {
      base.RegisterEventHandlers();

      PostBackButton.Click += new EventHandler (PostBackButton_Click);
      SaveButton.Click += new EventHandler (SaveButton_Click);
      SaveAndRestartButton.Click += new EventHandler (SaveAndRestartButton_Click);
      CancelButton.Click += new EventHandler (CancelButton_Click);
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      //EnableAbort = false;
      EnableOutOfSequencePostBacks = true;
      ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      LoadUserControl();
      PopulateDataSources();
      LoadValues (IsPostBack);
      string test = GetPermanentUrl();

      if (ScriptManager.GetCurrent (this).IsInAsyncPostBack)
        StackUpdatePanel.Update();
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      string mode = Global.PreferQuirksModeRendering ? "Quirks" : "Standard";
      string theme = SafeServiceLocator.Current.GetInstance<ResourceTheme>().Name;
      NavigationTabs.StatusText = mode + " " + theme;

      StringBuilder sb = new StringBuilder();
      sb.Append ("<b>Stack:</b><br />");
      for (WxeStep step = CurrentPageStep; step != null; step = step.ParentStep)
        sb.AppendFormat ("{0}<br />", step.ToString());
      Stack.Text = sb.ToString();
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);

      if (! _isCurrentObjectSaved)
      {
        SaveValues (true);
      }
    }

    protected override void LoadViewState (object savedState)
    {
      base.LoadViewState (savedState);
    }

    private void PostBackButton_Click (object sender, EventArgs e)
    {
    }

    private void SaveButton_Click (object sender, EventArgs e)
    {
      bool isValid = ValidateDataSources();
      if (isValid)
      {
        SaveValues (false);
        _isCurrentObjectSaved = true;
      }
    }

    private void SaveAndRestartButton_Click (object sender, EventArgs e)
    {
      bool isValid = ValidateDataSources();
      if (isValid)
      {
        SaveValues (false);
        _isCurrentObjectSaved = true;
        ExecuteNextStep();
      }
    }

    private void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException();
    }

    private void LoadUserControl ()
    {
      _dataEditControl = (IDataEditControl) LoadControl (CurrentFunction.UserControl);
      if (_dataEditControl == null)
        throw new InvalidOperationException (string.Format ("IDataEditControl '{0}' could not be loaded.", CurrentFunction.UserControl));
      _dataEditControl.ID = "DataEditControl";
      UserControlPlaceHolder.Controls.Add ((Control) _dataEditControl);
    }

    private void PopulateDataSources ()
    {
      CurrentObject.BusinessObject = (IBusinessObject) CurrentFunction.Person;
      if (_dataEditControl != null)
        _dataEditControl.BusinessObject = (IBusinessObject) CurrentFunction.Person;
    }

    private void LoadValues (bool interim)
    {
      CurrentObject.LoadValues (interim);
      if (_dataEditControl != null)
        _dataEditControl.LoadValues (interim);
    }

    private void SaveValues (bool interim)
    {
      if (_dataEditControl != null)
        _dataEditControl.SaveValues (interim);
      CurrentObject.SaveValues (interim);
    }

    private bool ValidateDataSources ()
    {
      PrepareValidation();

      bool isValid = _dataEditControl.Validate();
      isValid &= CurrentObject.Validate();

      return isValid;
    }
  }
}
