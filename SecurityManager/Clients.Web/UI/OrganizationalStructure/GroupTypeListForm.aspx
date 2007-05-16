<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupTypeListForm.aspx.cs" Inherits="GroupTypeListForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Import namespace="System.Web.UI.WebControls"%>
<%@ Import namespace="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure"%>
<%@ Register TagPrefix="securityManager" Src="GroupTypeListControl.ascx" TagName="GroupTypeListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:GroupTypeListControl ID="GroupTypeListControl" runat="server"></securityManager:GroupTypeListControl>
</asp:Content>
