<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupTypeListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.GroupTypeListForm" MasterPageFile="OrganizationalStructureMasterPage.Master" %>
<%@ Register TagPrefix="SecurityManager" Src="GroupTypeListControl.ascx" TagName="GroupTypeListControl" %>
<%@ Register TagPrefix="SecurityManager" Src="ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <SecurityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:GroupTypeListControl ID="GroupTypeListControl" runat="server"></SecurityManager:GroupTypeListControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder"></asp:Content>
