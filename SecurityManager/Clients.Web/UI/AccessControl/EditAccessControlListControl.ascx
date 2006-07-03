<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditAccessControlListControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditAccessControlListControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="EditStateCombinationControl.ascx" TagName="EditStateCombinationControl" %>
<%@ Register TagPrefix="securityManager" Src="EditAccessControlEntryControl.ascx" TagName="EditAccessControlEntryControl" %>

<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.AccessControl.AccessControlList, Rubicon.SecurityManager" />
<table class="accessControlList">
  <tr>
    <td class="stateCombinationsContainer">
      <div id="StateCombinationControls" runat="server" class="stateCombinationsContainer"><%-- 
        <securityManager:ObjectBoundRepeater ID="StateCombinationsRepeater" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StateCombinations">
          <HeaderTemplate><table><tr><td></HeaderTemplate>
          <SeparatorTemplate></td></tr><tr><td></SeparatorTemplate>
          <FooterTemplate></td></tr></table></FooterTemplate>
          <ItemTemplate><securityManager:EditStateCombinationControl id="EditStateCombinationControl" runat="server"/></ItemTemplate>
        </securityManager:ObjectBoundRepeater>
        --%></div>
    </td>
    <td class="accessControlEntriesContainer">
      <div id="AccessControlEntryControls" runat="server" class="accessControlEntriesContainer"><%-- 
        <securityManager:ObjectBoundRepeater ID="AccessControlEntriesRepeater" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="AccessControlEntries">
          <HeaderTemplate><table><tr><td></HeaderTemplate>
          <SeparatorTemplate></td></tr><tr><td></SeparatorTemplate>
          <FooterTemplate></td></tr></table></FooterTemplate>
          <ItemTemplate><securityManager:EditAccessControlEntryControl id="EditAccessControlEntryControl" runat="server"/></ItemTemplate>
        </securityManager:ObjectBoundRepeater>
        --%></div>
   </td>
  </tr>
  <tr>
  <td colspan="2" class="accessControlListButtons">
    <rubicon:WebButton ID="NewStateCombinationButton" runat="server" Text="$res:NewStateCombinationButton" OnClick="NewStateCombinationButton_Click" CausesValidation="false" />
    <rubicon:WebButton ID="NewAccessControlEntryButton" runat="server" Text="$res:NewAccessControlEntryButton" Style="margin-left: 1em;" OnClick="NewAccessControlEntryButton_Click" CausesValidation="false" />
    <rubicon:WebButton ID="DeleteAccessControlListButton" runat="server" Text="$res:DeleteAccessControlListButton" Style="margin-left: 1em;" OnClick="DeleteAccessControlListButton_Click" CausesValidation="false" />
  </td>
  </tr>
</table>

