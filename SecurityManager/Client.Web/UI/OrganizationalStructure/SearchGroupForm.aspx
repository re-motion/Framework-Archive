<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchGroupForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.UI.OrganizationalStructure.SearchGroupForm" MasterPageFile="OrganizationalStructureMasterPage.Master" %>
<%@ Register Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" TagPrefix="rubicon" %>
<%@ Register TagPrefix="securityManager" Src="SearchGroupControl.ascx" TagName="SearchGroupControl" %>
<%@ Register TagPrefix="securityManager" Src="ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:SearchGroupControl ID="SearchGroupControl" runat="server"></securityManager:SearchGroupControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <table cellpadding="0" cellspacing="0">
    <tr>
      <td>
        <rubicon:WebButton ID="CloseButton" runat="server" Text="$res:Close" OnClick="CloseButton_Click" CausesValidation="false"/>
      </td>
    </tr>
  </table>
</asp:Content>
