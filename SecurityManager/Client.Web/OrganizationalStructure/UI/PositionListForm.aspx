<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PositionListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.PositionListForm" MasterPageFile="OrganizationalStructureMasterPage.Master" %>
<%@ Register TagPrefix="securityManager" Src="PositionListControl.ascx" TagName="PositionListControl" %>
<%@ Register TagPrefix="securityManager" Src="ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:PositionListControl ID="PositionListControl" runat="server" />
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder"></asp:Content>
