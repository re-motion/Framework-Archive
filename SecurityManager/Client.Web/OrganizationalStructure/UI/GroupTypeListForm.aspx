<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupTypeListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.GroupTypeListForm" MasterPageFile="~/OrganizationalStructure/UI/OrganizationalStructure.Master" %>

<%@ Register TagPrefix="SecurityManager" Src="GroupTypeListControl.ascx" TagName="GroupTypeListControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <asp:Label ID="ErrorsOnPageLabel" runat="server" Text="###" CssClass="errorMessage" Visible="false" EnableViewState="false" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:GroupTypeListControl ID="GroupTypeListControl" runat="server"></SecurityManager:GroupTypeListControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder"></asp:Content>
