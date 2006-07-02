<%@ Page Language="C#" AutoEventWireup="true" Codebehind="EditPermissionsForm.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionsForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>
<%@ Register TagPrefix="securityManager" Src="EditAccessControlListControl.ascx" TagName="EditAccessControlListControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl ID="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <rubicon:FormGridManager ID="FormGridManager" runat="server" />
  <rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.Metadata.SecurableClassDefinition, Rubicon.SecurityManager" />
  <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="height:100%;">
    <tr>
      <td style="white-space: nowrap;" colspan="2">
        <rubicon:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DisplayName" ReadOnly="True" />
      </td>
    </tr>
    <tr>
      <td class="formGridControlsCell" style="height:100%;">
        <asp:PlaceHolder ID="AccessControlListsPlaceHolder" runat="server"/>
        <%-- 
        <securityManager:ObjectBoundRepeater ID="AccessControlListsRepeater" runat="server" PropertyIdentifier="AccessControlLists">
          <HeaderTemplate><div></HeaderTemplate>
          <SeparatorTemplate></div><div></SeparatorTemplate>
          <FooterTemplate></div></FooterTemplate>
          <ItemTemplate><securityManager:EditAccessControlListControl id="EditAccessControlListControl" runat="server"/></ItemTemplate>
        </securityManager:ObjectBoundRepeater>
        --%>
      </td>
    </tr>
  </table>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <rubicon:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" CausesValidation="false" />
  <rubicon:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" Style="margin-left: 1em;" OnClick="CancelButton_Click" CausesValidation="false" />
  <rubicon:WebButton ID="NewButton" runat="server" Text="$res:New" Style="margin-left: 1em;" OnClick="NewButton_Click" CausesValidation="false" />
  <rubicon:WebButton ID="PostBackButton" runat="server" Text="PostBack" Style="margin-left: 1em;" CausesValidation="false" />
</asp:Content>
