<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGroupTypeControl.ascx.cs" Inherits="Rubicon.SecurityManager.Client.Web.UI.OrganizationalStructure.EditGroupTypeControl" %>
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
      <rubicon:BocList ID="GroupsField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Groups" OnMenuItemClick="GroupsField_MenuItemClick" Selection="Multiple">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition ItemID="GroupNameItem" PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <rubicon:BocMenuItem ItemID="AddItem" Text="$res:Add">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
          <rubicon:BocMenuItem ItemID="RemoveItem" RequiredSelection="OneOrMore" Text="$res:Remove">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
        </ListMenuItems>
      </rubicon:BocList>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocList ID="ConcretePositionsField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ConcretePositions" OnMenuItemClick="ConcretePositionsField_MenuItemClick" Selection="Multiple">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition ItemID="ConcretePositionNameItem" PropertyPathIdentifier="Name">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
          <rubicon:BocSimpleColumnDefinition ItemID="PositionNameItem" PropertyPathIdentifier="Position.Name">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <rubicon:BocMenuItem ItemID="NewItem" Text="$res:New">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
          <rubicon:BocMenuItem ItemID="EditItem" RequiredSelection="OneOrMore" Text="$res:Edit">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
          <rubicon:BocMenuItem ItemID="DeleteItem" RequiredSelection="OneOrMore" Text="$res:Delete">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
        </ListMenuItems>
      </rubicon:BocList>
    </td>
  </tr>
</table>
