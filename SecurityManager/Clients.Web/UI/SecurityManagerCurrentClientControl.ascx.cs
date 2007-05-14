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

namespace Rubicon.SecurityManager.Clients.Web.UI
{
  public partial class CurrentClientControl : System.Web.UI.UserControl
  {
    private static readonly string s_isClientSelectionEnabledKey = typeof (CurrentClientControl).FullName + "_IsClientSelectionEnabled";
    private bool _isCurrentClientFieldReadOnly = true;
    private ClientTransaction _clientTransaction;

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
        DomainObjectCollection clients = Client.FindAll (_clientTransaction);
        CurrentClientField.SetBusinessObjectList (clients);
        CurrentClientField.LoadUnboundValue (Client.Current, false);

        bool isCurrentClientTheOnlyClientInTheCollection = clients.Count == 1 && Client.Current != null && clients.Contains (Client.Current.ID);
        bool isCurrentClientTheOnlyClient = clients.Count == 0 && Client.Current != null;
        bool hasExactlyOneClient = isCurrentClientTheOnlyClientInTheCollection || isCurrentClientTheOnlyClient;
        IsClientSelectionEnabled = !hasExactlyOneClient;
      }

      if (!IsClientSelectionEnabled)
        CurrentClientField.Command.Type = CommandType.None;
    }

    protected void CurrentClientField_SelectionChanged (object sender, EventArgs e)
    {
      string clientID = CurrentClientField.BusinessObjectID;
      if (StringUtility.IsNullOrEmpty (clientID))
      {
        ApplicationInstance.SetCurrentClient (null);
        _isCurrentClientFieldReadOnly = false;
      }
      else
      {
        ApplicationInstance.SetCurrentClient (Client.GetObject (ObjectID.Parse (clientID), _clientTransaction));
        _isCurrentClientFieldReadOnly = true;
      }

      CurrentClientField.IsDirty = false;
    }

    protected void CurrentClientField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      _isCurrentClientFieldReadOnly = false;
      CurrentClientField.SetBusinessObjectList (Client.FindAll (_clientTransaction));
      CurrentClientField.LoadUnboundValue (Client.Current, false);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (_isCurrentClientFieldReadOnly && Client.Current != null)
        CurrentClientField.ReadOnly = NaBooleanEnum.True;
      else
        CurrentClientField.ReadOnly = NaBooleanEnum.False;

      User user = ApplicationInstance.LoadUserFromSession (_clientTransaction);
      CurrentUserField.LoadUnboundValue (user, false);
    }

    private bool IsClientSelectionEnabled
    {
      get { return (bool?) ViewState[s_isClientSelectionEnabledKey] ?? true; }
      set { ViewState[s_isClientSelectionEnabledKey] = value; }
    }
  }
}