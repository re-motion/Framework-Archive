<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SecurableClassDefinitionListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.SecurableClassDefinitionListForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl ID="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.Metadata.SecurableClassDefinition, Rubicon.SecurityManager" />
  <rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
  <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
    <tr class="underlinedMarkerCellRow">
      <td class="formGridTitleCell" style="white-space: nowrap;">
        <rubicon:SmartLabel runat="server" ID="UserListLabel" Text="###" />
      </td>
      <td style="display: none; width: 100%"></td>
    </tr>
    <tr>
      <td style="height: 100%; vertical-align: top;">
        <rubicon:BocList ID="SecurableClassDefinitionList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="SecurableClassDefinitionList_ListItemCommandClick" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
          <FixedColumns>
            <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
              <PersistedCommand>
                <rubicon:BocListItemCommand Type="Event" />
              </PersistedCommand>
            </rubicon:BocSimpleColumnDefinition>
          </FixedColumns>
        </rubicon:BocList>
      </td>
    </tr>
  </table>
</asp:Content>
