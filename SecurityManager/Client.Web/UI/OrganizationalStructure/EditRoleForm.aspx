<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRoleForm.aspx.cs" Inherits="Rubicon.SecurityManager.Client.Web.UI.OrganizationalStructure.EditRoleForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Register Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" TagPrefix="rubicon" %>
<%@ Register TagPrefix="securityManager" Src="EditRoleControl.ascx" TagName="EditRoleControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:EditRoleControl id="EditRoleControl" runat="server"></securityManager:EditRoleControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <table cellpadding="0" cellspacing="0">
    <tr>
      <td>
        <rubicon:WebButton ID="CloseButton" runat="server" Text="$res:Apply" OnClick="ApplyButton_Click" CausesValidation="false"/>
      </td>
      <td>
        <rubicon:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" style="margin-left: 5px;" 
          OnClick="CancelButton_Click" CausesValidation="false"/>
      </td>
    </tr>
  </table>
</asp:Content>
