using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.NullableValueTypes;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.Web.UI.Controls;
using System.ComponentModel;

namespace Rubicon.SecurityManager.Clients.Web.UI
{
  public partial class CurrentTenantControl : System.Web.UI.UserControl
  {
    private static readonly string s_isTenantSelectionEnabledKey = typeof (CurrentTenantControl).FullName + "_IsTenantSelectionEnabled";
    private static readonly string s_enableAbstractTenantsKey = typeof (CurrentTenantControl).FullName + "_EnableAbstractTenants";

    private bool _isCurrentTenantFieldReadOnly = true;
    private ClientTransaction _clientTransaction;

    [DefaultValue (true)]
    public bool EnableAbstractTenants
    {
      get { return (bool?) ViewState[s_enableAbstractTenantsKey] ?? true; }
      set { ViewState[s_enableAbstractTenantsKey] = value; }
    }

    protected SecurityManagerHttpApplication ApplicationInstance
    {
      get { return (SecurityManagerHttpApplication) Context.ApplicationInstance; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      if (ClientTransaction.HasCurrent)
        _clientTransaction = ClientTransaction.Current;
      else
        _clientTransaction = new ClientTransaction ();
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        DomainObjectCollection tenants = GetPossibleTenants ();
        CurrentTenantField.SetBusinessObjectList (tenants);
        CurrentTenantField.LoadUnboundValue (Tenant.Current, false);

        bool isCurrentTenantTheOnlyTenantInTheCollection = tenants.Count == 1 && Tenant.Current != null && tenants.Contains (Tenant.Current.ID);
        bool isCurrentTenantTheOnlyTenant = tenants.Count == 0 && Tenant.Current != null;
        bool hasExactlyOneTenant = isCurrentTenantTheOnlyTenantInTheCollection || isCurrentTenantTheOnlyTenant;
        IsTenantSelectionEnabled = !hasExactlyOneTenant;
      }

      if (!IsTenantSelectionEnabled)
        CurrentTenantField.Command.Type = CommandType.None;
    }

    private DomainObjectCollection GetPossibleTenants ()
    {
      User user = ApplicationInstance.LoadUserFromSession (_clientTransaction);
      DomainObjectCollection tenants;
      if (user == null)
        tenants = new DomainObjectCollection ();
      else
        tenants = user.Tenant.GetHierachy ();

      if (!EnableAbstractTenants)
      {
        for (int i = tenants.Count - 1; i >= 0; i--)
        {
          if (((Tenant) tenants[i]).IsAbstract)
            tenants.RemoveAt (i);
        }
      }

      return tenants;
    }

    protected void CurrentTenantField_SelectionChanged (object sender, EventArgs e)
    {
      string tenantID = CurrentTenantField.BusinessObjectID;
      if (StringUtility.IsNullOrEmpty (tenantID))
      {
        ApplicationInstance.SetCurrentTenant (null);
        _isCurrentTenantFieldReadOnly = false;
      }
      else
      {
        ApplicationInstance.SetCurrentTenant (Tenant.GetObject (ObjectID.Parse (tenantID), _clientTransaction));
        _isCurrentTenantFieldReadOnly = true;
      }

      CurrentTenantField.IsDirty = false;
    }

    protected void CurrentTenantField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      _isCurrentTenantFieldReadOnly = false;
      CurrentTenantField.SetBusinessObjectList (GetPossibleTenants ());
      CurrentTenantField.LoadUnboundValue (Tenant.Current, false);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (_isCurrentTenantFieldReadOnly && Tenant.Current != null)
        CurrentTenantField.ReadOnly = NaBooleanEnum.True;
      else
        CurrentTenantField.ReadOnly = NaBooleanEnum.False;

      User user = ApplicationInstance.LoadUserFromSession (_clientTransaction);
      CurrentUserField.LoadUnboundValue (user, false);
    }

    private bool IsTenantSelectionEnabled
    {
      get { return (bool?) ViewState[s_isTenantSelectionEnabledKey] ?? true; }
      set { ViewState[s_isTenantSelectionEnabledKey] = value; }
    }
  }
}