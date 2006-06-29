using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.ObjectBinding.Web.UI.Controls;
using System.Collections.Generic;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions;

namespace Rubicon.SecurityManager.Clients.Web.Classes
{
  public abstract class BaseEditPage : BasePage
  {
    // types

    // static members and constants

    // member fields
    private List<DataEditUserControl> _dataEditUserControls = new List<DataEditUserControl> ();

    // construction and disposing

    // methods and properties
    protected new FormFunction CurrentFunction
    {
      get { return (FormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      foreach (DataEditUserControl control in _dataEditUserControls)
      {
        control.DataSource.BusinessObject = CurrentFunction.CurrentObject;
        control.LoadValues (IsPostBack);
      }
      LoadValues (IsPostBack);
    }

    protected virtual void LoadValues (bool interim)
    {
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      bool isValid = true;

      PrepareValidation ();

      foreach (DataEditUserControl dataEditUserControl in _dataEditUserControls)
        isValid &= dataEditUserControl.Validate ();
      isValid &= ValidatePage ();

      if (isValid)
      {
        foreach (DataEditUserControl dataEditUserControl in _dataEditUserControls)
          dataEditUserControl.SaveValues (false);
        SaveValues (false);

        CurrentFunction.CurrentTransaction.Commit ();
        ExecuteNextStep ();
      }
      else
      {
        ShowErrors ();
      }
    }

    protected virtual void ShowErrors ()
    {
    }
   
    protected virtual bool ValidatePage ()
    {
      return true;
    }

    protected virtual void SaveValues (bool interim)
    {
    }


    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);

      foreach (DataEditUserControl control in _dataEditUserControls)
        control.SaveValues (true);
    }

    protected void RegisterDataEditUserControl (DataEditUserControl dataEditUserControl)
    {
      _dataEditUserControls.Add (dataEditUserControl);
    }
  }
}
