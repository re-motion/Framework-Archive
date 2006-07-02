<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditAccessControlListControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditAccessControlListControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="EditStateCombinationControl.ascx" TagName="EditStateCombinationControl" %>
<%@ Register TagPrefix="securityManager" Src="EditAccessControlEntryControl.ascx" TagName="EditAccessControlEntryControl" %>

<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.AccessControl.AccessControlList, Rubicon.SecurityManager" />
<table style="height:100%; width: 100%;">
  <tr>
    <td style="height: 100%;">
      <div id="StateCombinationControls" runat="server" style="overflow: auto;	height: 100%; width: 100%;"><%-- 
        <securityManager:ObjectBoundRepeater ID="StateCombinationsRepeater" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StateCombinations">
          <HeaderTemplate><table><tr><td></HeaderTemplate>
          <SeparatorTemplate></td></tr><tr><td></SeparatorTemplate>
          <FooterTemplate></td></tr></table></FooterTemplate>
          <ItemTemplate><securityManager:EditStateCombinationControl id="EditStateCombinationControl" runat="server"/></ItemTemplate>
        </securityManager:ObjectBoundRepeater>
        --%></div>
    </td>
    <td style="height: 100%;">
      <div id="AccessControlEntryControls" runat="server" style="overflow: auto;	height: 100%; width: 100%;"><%-- 
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
  <td colspan="2" style="height: 0%">
    <rubicon:WebButton ID="NewStateCombinationButton" runat="server" Text="$res:NewStateCombinationButton" OnClick="NewStateCombinationButton_Click" CausesValidation="false" />
    <rubicon:WebButton ID="NewAccessControlEntryButton" runat="server" Text="$res:NewAccessControlEntryButton" Style="margin-left: 1em;" OnClick="NewAccessControlEntryButton_Click" CausesValidation="false" />
  </td>
  </tr>
</table>

