<%@ Page Language="C#" AutoEventWireup="true" Codebehind="EditPermissionsForm.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionsForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>
<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl ID="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
  <rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.Metadata.SecurableClassDefinition, Rubicon.SecurityManager" />
  <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
    <tr>
      <td style="white-space: nowrap;" colspan="2">
        <rubicon:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Name" ReadOnly="True" />
      </td>
    </tr>
    <tr>
      <td class="formGridControlsCell">
      <securityManager:ObjectBoundRepeater ID="AccessControlListsRepeater" runat="server">
      <HeaderTemplate></HeaderTemplate>
      <FooterTemplate></FooterTemplate>
      <ItemTemplate></ItemTemplate>
      </securityManager:ObjectBoundRepeater>
      </td>
    </tr>
  </table>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <rubicon:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" CausesValidation="false" />
  <rubicon:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" Style="margin-left: 5px;" OnClick="CancelButton_Click" CausesValidation="false" />
</asp:Content>
