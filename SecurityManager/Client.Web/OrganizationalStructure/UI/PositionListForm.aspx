<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PositionListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.PositionListForm" MasterPageFile="~/OrganizationalStructure/UI/OrganizationalStructureMasterPage.Master" %>
<%@ Register TagPrefix="SecurityManager" Src="PositionListControl.ascx" TagName="PositionListControl" %>
<%@ Register TagPrefix="SecurityManager" Src="ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <SecurityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:PositionListControl ID="PositionListControl" runat="server"></SecurityManager:PositionListControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder"></asp:Content>
