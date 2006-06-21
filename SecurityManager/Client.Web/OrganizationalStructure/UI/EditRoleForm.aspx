<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRoleForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.EditRoleForm" MasterPageFile="~/OrganizationalStructure/UI/OrganizationalStructure.Master" %>
<%@ Register Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" TagPrefix="rubicon" %>
<%@ Register TagPrefix="SecurityManager" Src="EditRoleControl.ascx" TagName="EditRoleControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <asp:Label ID="ErrorsOnPageLabel" runat="server" Text="###" CssClass="errorMessage" Visible="false" EnableViewState="false" />  
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:EditRoleControl id="EditRoleControl" runat="server"></SecurityManager:EditRoleControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <table cellpadding="0" cellspacing="0">
    <tr>
      <td>
        <rubicon:WebButton ID="CloseButton" runat="server" Text="$res:Close" OnClick="CloseButton_Click" CausesValidation="false"/>
      </td>
      <td>
        <rubicon:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" style="margin-left: 5px;" 
          OnClick="CancelButton_Click" CausesValidation="false"/>
      </td>
    </tr>
  </table>
</asp:Content>
