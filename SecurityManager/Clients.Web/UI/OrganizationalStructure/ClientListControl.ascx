<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientListControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure.ClientListControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>

<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.OrganizationalStructure.Client, Rubicon.SecurityManager" />
<rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
  <tr>
    <td style="height: 100%;">
      <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
        <tr class="underlinedMarkerCellRow">
          <td class="formGridTitleCell" style="white-space: nowrap;">
            <rubicon:SmartLabel runat="server" id="ClientListLabel" Text="###"/>
          </td>
          <td style="DISPLAY: none;WIDTH: 100%"></td>
        </tr>
        <tr>
          <td style="height: 100%; vertical-align: top;">
            <rubicon:BocList ID="ClientList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="ClientList_ListItemCommandClick" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
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
    </td>
  </tr>
  <tr>
    <td style="padding-left: 5px; padding-top: 5px;">
      <rubicon:WebButton ID="NewClientButton" runat="server" OnClick="NewClientButton_Click" />
    </td>
  </tr>
</table>
