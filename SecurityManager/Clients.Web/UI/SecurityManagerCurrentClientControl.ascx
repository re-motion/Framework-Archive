<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SecurityManagerCurrentClientControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.CurrentClientControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
 <div>
  <rubicon:BocReferenceValue ID="CurrentUserField" runat="server" ReadOnly="True">
    <PersistedCommand>
      <rubicon:BocCommand />
    </PersistedCommand>
  </rubicon:BocReferenceValue>
</div>
<div>
  <rubicon:BocReferenceValue ID="CurrentClientField" runat="server" Required="True" OnSelectionChanged="CurrentClientField_SelectionChanged" OnCommandClick="CurrentClientField_CommandClick">
    <PersistedCommand>
      <rubicon:BocCommand Show="ReadOnly" Type="Event" />
    </PersistedCommand>
    <DropDownListStyle AutoPostBack="True" />
  </rubicon:BocReferenceValue>
</div>