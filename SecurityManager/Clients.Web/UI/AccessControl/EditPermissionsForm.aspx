<%@ Page Language="C#" AutoEventWireup="true" Codebehind="EditPermissionsForm.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionsForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>
<%@ Register TagPrefix="securityManager" Src="EditAccessControlListControl.ascx" TagName="EditAccessControlListControl" %>

<asp:Content ID="ActualHeaderControlsPlaceHolder" runat="server" ContentPlaceHolderID="HeaderControlsPlaceHolder">
  <rubicon:BindableObjectDataSourceControl ID="CurrentObjectHeaderControls" runat="server" Type="Rubicon.SecurityManager.Domain.Metadata.SecurableClassDefinition, Rubicon.SecurityManager" Mode="Read" />
  <h1><rubicon:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObjectHeaderControls" PropertyIdentifier="DisplayName" /></h1>
</asp:Content>
<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder" />
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <rubicon:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Rubicon.SecurityManager.Domain.Metadata.SecurableClassDefinition, Rubicon.SecurityManager" />
  <asp:CustomValidator ID="DuplicateStateCombinationsValidator" runat="server" ErrorMessage="<%$ res:DuplicateStateCombinationsValidatorErrorMessage %>" OnServerValidate="DuplicateStateCombinationsValidator_ServerValidate"/>
  <asp:PlaceHolder ID="AccessControlListsPlaceHolder" runat="server"/>
  <%-- 
  <securityManager:ObjectBoundRepeater ID="AccessControlListsRepeater" runat="server" PropertyIdentifier="AccessControlLists">
    <HeaderTemplate><div class="accessControlList"></HeaderTemplate>
    <SeparatorTemplate></div><div class="accessControlList"></SeparatorTemplate>
    <FooterTemplate></div></FooterTemplate>
    <ItemTemplate><securityManager:EditAccessControlListControl id="EditAccessControlListControl" runat="server"/></ItemTemplate>
  </securityManager:ObjectBoundRepeater>
  --%>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <rubicon:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" CausesValidation="false" />
  <rubicon:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" Style="margin-left: 1em;" OnClick="CancelButton_Click" CausesValidation="false" />
  <rubicon:WebButton ID="NewAccessControlListButton" runat="server" Text="$res:NewAccessControlListButton" Style="margin-left: 1em;" OnClick="NewAccessControlListButton_Click" CausesValidation="False" />
  <%--<rubicon:WebButton ID="PostBackButton" runat="server" Text="PostBack" Style="margin-left: 1em;" CausesValidation="false" />--%>
</asp:Content>
