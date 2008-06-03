/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using Remotion.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;


namespace $PROJECT_ROOTNAMESPACE$.Classes
{
  public class EditFormPage: BasePage
  {
    private bool _isSaved;
    private bool _dataSourceModePropagated = false;

    /// <summary> Gets the data source for the edit form. Must be overridden in derived classes.</summary>
    protected virtual IBusinessObjectDataSourceControl DataSource { get { throw new NotImplementedException("Implement DataSource in derived class."); } }

    /// <summary> Gets the <see cref="TabbedMultiView"/> that holds the user controls.</summary>
    protected virtual TabbedMultiView UserControlMultiView 
    {
      get { return null; }
    }

    /// <summary> Gets all user controls that should load and save values of the current object. </summary>
    /// <remarks>
    /// The default implemenation gets all <see cref="DataEditUserControl"/> instances from the <see cref="TabbedMultiView"/>
    /// returned by <see cref="UserControlTabView"/>.
    /// </remarks>
    protected virtual IEnumerable<DataEditUserControl> DataEditUserControls
    {
      get
      {
        TabbedMultiView multiView = UserControlMultiView;
        if (multiView == null)
          yield break;

        foreach (TabView view in multiView.Views)
        {
          foreach (Control control in view.LazyControls)
          {
            DataEditUserControl userControl = control as DataEditUserControl;
            if (userControl != null)
              yield return userControl;
          }
        }
      }
    }

    protected void LoadObject (IBusinessObject businessObject)
    {
      DataSource.BusinessObject = businessObject;
      DataSource.LoadValues (IsPostBack);

      foreach (DataEditUserControl control in DataEditUserControls)
      {
        control.DataSource.BusinessObject = businessObject;
        control.LoadValues (IsPostBack);
      }
    }

    protected bool SaveObject()
    {
      EnsurePostLoadInvoked();
      EnsureValidatableControlsInitialized();

      bool isValid = DataSource.Validate();
      Control firstInvalidControl = null;

      foreach (DataEditUserControl control in DataEditUserControls)
      {
        if (! control.Validate())
        {
          isValid = false;
          firstInvalidControl = control;
          break;
        }
      }

      if (isValid)
      {
        foreach (DataEditUserControl control in DataEditUserControls)
          control.SaveValues (false);

        DataSource.SaveValues (false);
        _isSaved = true;
        // transaction will be committed by caller or using auto commit.
      }
      else
      {
        TabbedMultiView multiView = UserControlMultiView;
        if (multiView != null)
        {
          for (Control control = firstInvalidControl; control != null; control = control.Parent)
          {
            if (control is TabView)
            {
              multiView.SetActiveView ((TabView) control);
              break;
            }
          }
        }
      }

      return isValid;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      EnsureDataSourceModePropagated();
    }

    protected DataSourceMode DataSourceMode 
    {
      get { return DataSource.Mode; }
      set
      {
        DataSource.Mode = value;
        PropagateDataSourceMode();
      }
    }

    protected void EnsureDataSourceModePropagated()
    {
      if (! _dataSourceModePropagated)
      {
        PropagateDataSourceMode();
        _dataSourceModePropagated = true;
      }
    }

    private void PropagateDataSourceMode()
    {
      foreach (DataEditUserControl control in DataEditUserControls)
        control.DataSource.Mode = this.DataSource.Mode;
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);

      if (! _isSaved)
      {
        foreach (DataEditUserControl control in DataEditUserControls)
          control.SaveValues (true);

        DataSource.SaveValues (true);  
      }
    }

    public TUserControl GetUserControl<TUserControl> (string tabViewID)
      where TUserControl: Control
    {
      TabbedMultiView multiView = UserControlMultiView;
      if (multiView == null)
        throw new InvalidOperationException ("Page has no UserControlMultiView.");

      TabView view = (TabView) multiView.FindControl (tabViewID);
      if (view == null)
        throw new ArgumentOutOfRangeException ("No child control with ID " + tabViewID + " found.");

      view.EnsureLazyControls();
      foreach (Control childControl in view.LazyControls)
      {
        if (childControl is TUserControl)
          return (TUserControl) childControl;
      }

      throw new ArgumentOutOfRangeException ("TabView " + ID + " has no lazy control of type " + typeof (TUserControl).Name + ".");
    }

  }
}
