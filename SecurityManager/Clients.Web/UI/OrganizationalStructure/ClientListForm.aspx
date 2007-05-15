<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure.ClientListForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Register TagPrefix="securityManager" Src="ClientListControl.ascx" TagName="ClientListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:ClientListControl ID="ClientListControl" runat="server" />
</asp:Content>
