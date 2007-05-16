<%@ Page Language="C#" AutoEventWireup="true" Codebehind="UserListForm.aspx.cs" Inherits="UserListForm" MasterPageFile="../SecurityManagerMasterPage.Master" %>
<%@ Import namespace="System.Web.UI.WebControls"%>
<%@ Import namespace="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure"%>
<%@ Register TagPrefix="securityManager" Src="UserListControl.ascx" TagName="UserListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:UserListControl ID="UserListControl" runat="server"></securityManager:UserListControl>
</asp:Content>
