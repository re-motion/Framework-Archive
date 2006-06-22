<%@ Page Language="C#" AutoEventWireup="true" Codebehind="UserListForm.aspx.cs"
  Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.UserListForm" MasterPageFile="~/OrganizationalStructure/UI/OrganizationalStructure.Master" %>

<%@ Register TagPrefix="SecurityManager" Src="UserListControl.ascx" TagName="UserListControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <asp:Label ID="ErrorsOnPageLabel" runat="server" Text="###" CssClass="errorMessage" Visible="false" EnableViewState="false" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:UserListControl ID="UserListControl" runat="server"></SecurityManager:UserListControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder"></asp:Content>
