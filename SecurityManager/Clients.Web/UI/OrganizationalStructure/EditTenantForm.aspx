<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditTenantForm.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditTenantForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Register Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" TagPrefix="rubicon" %>
<%@ Register TagPrefix="securityManager" Src="EditTenantControl.ascx" TagName="EditTenantControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:EditTenantControl id="_EditTenantControl" runat="server" />
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
