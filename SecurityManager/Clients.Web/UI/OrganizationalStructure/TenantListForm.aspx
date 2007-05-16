<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TenantListForm.aspx.cs" Inherits="TenantListForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Import namespace="System.Web.UI.WebControls"%>
<%@ Import namespace="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure"%>
<%@ Register TagPrefix="securityManager" Src="TenantListControl.ascx" TagName="TenantListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:TenantListControl ID="_TenantListControl" runat="server" />
</asp:Content>
