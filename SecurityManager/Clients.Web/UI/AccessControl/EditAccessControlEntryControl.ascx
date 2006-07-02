<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditAccessControlEntryControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditAccessControlEntryControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="EditPermissionControl.ascx" TagName="EditPermissionControl" %>
<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.AccessControl.AccessControlEntry, Rubicon.SecurityManager" />
<rubicon:FormGridManager ID="FormGridManager" runat="server" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr>
    <td></td>
    <td><rubicon:BocReferenceValue ID="SpecificAbstractRoleField" runat="server" PropertyIdentifier="SpecificAbstractRole" DataSourceControl="CurrentObject" Select="Rubicon.SecurityManager.Domain.Metadata.AbstractRoleDefinition.FindAll" >
      <PersistedCommand>
        <rubicon:BocCommand />
      </PersistedCommand>
    </rubicon:BocReferenceValue></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:BocReferenceValue ID="SpecificPositionField" runat="server" PropertyIdentifier="SpecificPosition" DataSourceControl="CurrentObject" Select="Rubicon.SecurityManager.Domain.OrganizationalStructure.Position.FindAll" >
      <PersistedCommand>
        <rubicon:BocCommand />
      </PersistedCommand>
    </rubicon:BocReferenceValue></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:BocTextValue ID="PriorityField" runat="server" PropertyIdentifier="Priority" DataSourceControl="CurrentObject" /></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:BocTextValue ID="ActualPriority" runat="server" PropertyIdentifier="ActualPriority" DataSourceControl="CurrentObject" /></td>
  </tr>
  <tr>
    <td><rubicon:FormGridLabel ID="PermissionsLabel" runat="server" Text="###" /></td>
    <td></td>
  </tr>
  <tr>
    <td colspan="2">
      <asp:PlaceHolder ID="PermissionsPlaceHolder" runat="server" />
      <%--
      <securityManager:ObjectBoundRepeater ID="PermissionsRepeater" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Permissions">
        <HeaderTemplate><table><tr><td></HeaderTemplate>
        <SeparatorTemplate></td></tr><tr><td></SeparatorTemplate>
        <FooterTemplate></td></tr></table></FooterTemplate>
        <ItemTemplate><securityManager:EditPermissionControl id="EditPermissionControl" runat="server"/></ItemTemplate>
      </securityManager:ObjectBoundRepeater>
      --%>
    </td>
  </tr>
</table>


