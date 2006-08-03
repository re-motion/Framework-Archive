<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGroupTypeControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditGroupTypeControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>

<rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<rubicon:domainobjectdatasourcecontrol id="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.OrganizationalStructure.GroupType, Rubicon.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <rubicon:SmartLabel runat="server" id="GroupTypeLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Name">
        <TextBoxStyle MaxLength="100" />
      </rubicon:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocList ID="GroupsList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Groups" OnMenuItemClick="GroupsList_MenuItemClick" Selection="Multiple" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <rubicon:BocMenuItem ItemID="AddItem" Text="$res:Add">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </rubicon:BocMenuItem>
          <rubicon:BocMenuItem ItemID="RemoveItem" RequiredSelection="OneOrMore" Text="$res:Remove">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </rubicon:BocMenuItem>
        </ListMenuItems>
      </rubicon:BocList>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocList ID="PositionsList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Positions" OnMenuItemClick="PositionsList_MenuItemClick" Selection="Multiple">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Position">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <rubicon:BocMenuItem ItemID="NewItem" Text="$res:New">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </rubicon:BocMenuItem>
          <rubicon:BocMenuItem ItemID="EditItem" RequiredSelection="OneOrMore" Text="$res:Edit">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </rubicon:BocMenuItem>
          <rubicon:BocMenuItem ItemID="DeleteItem" RequiredSelection="OneOrMore" Text="$res:Delete">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </rubicon:BocMenuItem>
        </ListMenuItems>
      </rubicon:BocList>
    </td>
  </tr>
</table>
