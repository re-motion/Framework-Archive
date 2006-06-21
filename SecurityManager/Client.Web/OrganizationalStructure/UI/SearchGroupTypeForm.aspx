<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchGroupTypeForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.SearchGroupTypeForm" MasterPageFile="~/OrganizationalStructure/UI/OrganizationalStructure.Master" %>

<%@ Register TagPrefix="SecurityManager" Src="SearchGroupTypeControl.ascx" TagName="SearchGroupTypeControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <asp:Label ID="ErrorsOnPageLabel" runat="server" Text="###" CssClass="errorMessage" Visible="false" EnableViewState="false" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:SearchGroupTypeControl ID="SearchGroupTypeControl" runat="server"></SecurityManager:SearchGroupTypeControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder"></asp:Content>
