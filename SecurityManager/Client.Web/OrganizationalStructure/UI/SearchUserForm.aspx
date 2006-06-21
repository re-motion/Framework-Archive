<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SearchUserForm.aspx.cs"
  Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.SearchUserForm" MasterPageFile="~/OrganizationalStructure/UI/OrganizationalStructure.Master" %>

<%@ Register TagPrefix="SecurityManager" Src="SearchUserControl.ascx" TagName="SearchUserControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <asp:Label ID="ErrorsOnPageLabel" runat="server" Text="###" CssClass="errorMessage" Visible="false" EnableViewState="false" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:SearchUserControl ID="SearchUserControl" runat="server"></SecurityManager:SearchUserControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder"></asp:Content>
