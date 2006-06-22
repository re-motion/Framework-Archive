<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.GroupListForm" MasterPageFile="~/OrganizationalStructure/UI/OrganizationalStructure.Master" %>

<%@ Register TagPrefix="SecurityManager" Src="GroupListControl.ascx" TagName="GroupListControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <asp:Label ID="ErrorsOnPageLabel" runat="server" Text="###" CssClass="errorMessage" Visible="false" EnableViewState="false" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:GroupListControl ID="GroupListControl" runat="server"></SecurityManager:GroupListControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder"></asp:Content>
