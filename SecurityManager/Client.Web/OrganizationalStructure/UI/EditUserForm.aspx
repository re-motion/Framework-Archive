<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditUserForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.EditUserForm" MasterPageFile="~/OrganizationalStructure/UI/OrganizationalStructure.Master" %>
<%@ Register Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" TagPrefix="rubicon" %>
<%@ Register TagPrefix="SecurityManager" Src="EditUserControl.ascx" TagName="EditUserControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <asp:Label ID="ErrorsOnPageLabel" runat="server" Text="###" CssClass="errorMessage" Visible="false" EnableViewState="false" />  
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <SecurityManager:EditUserControl id="EditUserControl" runat="server"></SecurityManager:EditUserControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <table cellpadding="0" cellspacing="0">
    <tr>
      <td>
        <rubicon:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" CausesValidation="false"/>
      </td>
      <td>
        <rubicon:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" style="margin-left: 5px;" 
          OnClick="CancelButton_Click" CausesValidation="false"/>
      </td>
    </tr>
  </table>
</asp:Content>

