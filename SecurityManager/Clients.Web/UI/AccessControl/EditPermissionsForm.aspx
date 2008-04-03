<%@ Page Language="C#" AutoEventWireup="true" Codebehind="EditPermissionsForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionsForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>
<%@ Register TagPrefix="securityManager" Src="EditAccessControlListControl.ascx" TagName="EditAccessControlListControl" %>

<asp:Content ID="ActualHeaderControlsPlaceHolder" runat="server" ContentPlaceHolderID="HeaderControlsPlaceHolder">
  <remotion:BindableObjectDataSourceControl ID="CurrentObjectHeaderControls" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" Mode="Read" />
  <h1><remotion:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObjectHeaderControls" PropertyIdentifier="DisplayName" /></h1>
</asp:Content>
<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder" />
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" />
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
  <remotion:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" CausesValidation="false" />
  <remotion:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" Style="margin-left: 1em;" OnClick="CancelButton_Click" CausesValidation="false" />
  <remotion:WebButton ID="NewAccessControlListButton" runat="server" Text="$res:NewAccessControlListButton" Style="margin-left: 1em;" OnClick="NewAccessControlListButton_Click" CausesValidation="False" />
  <%--<remotion:WebButton ID="PostBackButton" runat="server" Text="PostBack" Style="margin-left: 1em;" CausesValidation="false" />--%>
</asp:Content>
