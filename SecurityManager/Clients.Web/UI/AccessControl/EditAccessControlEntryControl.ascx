<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditAccessControlEntryControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditAccessControlEntryControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="EditPermissionControl.ascx" TagName="EditPermissionControl" %>
<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.AccessControl.AccessControlEntry, Rubicon.SecurityManager" />
<rubicon:FormGridManager ID="FormGridManager" runat="server" />
<table id="FormGrid" runat="server" class="accessControlEntry">
  <tr>
    <td class="accessControlEntryTitleCell" colspan="2">
      <h3 ID="AccessControlEntryTitle" runat="server">###</h3>
      <div class="accessControlEntryButtons">
        <rubicon:WebButton ID="DeleteAccessControlEntryButton" runat="server" Text="$res:DeleteAccessControlEntryButton" OnClick="DeleteAccessControlEntryButton_Click" CausesValidation="false" />
      </div>
    </td>
  </tr>
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
    <td><rubicon:SmartLabel ID="PriorityLabel" runat="server" ForControl="PriorityField"/></td>
    <td>
      <rubicon:BocTextValue ID="PriorityField" runat="server" PropertyIdentifier="Priority" DataSourceControl="CurrentObject" Width="10em" />
      / <rubicon:BocTextValue ID="ActualPriority" runat="server" PropertyIdentifier="ActualPriority" DataSourceControl="CurrentObject" />
    </td>
  </tr>
  <tr>
    <td><rubicon:FormGridLabel ID="PermissionsLabel" runat="server" Text="###" /></td>
    <td>
      <asp:PlaceHolder ID="PermissionsPlaceHolder" runat="server" />
      <%--
      <securityManager:ObjectBoundRepeater ID="PermissionsRepeater" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Permissions">
        <HeaderTemplate><ul class="permissionsList"><li class="permissionsList"></HeaderTemplate>
        <SeparatorTemplate></li><li class="permissionsList"></SeparatorTemplate>
        <FooterTemplate></li></ul></FooterTemplate>
        <ItemTemplate><securityManager:EditPermissionControl id="EditPermissionControl" runat="server"/></ItemTemplate>
      </securityManager:ObjectBoundRepeater>
      --%>
    </td>
  </tr>
</table>


