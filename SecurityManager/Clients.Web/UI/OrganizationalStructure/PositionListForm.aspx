<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PositionListForm.aspx.cs" Inherits="PositionListForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Import namespace="System.Web.UI.WebControls"%>
<%@ Import namespace="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure"%>
<%@ Register TagPrefix="securityManager" Src="PositionListControl.ascx" TagName="PositionListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:PositionListControl ID="PositionListControl" runat="server"></securityManager:PositionListControl>
</asp:Content>
