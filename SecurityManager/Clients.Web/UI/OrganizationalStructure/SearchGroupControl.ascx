<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchGroupControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure.SearchGroupControl" %>

<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.OrganizationalStructure.Group, Rubicon.SecurityManager" />
<rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
  <tr>
    <td style="height: 100%;">
      <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
        <tr class="underlinedMarkerCellRow">
          <td class="formGridTitleCell" style="white-space: nowrap;">
            <rubicon:SmartLabel runat="server" id="GroupListLabel" Text="###"/>
          </td>
          <td style="DISPLAY: none;WIDTH: 100%"></td>
        </tr>
        <tr>
          <td style="height: 100%; vertical-align: top;">
            <rubicon:BocList ID="GroupList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="GroupList_ListItemCommandClick" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
              <FixedColumns>
                <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
                  <PersistedCommand>
                    <rubicon:BocListItemCommand />
                  </PersistedCommand>
                </rubicon:BocSimpleColumnDefinition>
                <rubicon:BocCommandColumnDefinition ItemID="ApplyItem" Text="$res:Apply" Width="0%">
                  <PersistedCommand>
                    <rubicon:BocListItemCommand Type="Event" />
                  </PersistedCommand>
                </rubicon:BocCommandColumnDefinition>
              </FixedColumns>
            </rubicon:BocList>
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>
