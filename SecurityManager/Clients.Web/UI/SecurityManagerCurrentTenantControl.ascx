<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SecurityManagerCurrentTenantControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.CurrentTenantControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
 <div>
  <rubicon:BocReferenceValue ID="CurrentUserField" runat="server" ReadOnly="True">
    <PersistedCommand>
      <rubicon:BocCommand />
    </PersistedCommand>
  </rubicon:BocReferenceValue>
</div>
<div>
  <rubicon:BocReferenceValue ID="CurrentTenantField" runat="server" Required="True" OnSelectionChanged="CurrentTenantField_SelectionChanged" OnCommandClick="CurrentTenantField_CommandClick">
    <PersistedCommand>
      <rubicon:BocCommand Show="ReadOnly" Type="Event" />
    </PersistedCommand>
    <DropDownListStyle AutoPostBack="True" />
  </rubicon:BocReferenceValue>
</div>